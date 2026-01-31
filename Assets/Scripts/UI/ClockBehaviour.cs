using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TimeEvent;

/// <summary>
/// Controlador do relógio e sistema de tempo do jogo.
///
/// Responsabilidades:
/// - Controlar o ciclo dia/noite (rotação do ponteiro e iluminação)
/// - Contar os segundos e dias do jogo
/// - Disparar eventos agendados no TimeEvent
/// - Gerenciar a velocidade do tempo baseado no SpeedManager
///
/// O jogo usa um sistema de tempo próprio onde:
/// - Cada "dia" tem um número configurável de "segundos" (padrão: 5000)
/// - A velocidade pode ser ajustada (0.5x, 1x, 2x)
/// - O tempo pode ser pausado sem afetar outras mecânicas
///
/// Requer: TimeEvent (componente no mesmo GameObject)
/// </summary>
[RequireComponent(typeof(TimeEvent))]
public class ClockBehaviour : MonoBehaviour
{
    #region Configurações Serializadas

    /// <summary>
    /// Intervalo base entre atualizações visuais do relógio (em segundos reais).
    /// Valor menor = animação mais suave, maior custo de CPU.
    /// </summary>
    [SerializeField] private float numberCallClock = 0.1f;

    /// <summary>
    /// Luz direcional que simula o sol.
    /// Rotaciona ao longo do dia para simular ciclo dia/noite.
    /// </summary>
    [SerializeField] private Light lightWorld = null;

    /// <summary>
    /// Ponteiro visual do relógio na UI.
    /// Completa uma volta a cada dia do jogo.
    /// </summary>
    [SerializeField] private RectTransform pointer = null;

    /// <summary>
    /// Texto que exibe o número do dia atual.
    /// </summary>
    [SerializeField] private TextMeshProUGUI textDay = null;

    /// <summary>
    /// Configurações do jogo (ScriptableObject).
    /// Contém totalSecundsInDay e outras configurações.
    /// </summary>
    [SerializeField] private SettingData setting = null;

    /// <summary>
    /// Referência ao gerenciador de velocidade.
    /// Controla a multiplicação de velocidade do tempo.
    /// </summary>
    [SerializeField] private SpeedManager speedManager = null;

    /// <summary>
    /// Referência ao gerenciador principal do jogo.
    /// Usado para verificar estado de pausa.
    /// </summary>
    [SerializeField] private GameManager gameManager = null;

    #endregion

    #region Campos Estáticos

    /// <summary>
    /// Flag estática para pausar eventos do tempo.
    /// Pode ser acessada globalmente: ClockBehaviour.PauseEvent = true;
    /// </summary>
    public static bool PauseEvent = false;

    #endregion

    #region Campos Privados

    /// <summary>
    /// Referência ao sistema de eventos de tempo.
    /// </summary>
    private TimeEvent timeEvent = null;

    /// <summary>
    /// Total de segundos que compõem um dia do jogo.
    /// Carregado das configurações no Awake().
    /// </summary>
    private int totalDayTime = 5000;

    /// <summary>
    /// Graus de rotação por chamada de atualização.
    /// Calculado para que o ponteiro complete 360° em um dia.
    /// </summary>
    private float degreesPerCall = 0;

    /// <summary>
    /// Rotação atual do ponteiro do relógio.
    /// </summary>
    private Vector3 clockRotationEuler = Vector3.zero;

    /// <summary>
    /// Rotação atual da luz do sol.
    /// </summary>
    private Vector3 lightRotationEuler = Vector3.zero;

    #endregion

    #region Propriedades Públicas

    /// <summary>
    /// Dia atual do jogo (começa em 1).
    /// Atualiza automaticamente o texto da UI quando alterado.
    /// </summary>
    private int currentDay;
    public int CurrentDay
    {
        get
        {
            return currentDay;
        }
        private set
        {
            currentDay = value;
            textDay.text = currentDay.ToString();
        }
    }

    /// <summary>
    /// Total de segundos em um dia (somente leitura).
    /// </summary>
    public int TotalDaySec { get { return totalDayTime; } }

    /// <summary>
    /// Segundo atual do dia (0 a totalDayTime-1).
    /// </summary>
    public int CurrentSec { get; private set; }

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização precoce - obtém referências e configurações.
    /// </summary>
    private void Awake()
    {
        timeEvent = GetComponent<TimeEvent>();

        // Salva rotação inicial da luz
        lightRotationEuler = lightWorld.transform.rotation.eulerAngles;

        // Carrega configuração de duração do dia
        totalDayTime = setting.totalSecundsInDay;
    }

    /// <summary>
    /// Inicialização - configura estado inicial e inicia coroutines.
    /// </summary>
    private void Start()
    {
        // Estado inicial: Dia 1, meio-dia
        CurrentDay = 1;
        CurrentSec = totalDayTime / 2;
        clockRotationEuler = pointer.localEulerAngles;

        // Calcula rotação por atualização para completar 360° em um dia
        float numbCall360 = totalDayTime / numberCallClock;
        degreesPerCall = 360f / numbCall360;

        // Inicia as coroutines de tempo
        StartCoroutine(DayWork());
        StartCoroutine(CountTime());
    }

    #endregion

    #region Coroutines de Tempo

    /// <summary>
    /// Coroutine responsável pela animação visual do relógio e luz.
    /// Executa em intervalo menor que CountTime para animação suave.
    ///
    /// Atualiza:
    /// - Rotação do ponteiro do relógio (sentido anti-horário)
    /// - Rotação da luz direcional (simula movimento do sol)
    /// </summary>
    private IEnumerator DayWork()
    {
        while (true)
        {
            // Aguarda enquanto o jogo estiver pausado
            // IMPORTANTE: Evita divisão por zero quando SpeedGame = 0
            while (gameManager.Pause || PauseEvent || speedManager.SpeedGame <= 0f)
            {
                yield return null;
            }

            // Rotaciona o ponteiro do relógio (sentido anti-horário)
            clockRotationEuler -= Vector3.forward * degreesPerCall;
            pointer.localRotation = Quaternion.Euler(clockRotationEuler);

            // Rotaciona a luz do sol (simula passagem do dia)
            lightRotationEuler += Vector3.right * (degreesPerCall);
            lightWorld.transform.rotation = Quaternion.Euler(lightRotationEuler);

            // Aguarda intervalo ajustado pela velocidade do jogo
            float timeCall = numberCallClock / speedManager.SpeedGame;
            yield return new WaitForSeconds(timeCall);
        }
    }

    /// <summary>
    /// Coroutine principal de contagem de tempo.
    /// Executa a cada "segundo" do jogo e dispara eventos agendados.
    ///
    /// Responsabilidades:
    /// - Incrementar contador de segundos
    /// - Disparar eventos por segundo (actionPerSecond)
    /// - Detectar fim do dia e incrementar contador de dias
    /// - Disparar eventos de fim de dia (actionEndDay)
    /// - Executar eventos agendados (diários recorrentes e únicos)
    /// - Limpar eventos passados para evitar memory leak
    /// </summary>
    private IEnumerator CountTime()
    {
        while (true)
        {
            // Aguarda enquanto o jogo estiver pausado
            // IMPORTANTE: Evita divisão por zero quando SpeedGame = 0
            while (gameManager.Pause || PauseEvent || speedManager.SpeedGame <= 0f)
            {
                yield return null;
            }

            // Incrementa contador de segundos
            CurrentSec++;

            // Dispara eventos por segundo (se houver inscritos)
            if (timeEvent.actionPerSecond != null)
                timeEvent.actionPerSecond();

            // Verifica se o dia terminou
            if (CurrentSec >= totalDayTime)
            {
                CurrentSec = 0;
                CurrentDay++;

                // Reseta contador de eventos de doença do dia
                CharacterStatus.NumberAventsSicknessInday = 0;

                // Dispara eventos de fim de dia
                if (timeEvent.actionEndDay != null)
                    timeEvent.actionEndDay();

                // IMPORTANTE: Limpa eventos de dias passados para evitar memory leak
                timeEvent.CleanupPastDays(CurrentDay);
            }

            // Executa eventos diários recorrentes (ex: verificação de morte)
            ActionClockDelegate action;
            if (timeEvent.actionInSecEveryDay.TryGetValue(CurrentSec, out action) && action != null)
                action();

            // Executa eventos únicos agendados para este dia/segundo
            Dictionary<int, ActionClockDelegate> secDay;
            if (timeEvent.actionInDaySecundSpecific.TryGetValue(CurrentDay, out secDay) && secDay.TryGetValue(CurrentSec, out action))
            {
                action();
                // Remove evento após execução
                timeEvent.ClearDaySecondActions(CurrentDay, CurrentSec);
            }

            // Aguarda 1 segundo do jogo (ajustado pela velocidade)
            float timeCall = 1f / speedManager.SpeedGame;
            yield return new WaitForSeconds(timeCall);
        }
    }

    #endregion
}
