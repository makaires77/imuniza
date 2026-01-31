using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Sistema de vacinação preventiva no Centro de Saúde.
///
/// O Centro de Saúde é uma estrutura focada em prevenção que permite
/// vacinar personagens saudáveis contra doenças. Diferente do Hospital:
/// - NÃO trata doenças (não cura)
/// - Apenas administra vacinas preventivas
/// - É mais rápido e mais barato
/// - Personagens saem com imunidade contra uma doença
///
/// Mecânicas principais:
/// - Tempo de vacinação configurável (padrão: 1 dia)
/// - Custo diário de operação
/// - Personagem fica escondido durante a vacinação
/// - Após vacinação, personagem retorna com imunidade
///
/// Uso estratégico:
/// Vacinar personagens saudáveis antes que contraiam doenças é mais
/// eficiente do que tratar depois. Ideal para controle de epidemias.
///
/// Interface: IRouteGlobal (permite ser destino de rota de personagens)
/// </summary>
public class HealthCenter : MonoBehaviour, IRouteGlobal
{
    #region Configurações de UI

    /// <summary>
    /// Ícone que indica presença de pacientes no centro de saúde.
    /// Visível apenas quando há pelo menos 1 paciente.
    /// </summary>
    [SerializeField]
    private GameObject iconPatitents = null;

    /// <summary>
    /// Texto que exibe o custo diário atual das vacinações.
    /// Formato: "$-XX" (ex: "$-10" para 1 paciente × $10/dia)
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI countPatientText = null;

    #endregion

    #region Configurações de Tratamento

    [Space]
    /// <summary>
    /// Duração da vacinação em dias do jogo.
    /// Padrão: 1 dia para vacinação completa.
    /// </summary>
    [SerializeField]
    private int dayTreat = 1;

    /// <summary>
    /// Custo total da vacinação (em moedas do jogo).
    /// Dividido pelo número de dias para calcular custo diário.
    /// Padrão: 10 moedas / 1 dia = 10 moedas/dia
    /// </summary>
    [SerializeField]
    private int costTotalTreat = 10;

    /// <summary>
    /// Custo calculado por dia de vacinação.
    /// </summary>
    private int costPerDay = 0;

    #endregion

    #region Referências de Sistemas

    /// <summary>
    /// Referência ao sistema de eventos de tempo.
    /// </summary>
    private TimeEvent timeEvent;

    /// <summary>
    /// Referência ao sistema de tempo do jogo.
    /// </summary>
    private ClockBehaviour clock;

    /// <summary>
    /// Painel de alertas para mostrar notificações de vacinação.
    /// </summary>
    private AlertPanel alertPanel;

    #endregion

    #region Campos de Estado

    /// <summary>
    /// Número atual de pacientes em vacinação.
    /// </summary>
    private int numPatients = 0;

    /// <summary>
    /// Número de pacientes em vacinação.
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
        }
    }

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização precoce - obtém referências aos sistemas.
    /// </summary>
    private void Awake()
    {
        timeEvent = FindObjectOfType<TimeEvent>();
        clock = FindObjectOfType<ClockBehaviour>();
        alertPanel = FindObjectOfType<AlertPanel>();
    }

    /// <summary>
    /// Inicialização - calcula custo por dia.
    /// </summary>
    private void Start()
    {
        costPerDay = costTotalTreat / dayTreat;
    }

    #endregion

    #region Sistema de Vacinação

    /// <summary>
    /// Inicia o processo de vacinação de um paciente.
    ///
    /// Fluxo:
    /// 1. Incrementa contador de pacientes
    /// 2. Esconde o personagem (está "dentro" do centro de saúde)
    /// 3. Pausa verificações de morte durante a vacinação
    /// 4. Agenda evento de conclusão da vacinação
    /// 5. Agenda cobrança diária
    ///
    /// O personagem deve ter previamente definido qual vacina deseja
    /// através de SetVaccineWantTake().
    /// </summary>
    /// <param name="patient">Transform do personagem a vacinar</param>
    public void TreatPatient(Transform patient)
    {
        NumPatients++;
        iconPatitents.SetActive(true);

        // Esconde o personagem durante a vacinação
        GameObject patienteTreat = patient.GetComponentInParent<IA>().gameObject;
        patienteTreat.SetActive(false);

        // Pausa verificações de morte durante o processo
        patient.GetComponent<CharacterStatus>().InMedicalTreatment();

        // Agenda término da vacinação
        int day = clock.CurrentDay + dayTreat;
        timeEvent.AddActionInSpecificSecAndDay(
            () => Treat(patienteTreat),
            clock.CurrentSec,
            day
        );

        // Agenda cobrança diária
        int daysCostMoney = clock.CurrentDay + 1;
        for (int i = 0; i < dayTreat; i++)
        {
            timeEvent.AddActionInSpecificSecAndDay(
                () => Cost(),
                clock.CurrentSec,
                daysCostMoney
            );
            daysCostMoney++;
        }
    }

    /// <summary>
    /// Finaliza o processo de vacinação de um paciente.
    ///
    /// Ações:
    /// 1. Decrementa contador de pacientes
    /// 2. Reativa o personagem (sai do centro de saúde)
    /// 3. Administra a vacina (confere imunidade)
    /// 4. Cria alerta de vacinação completa
    /// 5. Atualiza visual do personagem baseado no estado de saúde
    /// </summary>
    /// <param name="patient">GameObject do personagem</param>
    /// <returns>True sempre (para compatibilidade com delegate)</returns>
    private bool Treat(GameObject patient)
    {
        --NumPatients;

        // Reativa o personagem
        patient.SetActive(true);

        // Administra a vacina
        DataVaccine vaccine = patient.GetComponentInChildren<CharacterStatus>().TakeVaccine();

        // Cria alerta de vacinação completa
        alertPanel.SpawnAlertHelthCenter(
            patient.GetComponentInChildren<BodyIA>().transform,
            vaccine
        );

        // Atualiza ícone de pacientes
        if (NumPatients == 0)
            iconPatitents.SetActive(false);

        // Atualiza visual baseado no estado de saúde
        CharacterStatus status = patient.GetComponentInChildren<CharacterStatus>();
        StatusBehaviour statusBehaviour = status.GetComponentInChildren<StatusBehaviour>();

        switch (status.Health)
        {
            case HealthCondition.Healthy:
                // Personagem saudável - visual normal
                statusBehaviour.SetHeal();
                break;
            case HealthCondition.Sick:
                // Personagem ainda doente - visual de doente
                // (pode acontecer se foi vacinado enquanto doente)
                statusBehaviour.SetSick();
                break;
        }

        return true;
    }

    /// <summary>
    /// Processa o custo diário do centro de saúde.
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
