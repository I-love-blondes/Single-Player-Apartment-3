Imports GTA
Imports GTA.Math
Imports GTA.Native
Imports NFunc = GTA.Native.Function
Imports SPAII.Compat

''' <summary>
''' Gerencia QUAIS predios estao carregados. Dados leves (posicoes, precos, donos) ficam sempre em memoria;
''' recursos pesados (menus, placas de venda, props, pins de interior, scaleforms) so existem para predios
''' proximos do jogador: carregam a menos de SpaSettings.LoadDistance e descarregam a mais de SpaSettings.UnloadDistance.
''' </summary>
Public Module ApartmentManager

    ''' <summary>Predios com recursos carregados agora. E sobre esta lista (pequena) que o tick por frame itera.</summary>
    Public ReadOnly Loaded As New List(Of BuildingClass)

    Private _nextSweep As Date = Date.MinValue
    Private _nextJanitor As Date = Date.UtcNow.AddSeconds(30)

    Public Function IsAtHome() As Boolean
        Return NFunc.Call(Of Boolean)(Hash.IS_INTERIOR_SCENE)
    End Function

    ''' <summary>Chamado todo frame; so trabalha de verdade a cada SweepIntervalMs.</summary>
    Public Sub Update()
        Dim nowUtc = Date.UtcNow
        If nowUtc >= _nextSweep Then
            _nextSweep = nowUtc.AddMilliseconds(SpaSettings.SweepIntervalMs)
            Try
                Sweep()
            Catch ex As Exception
                Logger.Log($"ApartmentManager.Sweep: {ex.Message} {ex.StackTrace}")
            End Try
        End If
        If nowUtc >= _nextJanitor Then
            _nextJanitor = nowUtc.AddSeconds(SpaSettings.JanitorIntervalSec)
            Janitor.Run()
        End If
    End Sub

    Private Sub Sweep()
        If Not buildingsLoaded Then Return
        Dim ped As Ped = Game.Player.Character
        If ped Is Nothing OrElse Not ped.Exists() Then Return
        Dim pos As Vector3 = ped.Position

        Dim unloadSq As Single = SpaSettings.UnloadDistance * SpaSettings.UnloadDistance
        Dim loadSq As Single = SpaSettings.LoadDistance * SpaSettings.LoadDistance

        ' nunca descarrega com menu aberto (os handlers de menu ainda usam os objetos)
        Dim canUnload As Boolean = Not MenuPool.IsAnyMenuOpen()
        Dim loadsLeft As Integer = SpaSettings.MaxLoadsPerSweep

        For i As Integer = 0 To buildings.Count - 1
            Dim bd As BuildingClass = buildings(i)
            Dim d2 As Single = bd.DistanceSquaredFrom(pos)

            If bd.RuntimeLoaded Then
                If canUnload AndAlso d2 > unloadSq AndAlso Not IsBuildingActive(bd) Then
                    bd.UnloadRuntime()
                    Loaded.Remove(bd)
                End If
            ElseIf d2 <= loadSq AndAlso loadsLeft > 0 Then
                bd.LoadRuntime()
                If bd.RuntimeLoaded Then
                    Loaded.Add(bd)
                    loadsLeft -= 1
                End If
            End If
        Next

        ApplyMissionBlipVisibility()
    End Sub

    ''' <summary>Predio em uso (jogador dentro do apartamento/garagem): NUNCA descarregar, mesmo que o interior fique a quilometros da porta.</summary>
    Public Function IsBuildingActive(bd As BuildingClass) As Boolean
        If HighEndApartment.Building Is bd Then Return True
        For Each apt In ActiveApartments()
            If bd.Apartments.Contains(apt) Then Return True
        Next
        Return False
    End Function

    Private Function ActiveApartments() As List(Of ApartmentClass)
        Dim r As New List(Of ApartmentClass)(5)
        If TwoCarGarage.Apartment IsNot Nothing Then r.Add(TwoCarGarage.Apartment)
        If SixCarGarage.Apartment IsNot Nothing Then r.Add(SixCarGarage.Apartment)
        If TenCarGarage.Apartment IsNot Nothing Then r.Add(TenCarGarage.Apartment)
        If MediumEndApartment.Apartment IsNot Nothing Then r.Add(MediumEndApartment.Apartment)
        If LowEndApartment.Apartment IsNot Nothing Then r.Add(LowEndApartment.Apartment)
        Return r
    End Function

    ''' <summary>IDs de interior que ainda sao necessarios agora (usado para liberar os demais).</summary>
    Public Function NeededInteriors() As HashSet(Of Integer)
        Dim need As New HashSet(Of Integer)
        If PI <> 0 Then need.Add(PI)
        If TwoCarGarage.Apartment IsNot Nothing Then need.Add(TwoCarGarage.Interior.GetInterior)
        If SixCarGarage.Apartment IsNot Nothing Then need.Add(SixCarGarage.Interior.GetInterior)
        If TenCarGarage.Apartment IsNot Nothing Then need.Add(TenCarGarage.Interior.GetInterior)
        If MediumEndApartment.Apartment IsNot Nothing Then need.Add(MediumEndApartment.Interior.GetInterior)
        If LowEndApartment.Apartment IsNot Nothing Then need.Add(LowEndApartment.Interior.GetInterior)
        If HighEndApartment.Building IsNot Nothing Then
            For Each apt In HighEndApartment.Building.Apartments
                need.Add(apt.InteriorPos.GetInterior)
            Next
        End If
        Return need
    End Function

    Private Sub ApplyMissionBlipVisibility()
        If Not SpaSettings.HideBlipsOnMission Then Return
        Dim mission As Boolean = IsMissionFlag()
        Dim alpha As Integer = If(mission, 0, 255)
        For i As Integer = 0 To buildings.Count - 1
            Dim bd = buildings(i)
            If bd.BuildingBlip IsNot Nothing AndAlso bd.BuildingBlip.Alpha <> alpha Then bd.BuildingBlip.Alpha = alpha
            If bd.GarageBlip IsNot Nothing AndAlso bd.GarageBlip.Alpha <> alpha Then bd.GarageBlip.Alpha = alpha
        Next
    End Sub

    ''' <summary>Descarrega tudo (script abortado/recarregado): nao deixa menus, props, blips nem pins para tras.</summary>
    Public Sub Shutdown()
        For Each bd In Loaded.ToList()
            Try
                bd.UnloadRuntime()
            Catch ex As Exception
                Logger.Log($"Shutdown.Unload: {ex.Message}")
            End Try
        Next
        Loaded.Clear()
        For Each bd In buildings
            Try
                bd.RemoveBlips()
            Catch ex As Exception
                Logger.Log($"Shutdown.Blips: {ex.Message}")
            End Try
        Next
        InteriorService.ReleaseAll()
        Janitor.ReleaseScaleforms()
        VehicleTags.Purge()
    End Sub

End Module
