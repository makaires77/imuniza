using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Gerencia o estado de saúde e dados de um personagem (NPC).
///
/// Este é um dos componentes mais importantes do jogo, responsável por:
/// - Armazenar dados pessoais (nome, idade, sistema imunológico)
/// - Gerenciar estados de saúde (saudável, doente, morto)
/// - Controlar vacinas tomadas e imunidades
/// - Agendar verificações de morte para personagens doentes
/// - Comunicar mudanças de estado para a UI (HUD, Dashboard)
///
/// Ciclo de vida do personagem:
/// SAUDÁVEL → [contrai doença] → DOENTE → [tratamento] → CURADO (+ vacina)
///                                      → [sem tratamento] → MORTO
///
/// Anexado a: Prefab do personagem (IA_Man, IA_Test)
/// </summary>
public class CharacterStatus : MonoBehaviour
{
    #region Propriedades Públicas - Dados do Personagem

    /// <summary>
    /// Nome completo do personagem (gerado aleatoriamente).
    /// </summary>
    public string FullName { get; private set; }

    /// <summary>
    /// Idade do personagem (10-60 anos).
    /// Atualmente apenas visual, não afeta gameplay.
    /// </summary>
    public int Age { get; private set; }

    /// <summary>
    /// Estado atual de saúde do personagem.
    /// Saudável, Doente ou Morto.
    /// </summary>
    public HealthCondition Health { get; private set; }

    /// <summary>
    /// Atividade atual do personagem (Nenhuma ou Na Fila).
    /// </summary>
    public Activity Activity { get; set; }

    /// <summary>
    /// Força do sistema imunológico (40-80%).
    /// Afeta a chance de contrair doenças.
    /// Fórmula: chanceInfecção = transmissibilidade - imuneSystem
    /// </summary>
    public int ImuneSystem { get; private set; }

    /// <summary>
    /// Lista de vacinas que o personagem já tomou.
    /// Confere imunidade contra as doenças correspondentes.
    /// </summary>
    public List<DataVaccine> VaccinesTaken { get; private set; }

    /// <summary>
    /// Doença que o personagem contraiu (null se saudável).
    /// </summary>
    public DataSickness SicknessGot { get; private set; }

    /// <summary>
    /// Indica se a doença foi diagnosticada (no laboratório).
    /// </summary>
    public bool IsDiagnosed { get; set; }

    /// <summary>
    /// Indica se o personagem pode ficar doente.
    /// Definido como true após a inicialização.
    /// </summary>
    public bool IsPermissionSick { get; private set; } = false;

    /// <summary>
    /// Indica se a ambulância (IML) foi chamada para este personagem.
    /// </summary>
    public bool IsCallIML { get; set; }

    #endregion

    #region Campos Serializados

    /// <summary>
    /// ScriptableObject com lista de nomes possíveis.
    /// </summary>
    [SerializeField] private DataName dataNames = null;

    /// <summary>
    /// Prefab da zona de doença que aparece quando o personagem morre.
    /// Corpos mortos podem infectar outros personagens próximos.
    /// </summary>
    [SerializeField] private DiseaseHumanPoint prefabDisease = null;

    #endregion

    #region Campos Privados

    /// <summary>
    /// Zona de doença instanciada quando o personagem morre.
    /// </summary>
    private DiseaseHumanPoint diseaseSpawned = null;

    // Referências a outros sistemas
    private HUD hud = null;
    private TimeEvent timeEvent = null;
    private ClockBehaviour clock = null;
    private AlertPanel alertPanel = null;
    private static DebugPanel debugPanel = null;
    private Dashboard dashboard;

    // Componentes do personagem
    private StatusBehaviour statusBehaviour;
    private Animator anim;
    private NavMeshAgent agent;

    /// <summary>
    /// Segundo do dia em que o personagem ficou doente.
    /// Usado para agendar verificações diárias de morte.
    /// </summary>
    private int secGotSick = -1;

    /// <summary>
    /// Delegate para verificação de morte.
    /// Agendado para executar diariamente no segundo em que ficou doente.
    /// </summary>
    private TimeEvent.ActionClockDelegate checkDeadDelegate;

    /// <summary>
    /// Vacina que o personagem deseja/vai tomar.
    /// </summary>
    private DataVaccine vaccineWantTake = null;

    #endregion

    #region Campos Estáticos

    /// <summary>
    /// Contador de eventos de doença no dia atual.
    /// Usado para limitar transmissões por dia.
    /// Resetado pelo ClockBehaviour ao início de cada dia.
    /// </summary>
    private static int numberAventsSicknessInday = 0;
    public static int NumberAventsSicknessInday
    {
        get { return numberAventsSicknessInday; }
        set { numberAventsSicknessInday = value; }
    }

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização precoce - obtém referências aos outros sistemas.
    /// </summary>
    private void Awake()
    {
        // Encontra referências globais
        hud = FindObjectOfType<HUD>();
        timeEvent = FindObjectOfType<TimeEvent>();
        clock = FindObjectOfType<ClockBehaviour>();
        alertPanel = FindObjectOfType<AlertPanel>();
        debugPanel = FindObjectOfType<DebugPanel>();
        dashboard = FindObjectOfType<Dashboard>();

        // Obtém componentes do personagem
        statusBehaviour = GetComponentInChildren<StatusBehaviour>();
        agent = GetComponentInChildren<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Inicialização - configura estado inicial do personagem.
    /// </summary>
    private void Start()
    {
        VaccinesTaken = new List<DataVaccine>();
        IsCallIML = false;
        Activity = Activity.None;

        // NOTA: O delegate checkDeadDelegate é criado apenas em Sicken()
        // quando SicknessGot é garantidamente não-null.
        // Isso evita NullReferenceException.

        SetData();
    }

    #endregion

    #region Configuração Inicial

    /// <summary>
    /// Gera dados aleatórios para o personagem.
    /// Chamado no Start() após spawn.
    /// </summary>
    private void SetData()
    {
        // Gera nome aleatório
        FullName = dataNames.GetRandomFullName();

        // Gera atributos aleatórios
        Age = UnityEngine.Random.Range(10, 60);
        ImuneSystem = UnityEngine.Random.Range(40, 80);

        // Inicia saudável
        Health = HealthCondition.Healthy;

        // Atualiza contadores na HUD
        hud.UpdateHealthy();
        hud.UpdateSick();

        // Permite que fique doente
        IsPermissionSick = true;
    }

    #endregion

    #region Transições de Estado de Saúde

    /// <summary>
    /// Método público para fazer o personagem ficar doente.
    /// Chamado pelo sistema de transmissão de doenças.
    /// </summary>
    /// <param name="sickness">Dados da doença contraída</param>
    public void ChangeHealthConditionSick(DataSickness sickness)
    {
        Sicken(sickness);
    }

    /// <summary>
    /// Processa a transição para estado DOENTE.
    ///
    /// Ações realizadas:
    /// 1. Define a doença contraída
    /// 2. Atualiza estado visual (sprite, animação)
    /// 3. Atualiza contadores na HUD
    /// 4. Agenda verificação diária de morte
    /// 5. Registra evento no Dashboard
    /// </summary>
    /// <param name="sickness">Dados da doença</param>
    private void Sicken(DataSickness sickness)
    {
        SicknessGot = sickness;
        Health = HealthCondition.Sick;

        // Atualiza visual
        statusBehaviour.SetSick();
        if (anim)
            anim.SetBool("isSick", true);

        // Atualiza HUD
        hud.UpdateSick();
        hud.UpdateHealthy();

        // Registra segundo em que ficou doente
        secGotSick = clock.CurrentSec;

        // Cria delegate para verificação de morte
        // IMPORTANTE: Criado aqui quando SicknessGot é garantidamente não-null
        checkDeadDelegate = () => CheckDeath(SicknessGot.lethality);
        timeEvent.AddActionEveryDay(ref checkDeadDelegate, secGotSick);

        // Registra no Dashboard
        dashboard.InsertData(TypeEventDashboard.Sick, clock.CurrentDay);
    }

    /// <summary>
    /// Cura o personagem, retornando ao estado SAUDÁVEL.
    ///
    /// Ações realizadas:
    /// 1. Remove a doença
    /// 2. Atualiza estado visual
    /// 3. Atualiza HUD
    /// 4. Remove verificação de morte
    /// 5. Registra cura no Dashboard
    /// </summary>
    public void Heal()
    {
        Debug.Log("Heal");

        SicknessGot = null;
        Health = HealthCondition.Healthy;

        // Atualiza visual
        statusBehaviour.SetHeal();
        if (anim)
            anim.SetBool("isSick", false);

        // Atualiza HUD
        hud.UpdateSick();
        hud.UpdateHealthy();

        // Remove verificação de morte
        timeEvent.RemoveActionEveryDay(ref checkDeadDelegate, secGotSick);

        // Registra no Dashboard
        dashboard.InsertData(TypeEventDashboard.Heal, clock.CurrentDay);
    }

    /// <summary>
    /// Coloca o personagem em tratamento médico.
    /// Suspende temporariamente as verificações de morte.
    /// Chamado pelo Hospital quando o paciente é internado.
    /// </summary>
    public void InMedicalTreatment()
    {
        hud.UpdateSick();
        hud.UpdateHealthy();

        // Remove verificação de morte enquanto em tratamento
        timeEvent.RemoveActionEveryDay(ref checkDeadDelegate, secGotSick);
    }

    /// <summary>
    /// Retorna do laboratório (diagnóstico completo).
    /// Reativa as verificações de morte.
    /// O personagem ainda está doente, apenas diagnosticado.
    /// </summary>
    public void RetunTheLaboratory()
    {
        hud.UpdateSick();
        hud.UpdateHealthy();

        // Reagenda verificação de morte no segundo atual
        secGotSick = clock.CurrentSec;
        timeEvent.AddActionEveryDay(ref checkDeadDelegate, secGotSick);
    }

    #endregion

    #region Sistema de Vacinação

    /// <summary>
    /// Define qual vacina o personagem vai tomar.
    /// Chamado antes de TakeVaccine().
    /// </summary>
    /// <param name="vaccine">Dados da vacina</param>
    public void SetVaccineWantTake(DataVaccine vaccine)
    {
        vaccineWantTake = vaccine;
    }

    /// <summary>
    /// Administra a vacina ao personagem.
    /// Adiciona à lista de vacinas tomadas, conferindo imunidade.
    ///
    /// NOTA: Não agenda verificações de morte.
    /// Vacinas são administradas após cura ou preventivamente.
    /// </summary>
    /// <returns>A vacina administrada</returns>
    public DataVaccine TakeVaccine()
    {
        VaccinesTaken.Add(vaccineWantTake);

        hud.UpdateSick();
        hud.UpdateHealthy();

        // Registra no Dashboard
        dashboard.InsertData(TypeEventDashboard.Heal, clock.CurrentDay);

        return vaccineWantTake;
    }

    #endregion

    #region Sistema de Morte

    /// <summary>
    /// Verificação diária de morte para personagens doentes.
    /// Executada pelo TimeEvent no mesmo segundo em que ficou doente.
    ///
    /// A chance de morte é baseada na letalidade da doença:
    /// - Random(0-100) <= letalidade → MORTE
    /// </summary>
    /// <param name="lethality">Letalidade da doença (0-100)</param>
    /// <returns>True se o personagem morreu</returns>
    private bool CheckDeath(int lethality)
    {
        // Verificação defensiva contra edge cases
        if (SicknessGot == null || Health != HealthCondition.Sick)
            return false;

        // Rola chance de morte
        if (UnityEngine.Random.Range(0, 100) <= lethality)
        {
            Die();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Processa a morte do personagem.
    ///
    /// Ações realizadas:
    /// 1. Muda estado para MORTO
    /// 2. Para o NavMeshAgent
    /// 3. Atualiza visual (animação de morte)
    /// 4. Atualiza HUD
    /// 5. Spawna zona de doença no corpo (pode infectar outros)
    /// 6. Cria alerta de morte
    /// 7. Remove verificação de morte
    /// 8. Registra no Dashboard
    /// </summary>
    private void Die()
    {
        Debug.Log("Die");

        Health = HealthCondition.Dead;

        // Para o movimento
        agent.isStopped = true;

        // Atualiza visual
        statusBehaviour.SetDead();
        if (anim)
            anim.SetBool("isDead", true);

        // Atualiza HUD
        hud.UpdateDead();
        hud.UpdateSick();

        // Spawna zona de doença no corpo morto
        // IMPORTANTE: Corpos podem infectar personagens próximos!
        diseaseSpawned = Instantiate(prefabDisease, transform);
        diseaseSpawned.transform.localPosition = Vector3.zero;
        diseaseSpawned.Sickness = SicknessGot;
        diseaseSpawned.gameObject.SetActive(true);

        // Cria alerta de morte na UI
        alertPanel.SpawnAlertDead(this);

        // Remove verificação de morte (não é mais necessária)
        timeEvent.RemoveActionEveryDay(ref checkDeadDelegate, secGotSick);

        // Registra no Dashboard
        dashboard.InsertData(TypeEventDashboard.Dead, clock.CurrentDay);
    }

    #endregion
}

#region Enums

/// <summary>
/// Estados de saúde possíveis para um personagem.
/// </summary>
public enum HealthCondition
{
    /// <summary>
    /// Saudável - pode se movimentar e ser infectado.
    /// </summary>
    Healthy,

    /// <summary>
    /// Doente - pode se movimentar, transmite doença, pode morrer.
    /// </summary>
    Sick,

    /// <summary>
    /// Morto - imóvel, corpo pode infectar outros.
    /// </summary>
    Dead
}

/// <summary>
/// Atividades que um personagem pode estar realizando.
/// </summary>
public enum Activity
{
    /// <summary>
    /// Nenhuma atividade especial - caminhando normalmente.
    /// </summary>
    None,

    /// <summary>
    /// Aguardando na fila do hospital.
    /// </summary>
    Queue
}

#endregion
