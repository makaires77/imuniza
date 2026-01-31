using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Sistema de diagnóstico laboratorial para personagens doentes.
///
/// O Laboratório é uma estrutura especializada que realiza exames para
/// diagnosticar doenças. Diferente do Hospital, o laboratório:
/// - NÃO cura o personagem
/// - Apenas identifica a doença (diagnóstico)
/// - É mais rápido e mais barato que o Hospital
///
/// Mecânicas principais:
/// - Tempo de exame configurável (padrão: 1 dia)
/// - Custo diário de operação
/// - Personagem fica escondido durante o exame
/// - Após diagnóstico, personagem retorna ainda doente
///
/// Uso estratégico:
/// O diagnóstico permite que o jogador saiba qual doença o personagem tem,
/// possibilitando escolher o tratamento correto ou priorizar casos graves.
///
/// Interface: IRouteGlobal (permite ser destino de rota de personagens)
/// </summary>
public class Laboratory : MonoBehaviour, IRouteGlobal
{
    #region Configurações de UI

    /// <summary>
    /// Texto que exibe o custo diário atual dos exames.
    /// Formato: "$-XX" (ex: "$-15" para 1 paciente × $15/dia)
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI countPatientText = null;

    /// <summary>
    /// Ícone que indica presença de pacientes no laboratório.
    /// Visível apenas quando há pelo menos 1 paciente.
    /// </summary>
    [SerializeField]
    private GameObject iconPatients = null;

    #endregion

    #region Configurações de Tratamento

    [Space]
    /// <summary>
    /// Duração do exame em dias do jogo.
    /// Padrão: 1 dia para diagnóstico completo.
    /// </summary>
    [SerializeField]
    private int dayTreat = 1;

    /// <summary>
    /// Custo total do exame (em moedas do jogo).
    /// Dividido pelo número de dias para calcular custo diário.
    /// Padrão: 15 moedas / 1 dia = 15 moedas/dia
    /// </summary>
    [SerializeField]
    private int costTotalTreat = 15;

    /// <summary>
    /// Custo calculado por dia de exame.
    /// </summary>
    private int costPerDay = 0;

    #endregion

    #region Referências de Sistemas

    /// <summary>
    /// Referência ao sistema de tempo do jogo.
    /// </summary>
    private ClockBehaviour clock;

    /// <summary>
    /// Referência ao sistema de eventos de tempo.
    /// </summary>
    private TimeEvent timeEvent;

    /// <summary>
    /// Painel de alertas para mostrar notificações de diagnóstico.
    /// </summary>
    private AlertPanel alertPanel;

    #endregion

    #region Campos de Estado

    /// <summary>
    /// Número atual de pacientes em exame.
    /// </summary>
    private int numPatients = 0;

    /// <summary>
    /// Número de pacientes em exame.
    /// Atualiza automaticamente o texto de custo na UI.
    /// </summary>
    private int NumPatients
    {
        get
        {
            return numPatients;
        }
        set
        {
            numPatients = value;
            countPatientText.text = "$-" + (numPatients * costPerDay).ToString();

            // Atualiza visibilidade do ícone de pacientes
            if (iconPatients != null)
                iconPatients.SetActive(numPatients > 0);
        }
    }

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização precoce - obtém referências aos sistemas.
    /// </summary>
    private void Awake()
    {
        alertPanel = FindObjectOfType<AlertPanel>();
        clock = FindObjectOfType<ClockBehaviour>();
        timeEvent = FindObjectOfType<TimeEvent>();
    }

    /// <summary>
    /// Inicialização - calcula custo por dia.
    /// </summary>
    private void Start()
    {
        costPerDay = costTotalTreat / dayTreat;
    }

    #endregion

    #region Sistema de Diagnóstico

    /// <summary>
    /// Inicia o exame diagnóstico de um paciente.
    ///
    /// Fluxo:
    /// 1. Incrementa contador de pacientes
    /// 2. Obtém referências do personagem
    /// 3. Esconde o personagem (está "dentro" do laboratório)
    /// 4. Pausa verificações de morte durante o exame
    /// 5. Agenda evento de conclusão do diagnóstico
    /// 6. Agenda cobrança diária
    ///
    /// NOTA: O personagem sai do laboratório ainda doente,
    /// mas com a doença identificada (IsDiagnosed = true).
    /// </summary>
    /// <param name="patient">Transform do personagem doente</param>
    public void TreatPatient(Transform patient)
    {
        NumPatients++;

        // Obtém componentes do paciente
        CharacterStatus characterPatient = patient.GetComponent<CharacterStatus>();
        BodyIA bodyIA = patient.GetComponent<BodyIA>();

        // Validação de segurança
        if (bodyIA == null)
        {
            Debug.LogError("Laboratory: BodyIA not found on patient");
            return;
        }

        IA patientIA = bodyIA.IA;
        if (patientIA == null)
        {
            Debug.LogError("Laboratory: IA not found on patient");
            return;
        }

        GameObject patientGO = patientIA.gameObject;

        // Esconde paciente durante exame
        patientGO.SetActive(false);

        // Pausa verificações de morte durante exame
        characterPatient.InMedicalTreatment();

        // Agenda término do exame
        int dayFinishTreat = clock.CurrentDay + dayTreat;
        timeEvent.AddActionInSpecificSecAndDay(
            () => Treat(patientGO, characterPatient),
            clock.CurrentSec,
            dayFinishTreat
        );

        // Agenda cobrança para o dia seguinte
        timeEvent.AddActionInSpecificSecAndDay(
            () => Cost(),
            clock.CurrentSec,
            clock.CurrentDay + 1
        );
    }

    /// <summary>
    /// Finaliza o exame diagnóstico de um paciente.
    ///
    /// Ações:
    /// 1. Decrementa contador de pacientes
    /// 2. Reativa o personagem (sai do laboratório)
    /// 3. Marca como diagnosticado
    /// 4. Retorna personagem ao mundo (ainda doente)
    /// 5. Cria alerta de diagnóstico completo
    /// </summary>
    /// <param name="patient">GameObject do personagem</param>
    /// <param name="status">Status do personagem</param>
    /// <returns>True sempre (para compatibilidade com delegate)</returns>
    private bool Treat(GameObject patient, CharacterStatus status)
    {
        NumPatients--;

        // Reativa o personagem
        patient.SetActive(true);

        // Marca como diagnosticado (doença identificada)
        status.IsDiagnosed = true;

        // Retorna paciente ao mundo (ainda doente, mas diagnosticado)
        // Isso reativa as verificações de morte
        status.RetunTheLaboratory();

        // Cria alerta de diagnóstico completo
        BodyIA bodyIA = patient.GetComponentInChildren<BodyIA>();
        if (bodyIA != null)
            alertPanel.SpawnAlertDiagnosis(bodyIA.transform);

        return true;
    }

    /// <summary>
    /// Processa o custo diário do laboratório.
    /// Deduz o valor do dinheiro do jogador.
    /// </summary>
    /// <returns>True sempre (para compatibilidade com delegate)</returns>
    private bool Cost()
    {
        MoneyManager.CurrentMoney -= costPerDay;
        return true;
    }

    #endregion
}
