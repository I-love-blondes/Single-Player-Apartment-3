Imports GTA

''' <summary>
''' Configuracoes de desempenho do SPA II. Lidas da secao [PERFORMANCE] de scripts\SPA II\modconfig.ini.
''' Se a secao nao existir, os valores padrao abaixo sao usados (e gravados uma unica vez).
''' </summary>
Public Module SpaSettings

    ''' <summary>Distancia (m) acima da qual um predio e descarregado (menus, placa, props). Padrao 250.</summary>
    Public UnloadDistance As Single = 250.0F
    ''' <summary>Distancia (m) abaixo da qual um predio e carregado. Menor que UnloadDistance (histerese, evita carrega/descarrega repetido).</summary>
    Public LoadDistance As Single = 200.0F
    ''' <summary>Intervalo entre verificacoes de distancia (ms).</summary>
    Public SweepIntervalMs As Integer = 500
    ''' <summary>Maximo de predios carregados por verificacao (suaviza picos de frame ao chegar em areas densas).</summary>
    Public MaxLoadsPerSweep As Integer = 2
    ''' <summary>Segundos que um interior/IPL fica "pinned" depois de deixar de ser necessario.</summary>
    Public InteriorReleaseGraceSec As Integer = 20
    ''' <summary>Intervalo da limpeza periodica (veiculos mortos, scaleforms, GC) em segundos.</summary>
    Public JanitorIntervalSec As Integer = 30
    ''' <summary>Heap gerenciado (MB) acima do qual a limpeza forca uma coleta do GC.</summary>
    Public GcThresholdMB As Integer = 150
    ''' <summary>Coleta de GC forcada a cada N minutos mesmo abaixo do limite (0 = desligado).</summary>
    Public ForceGcMinutes As Integer = 10
    ''' <summary>Tempo maximo (ms) esperando um modelo de veiculo carregar antes de desistir.</summary>
    Public ModelLoadTimeoutMs As Integer = 3000
    ''' <summary>Tempo maximo (ms) esperando um interior ficar pronto antes de teleportar o jogador (evita cair no vazio).</summary>
    Public InteriorReadyTimeoutMs As Integer = 4000
    ''' <summary>Tempo maximo (ms) esperando um IPL ficar ativo.</summary>
    Public IplTimeoutMs As Integer = 5000
    ''' <summary>Esconde os blips das propriedades durante missoes.</summary>
    Public HideBlipsOnMission As Boolean = True

    Private Const Section As String = "PERFORMANCE"

    Public Sub Load(cfg As ScriptSettings)
        Try
            UnloadDistance = cfg.GetValue(Of Single)(Section, "UnloadDistance", UnloadDistance)
            LoadDistance = cfg.GetValue(Of Single)(Section, "LoadDistance", LoadDistance)
            SweepIntervalMs = cfg.GetValue(Of Integer)(Section, "SweepIntervalMs", SweepIntervalMs)
            MaxLoadsPerSweep = cfg.GetValue(Of Integer)(Section, "MaxLoadsPerSweep", MaxLoadsPerSweep)
            InteriorReleaseGraceSec = cfg.GetValue(Of Integer)(Section, "InteriorReleaseGraceSec", InteriorReleaseGraceSec)
            JanitorIntervalSec = cfg.GetValue(Of Integer)(Section, "JanitorIntervalSec", JanitorIntervalSec)
            GcThresholdMB = cfg.GetValue(Of Integer)(Section, "GcThresholdMB", GcThresholdMB)
            ForceGcMinutes = cfg.GetValue(Of Integer)(Section, "ForceGcMinutes", ForceGcMinutes)
            ModelLoadTimeoutMs = cfg.GetValue(Of Integer)(Section, "ModelLoadTimeoutMs", ModelLoadTimeoutMs)
            IplTimeoutMs = cfg.GetValue(Of Integer)(Section, "IplTimeoutMs", IplTimeoutMs)
            InteriorReadyTimeoutMs = cfg.GetValue(Of Integer)(Section, "InteriorReadyTimeoutMs", InteriorReadyTimeoutMs)
            HideBlipsOnMission = cfg.GetValue(Of Boolean)("SETTING", "HideBlipsOnMission", HideBlipsOnMission)
            Validate()
            WriteDefaultsIfMissing(cfg)
        Catch ex As Exception
            Logger.Log($"SpaSettings.Load: {ex.Message}")
            Validate()
        End Try
    End Sub

    ''' <summary>Garante valores sensatos mesmo se o usuario editar o .ini errado.</summary>
    Private Sub Validate()
        If UnloadDistance < 100.0F Then UnloadDistance = 100.0F
        If LoadDistance < 50.0F Then LoadDistance = 50.0F
        If LoadDistance >= UnloadDistance Then LoadDistance = UnloadDistance - 50.0F   ' histerese obrigatoria
        If SweepIntervalMs < 100 Then SweepIntervalMs = 100
        If MaxLoadsPerSweep < 1 Then MaxLoadsPerSweep = 1
        If InteriorReleaseGraceSec < 5 Then InteriorReleaseGraceSec = 5
        If JanitorIntervalSec < 5 Then JanitorIntervalSec = 5
        If GcThresholdMB < 32 Then GcThresholdMB = 32
        If ForceGcMinutes < 0 Then ForceGcMinutes = 0
        If ModelLoadTimeoutMs < 500 Then ModelLoadTimeoutMs = 500
        If IplTimeoutMs < 1000 Then IplTimeoutMs = 1000
        If InteriorReadyTimeoutMs < 500 Then InteriorReadyTimeoutMs = 500
    End Sub

    Private Sub WriteDefaultsIfMissing(cfg As ScriptSettings)
        Try
            If cfg.GetValue(Of String)(Section, "UnloadDistance", Nothing) IsNot Nothing Then Return
            cfg.SetValue(Of Single)(Section, "UnloadDistance", UnloadDistance)
            cfg.SetValue(Of Single)(Section, "LoadDistance", LoadDistance)
            cfg.SetValue(Of Integer)(Section, "SweepIntervalMs", SweepIntervalMs)
            cfg.SetValue(Of Integer)(Section, "MaxLoadsPerSweep", MaxLoadsPerSweep)
            cfg.SetValue(Of Integer)(Section, "InteriorReleaseGraceSec", InteriorReleaseGraceSec)
            cfg.SetValue(Of Integer)(Section, "JanitorIntervalSec", JanitorIntervalSec)
            cfg.SetValue(Of Integer)(Section, "GcThresholdMB", GcThresholdMB)
            cfg.SetValue(Of Integer)(Section, "ForceGcMinutes", ForceGcMinutes)
            cfg.SetValue(Of Integer)(Section, "ModelLoadTimeoutMs", ModelLoadTimeoutMs)
            cfg.SetValue(Of Integer)(Section, "IplTimeoutMs", IplTimeoutMs)
            cfg.SetValue(Of Integer)(Section, "InteriorReadyTimeoutMs", InteriorReadyTimeoutMs)
            cfg.Save()
        Catch ex As Exception
            Logger.Log($"Settings.WriteDefaults: {ex.Message}")
        End Try
    End Sub

End Module
