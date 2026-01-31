using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

/// <summary>
/// Sistema de tratamento hospitalar para personagens doentes.
///
/// O Hospital é a principal estrutura de cura do jogo. Quando um personagem
/// doente é enviado para cá, ele recebe tratamento completo que inclui:
/// - Diagnóstico da doença
/// - Cura da infecção
/// - Vacinação automática contra a doença tratada
///
/// Mecânicas principais:
/// - Sistema de leitos com limite de capacidade
/// - Fila de espera quando lotado
/// - Custo diário por paciente internado
/// - Tempo de tratamento configurável
///
/// Interface: IRouteGlobal (permite ser destino de rota de personagens)
/// </summary>
public class Hospital : MonoBehaviour, IRouteGlobal
{
    #region Configurações de UI

    /// <summary>
    /// Ícone que indica presença de pacientes no hospital.
    /// Visível apenas quando há pelo menos 1 paciente.
    /// </summary>
    [SerializeField] private GameObject iconPatitents = null;

    /// <summary>
    /// Texto que exibe o custo diário atual dos tratamentos.
    /// Formato: "$-XX" (ex: "$-80" para 4 pacientes × $20/dia)
    /// </summary>
    [SerializeField] private TextMeshProUGUI countPatientText = null;

    /// <summary>
    /// Painel de gerenciamento do hospital (mostra leitos e pacientes).
    /// </summary>
    [SerializeField] private HospitalPanel panel = null;

    /// <summary>
    /// Painel de ações/status do paciente selecionado.
    /// </summary>
    [SerializeField] private ActionPanel statusPanel = null;

    #endregion

    #region Configurações de Tratamento

    [Space]
    /// <summary>
    /// Duração do tratamento em dias do jogo.
    /// Padrão: 4 dias para cura completa + vacinação.
    /// </summary>
    [SerializeField] private int dayTreat = 4;

    /// <summary>
    /// Custo total do tratamento completo (em moedas do jogo).
    /// Dividido pelo número de dias para calcular custo diário.
    /// Padrão: 80 moedas / 4 dias = 20 moedas/dia
    /// </summary>
    [SerializeField] private int costTotalTreat = 80;

    #endregion

    #region Configurações de Fila

    [Space]
    /// <summary>
    /// Ponto de início da fila de espera (Transform no mundo).
    /// Pacientes formam fila a partir deste ponto.
    /// </summary>
    [SerializeField] private Transform queueStart = null;

    /// <summary>
    /// Gerenciador de vacinas - para encontrar vacina correta após cura.
    /// </summary>
    [SerializeField] private DataVaccineManager vaccineManager = null;

    #endregion

    #region Campos Privados

    /// <summary>
    /// Posição atual do fim da fila (atualizada conforme chegam pacientes).
    /// </summary>
    private Vector3 posEndQueue = Vector3.zero;

    /// <summary>
    /// Custo calculado por dia de tratamento.
    /// </summary>
    private int costPerDay = 0;

    /// <summary>
    /// Número atual de pacientes internados (não inclui fila).
    /// </summary>
    private int numPatients = 0;

    /// <summary>
    /// Lista de personagens aguardando na fila de espera.
    /// </summary>
    private List<CharacterStatus> charactersInQueue = new List<CharacterStatus>();

    // Referências aos sistemas do jogo
    private ClockBehaviour clock;
    private TimeEvent timeEvent;
    private AlertPanel alertPanel;

    #endregion

    #region Propriedades

    /// <summary>
    /// Número atual de leitos ocupados (desbloqueados pelo jogador).
    /// </summary>
    public int NumberBed { get; set; }

    /// <summary>
    /// Número máximo de leitos disponíveis no hospital.
    /// </summary>
    public int NumberMaxBed { get { return 8; } }

    /// <summary>
    /// Número de pacientes internados.
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
        clock = FindObjectOfType<ClockBehaviour>();
        timeEvent = FindObjectOfType<TimeEvent>();
        alertPanel = FindObjectOfType<AlertPanel>();
    }

    /// <summary>
    /// Inicialização - calcula custo por dia e posição inicial da fila.
    /// </summary>
    private void Start()
    {
        costPerDay = costTotalTreat / dayTreat;
        posEndQueue = queueStart.position + (Vector3.back * 2);
    }

    #endregion

    #region Sistema de Tratamento

    /// <summary>
    /// Inicia o tratamento de um paciente.
    ///
    /// Fluxo:
    /// 1. Verifica se há leito disponível
    /// 2. Se sim: interna o paciente
    /// 3. Se não: adiciona à fila de espera
    ///
    /// Quando internado:
    /// - Esconde o personagem (está "dentro" do hospital)
    /// - Pausa verificações de morte
    /// - Ocupa um leito no painel
    /// - Agenda eventos de diagnóstico, custo diário e cura
    /// </summary>
    /// <param name="patient">Transform do personagem doente</param>
    public void TreatPatient(Transform patient)
    {
        CharacterStatus characterPatient = patient.GetComponent<CharacterStatus>();

        // Verifica disponibilidade de leito
        if (NumberBed > NumPatients)
        {
            // Interna o paciente
            NumPatients++;
            iconPatitents.SetActive(true);

            // Esconde o personagem durante internação
            GameObject patienteTreat = patient.GetComponent<BodyIA>().IA.gameObject;
            patienteTreat.SetActive(false);

            // Pausa verificações de morte durante tratamento
            characterPatient.InMedicalTreatment();

            // Ocupa um leito no painel visual
            BedItem bed = panel.OccupyBed(characterPatient, dayTreat);

            // Agenda cura para o fim do tratamento
            int dayFinishTreat = clock.CurrentDay + dayTreat;
            timeEvent.AddActionInSpecificSecAndDay(
                () => Treat(patienteTreat, bed),
                clock.CurrentSec,
                dayFinishTreat
            );

            // Agenda diagnóstico para o primeiro dia
            int daysCostMoney = clock.CurrentDay + 1;
            timeEvent.AddActionInSpecificSecAndDay(
                () => Diagnose(patient),
                clock.CurrentSec,
                daysCostMoney
            );

            // Agenda cobrança diária
            for (int i = daysCostMoney; i < dayFinishTreat; i++)
            {
                timeEvent.AddActionInSpecificSecAndDay(
                    () => Cost(bed),
                    clock.CurrentSec,
                    i
                );
            }

            // Mostra painéis de informação
            panel.OpenPanel();
            statusPanel.Show(patient.transform, false);
        }
        else
        {
            // Hospital lotado - adiciona à fila
            AddPatientInQueue(characterPatient);
        }
    }

    /// <summary>
    /// Trata o próximo paciente da fila de espera.
    /// Chamado automaticamente quando um leito é liberado.
    /// </summary>
    public void TreatPatientInQueue()
    {
        if (charactersInQueue.Count > 0)
        {
            TreatPatient(charactersInQueue[0].transform);
            RemovePatientInQueue(charactersInQueue[0]);
        }
    }

    /// <summary>
    /// Finaliza o tratamento de um paciente.
    ///
    /// Ações:
    /// 1. Reativa o personagem (sai do hospital)
    /// 2. Administra vacina contra a doença tratada
    /// 3. Cura completamente o personagem
    /// 4. Libera o leito
    /// 5. Cria alerta de alta hospitalar
    /// 6. Processa próximo da fila (se houver)
    /// </summary>
    /// <param name="patient">GameObject do personagem</param>
    /// <param name="bed">Item do leito ocupado</param>
    /// <returns>True sempre (para compatibilidade com delegate)</returns>
    private bool Treat(GameObject patient, BedItem bed)
    {
        CharacterStatus characterPatient = patient.GetComponentInChildren<CharacterStatus>();
        characterPatient.Activity = Activity.None;

        // Reativa o personagem
        patient.SetActive(true);

        // Administra vacina e cura
        TakeVaccine(characterPatient);
        characterPatient.Heal();

        // Libera leito
        --NumPatients;
        bed.Vacate();

        // Cria alerta de alta
        alertPanel.SpawnAlertHospital(characterPatient.GetComponent<BodyIA>().transform);

        // Atualiza ícone de pacientes
        if (NumPatients == 0)
            iconPatitents.SetActive(false);

        // Processa próximo da fila
        TreatPatientInQueue();

        return true;
    }

    #endregion

    #region Sistema de Fila

    /// <summary>
    /// Adiciona um paciente à fila de espera.
    ///
    /// O paciente caminha até a posição da fila e aguarda
    /// até que um leito seja liberado.
    /// </summary>
    /// <param name="character">Status do personagem</param>
    public void AddPatientInQueue(CharacterStatus character)
    {
        Debug.Log("Add in queue XD");

        // Marca como "na fila"
        character.Activity = Activity.Queue;

        // Calcula próxima posição na fila
        posEndQueue += (Vector3.back * 2);

        // Envia personagem para a posição
        character.GetComponent<NavMeshAgent>().SetDestination(posEndQueue);

        // Adiciona à lista de espera
        charactersInQueue.Add(character);
    }

    /// <summary>
    /// Remove um paciente da fila de espera.
    /// Chamado quando o paciente é internado.
    /// </summary>
    /// <param name="character">Status do personagem</param>
    private void RemovePatientInQueue(CharacterStatus character)
    {
        charactersInQueue.Remove(character);
    }

    #endregion

    #region Sistema de Custos

    /// <summary>
    /// Processa o custo diário de um leito ocupado.
    /// Deduz o valor do dinheiro do jogador.
    /// </summary>
    /// <param name="bed">Item do leito</param>
    /// <returns>True sempre (para compatibilidade com delegate)</returns>
    private bool Cost(BedItem bed)
    {
        MoneyManager.CurrentMoney -= costPerDay;
        bed.DaysTratment -= 1;
        return true;
    }

    #endregion

    #region Sistema de Diagnóstico

    /// <summary>
    /// Marca o paciente como diagnosticado.
    /// Isso permite que o jogador veja qual doença o paciente tem.
    /// </summary>
    /// <param name="patient">Transform do paciente</param>
    /// <returns>True sempre (para compatibilidade com delegate)</returns>
    private bool Diagnose(Transform patient)
    {
        CharacterStatus status = patient.GetComponentInChildren<CharacterStatus>();
        status.IsDiagnosed = true;

        // Cria alerta de diagnóstico
        alertPanel.SpawnAlertDiagnosis(patient.GetComponentInChildren<BodyIA>().transform);

        return true;
    }

    #endregion

    #region Sistema de Vacinação

    /// <summary>
    /// Administra a vacina correta ao paciente curado.
    ///
    /// Busca no gerenciador de vacinas qual vacina previne
    /// a doença que o paciente tinha e a administra.
    /// Isso confere imunidade contra reinfecção.
    /// </summary>
    /// <param name="patient">Status do paciente</param>
    /// <returns>True sempre (para compatibilidade com delegate)</returns>
    private bool TakeVaccine(CharacterStatus patient)
    {
        if (patient.SicknessGot != null)
        {
            // Procura vacina que previne a doença do paciente
            for (int i = 0; i < vaccineManager.vaccines.Length; i++)
            {
                if (patient.SicknessGot == vaccineManager.vaccines[i].prevents[0])
                {
                    patient.SetVaccineWantTake(vaccineManager.vaccines[i]);
                    patient.TakeVaccine();
                    break;
                }
            }
        }

        return true;
    }

    #endregion

    #region Interação

    /// <summary>
    /// Abre o painel do hospital quando clicado.
    /// </summary>
    private void OnMouseDown()
    {
        panel.OpenPanel();
    }

    #endregion
}
