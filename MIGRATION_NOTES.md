# SPA II – port para SHVDN3 + LemonUI, com descarregamento dinâmico

Linguagem: **VB.NET** (o projeto original é VB; não foi convertido para C#). Alvo: .NET Framework 4.8.
Compilado contra **ScriptHookVDotNet 3.6.0**, **LemonUI.SHVDN3 2.2** e **iFruitAddon2 3.1.1**.

## Arquivos novos (`Core/`)
| Arquivo | Função |
|---|---|
| `ApartmentManager.vb` | Verificação de distância (a cada 500 ms) e carga/descarga por prédio |
| `Settings.vb` | Parâmetros de desempenho (módulo `SpaSettings`), lidos de `[PERFORMANCE]` no `modconfig.ini` |
| `InteriorService.vb` | `PIN_INTERIOR_IN_MEMORY` / `UNPIN_INTERIOR` e `REQUEST_IPL` / `REMOVE_IPL` rastreados |
| `Janitor.vb` | Limpeza periódica: veículos mortos, scaleforms, interiores ociosos, `GC.Collect` |
| `LemonCompat.vb` | Ponte `UIMenu`/`UIMenuItem`/`MenuPool` → LemonUI (`NativeMenu`, `ObjectPool`, `BigMessage`) |
| `VehicleCompat.vb`, `MetadataCompat.vb` | API de mods de veículo do SHVDN3; substituto do `Metadata.dll` |

## Causas reais do vazamento (encontradas no código original)
1. `PIN_INTERIOR_IN_MEMORY` era chamado ao entrar e **nunca** desfeito; IPLs pedidos nunca removidos.
2. 11 scaleforms (`MP_CAR_STATS_01..10`, `instructional_buttons`) carregados na inicialização e nunca liberados.
3. ~400 menus, ~95 placas de venda e todas as portas criados de uma vez; lógica de interior rodava N vezes por frame (uma por prédio); `config.GetValue` lido do INI a cada frame, por prédio.
4. `RefreshBlips` criava blips sem remover os antigos; `outVehicleList` nunca era podada.
5. `While Not model.IsLoaded` e `ChangeIPL` sem timeout; `SpawnForSaleSigns` recursivo sem limite; `Vehicle0.PlaceOnGround()` com NRE quando o modelo de DLC não existe.

## O que foi feito, por requisito
1. **Descarregamento dinâmico** – Prédio carrega a ≤ `LoadDistance` (200 m) e descarrega a > `UnloadDistance` (250 m); a histerese evita carregar/descarregar em loop. Descarregar = remover menus do pool (eventos desligados), deletar a placa de venda, liberar pins/IPLs após `InteriorReleaseGraceSec`. **Um prédio em uso (jogador dentro do apartamento/garagem) nunca é descarregado**, mesmo com o interior a km da porta (Z=-99), e nada é descarregado com menu aberto. O interior só é pinado ao entrar, e a reentrada espera o interior ficar pronto (com timeout).
2. **Memória** – blips idempotentes (sem duplicar), `VehicleTags.Purge`, poda de `outVehicleList`, scaleforms sob demanda, `Shutdown()` completo no `Aborted`, log com anti-flood e limite de 2 MB, `GC.Collect` só quando o heap passa de `GcThresholdMB` ou a cada `ForceGcMinutes`, fora de menus.
3. **LemonUI** – todo menu usa `NativeMenu` do LemonUI via `LemonCompat.vb`. `INMNativeUI.dll`, `Metadata.dll` e SHVDN2 não são mais referenciados.
4. **Exceções** – `VehicleData.Save` grava em `.tmp`, valida e só então substitui o XML (mantém `.bak`); `ReadFromFile` recupera do `.bak`; `Vehicles()` ignora XML corrompido sem esconder os demais; `CreateGarageVehicle` valida `IsInCdImage`, tem timeout e retorna `Nothing` se o DLC sumiu (o XML é preservado); `LoadVehicles` tem try/catch por veículo; `SaveVehicle` só apaga o arquivo antigo depois de salvar o novo e sempre restaura câmera/HUD/fade em caso de erro.

## Desvios e decisões que você deve conhecer
- **Ponte em vez de reescrever os ~34 handlers**: os handlers continuam com a assinatura antiga (`OnItemSelect`, `OnIndexChange`, `OnMenuClose`), mas quem desenha e trata entrada é o LemonUI. Duas diferenças de semântica foram tratadas de propósito: `OnIndexChange` não dispara ao abrir o menu, e `OnMenuClose` só dispara quando o jogador volta/cancela (não quando o código troca de menu).
- **Decorators removidos**: o `Metadata.dll` destravava a tabela de decorators via memória (frágil entre builds). O ID de garagem dos veículos agora fica num dicionário gerenciado (`VehicleTags`).
- **`Game.Globals(...).SetInt(1)` removido** do construtor: a API não existe no SHVDN3 e o índice do global só era mapeado até a build 2060; gravar lá na 2699 corromperia memória. Se algo da loja do jogo mudar de comportamento, foi aqui.
- **Blips continuam** para todas as propriedades (são os ícones de "à venda" do mapa); o que sai da memória são entidades, menus, pins e IPLs.
- Banner dos menus: o hack `Point(0,-107)` foi substituído por `Banner = Nothing`; banners de guarda-roupa são `ScaledTexture`.
- `GC.Collect` não libera recursos nativos do jogo; por isso pins/IPLs/scaleforms são liberados explicitamente.

## Instalação (substitui a versão antiga)
Na pasta `scripts`: **apague** `SPAII.dll`, `INMNativeUI.dll`, `Metadata.dll`, `iFruitAddon2.dll` antigos e copie os `.dll` do pacote.
Requer ScriptHookVDotNet **3.x** que suporte a sua build do jogo (SHVDN2 deve ser removido). Saves em `scripts\SPA II` continuam compatíveis (esquema do XML inalterado).

## `modconfig.ini` – seção `[PERFORMANCE]` (criada na primeira execução)
`UnloadDistance=250`, `LoadDistance=200`, `SweepIntervalMs=500`, `MaxLoadsPerSweep=2`, `InteriorReleaseGraceSec=20`, `JanitorIntervalSec=30`, `GcThresholdMB=150`, `ForceGcMinutes=10`, `ModelLoadTimeoutMs=3000`, `InteriorReadyTimeoutMs=4000`, `IplTimeoutMs=5000`. Valores inválidos são corrigidos automaticamente (ex.: `LoadDistance` sempre < `UnloadDistance`). Com `DebugMode=True`, o log registra heap, prédios carregados, pins e menus a cada ciclo do Janitor.

## Verificado × não verificado
- ✔ Compila sem erros contra SHVDN 3.6.0 / LemonUI 2.2 / iFruitAddon2 3.1.1.
- ✔ Testes automatizados (rodados): salvar/ler XML, recuperação de XML corrompido via `.bak`, falha de escrita sem exceção, leitura/validação das configurações, anti-flood do log.
- ✘ **Não testado dentro do GTA V** (não há como executar o jogo aqui). Teste antes de confiar:
  1. Andar até um prédio (>250 m → <200 m) e confirmar o menu de compra e a placa.
  2. Comprar, entrar, trocar o estilo do apartamento (IPL), sair; repetir em garagem de 2/6/10 carros.
  3. Salvar um carro; carregar com um carro de DLC desinstalado (deve logar e seguir).
  4. Sessão longa com `DebugMode=True`: o `heap` e `pins` devem estabilizar.
  5. Guarda-roupa: navegar e voltar (a pré-visualização depende de `OnIndexChange`).

## Revisao 2 e 3
- **Corrigido**: as 5 chamadas `Camera.InterpTo` (cameras de entrada/saida de apartamento e garagem) passavam `Boolean` para parametros `Int32`; no VB `True` vira -1 (no SHVDN2 era 1). Agora passam 1/0 explicitamente.
- Verificado: o codigo compila contra SHVDN 3.6.0 **e** contra a nightly v3.7.0-nightly.191, chamando exatamente as mesmas 186 assinaturas.
- Recarregar scripts: `ReloadKey` vem como `None` por padrao e **F4 abre o console do SHVDN**. Os scripts carregam sozinhos ao iniciar o jogo. Para testar, prefira **fechar e abrir o jogo**; recarregar recria o dominio do SHVDN e e uma causa conhecida de crash.
- **Pacote corrigido (rev3)**: faltavam `SPA II\Sounds` (audios do mecanico) e `SPA II\Garages`; agora estao incluidos.
- **Inicializacao a prova de falhas**: cada etapa do construtor tem try/catch proprio e as pastas `scripts\SPA II`, `Garages` e `Sounds` sao criadas se faltarem. Antes, qualquer excecao no construtor impedia o mod de iniciar (sem blips no mapa).
- **Como saber se o mod carregou**: o `SPA II.log` (pasta raiz do jogo) ganha, a cada abertura do jogo, `[STARTUP] SPA II iniciado | jogo <versao> | predios N | blips N` e depois `[STARTUP] primeiro tick executado`. Se essas linhas nao existirem, o SHVDN nao esta carregando o mod; se houver linhas `etapa ... falhou`, o erro esta descrito ali.

