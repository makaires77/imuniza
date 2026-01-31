using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterStatus : MonoBehaviour
{
    public string               FullName        { get; private set; }
    public int                  Age             { get; private set; }
    public HealthCondition      Health          { get; private set; }
    public Activity             Activity        { get; set; }
    public int                  ImuneSystem     { get; private set; }
    public List<DataVaccine>    VaccinesTaken   { get; private set; }
    public DataSickness         SicknessGot     { get; private set; }
    public bool                 IsDiagnosed     { get; set; }
    public bool                 IsPermissionSick { get; private set; } = false;

    [SerializeField] private DataName dataNames = null;
    [SerializeField] private DiseaseHumanPoint prefabDisease = null;
    private DiseaseHumanPoint diseaseSpawned = null;
    public bool IsCallIML { get; set; }

    private HUD hud = null;
    private TimeEvent timeEvent = null;
    private ClockBehaviour clock = null;
    private AlertPanel alertPanel = null;
    private static DebugPanel debugPanel = null;
    private StatusBehaviour statusBehaviour;
    private Animator anim;
    private NavMeshAgent agent;

    private int secGotSick = -1;

    private TimeEvent.ActionClockDelegate checkDeadDelegate;
    private DataVaccine vaccineWantTake = null;
    private Dashboard dashboard;

    private static int numberAventsSicknessInday = 0;
    public static int NumberAventsSicknessInday {
        get
        {
            return numberAventsSicknessInday;
        }
        set
        {
            numberAventsSicknessInday = value;
        }
    }

    private void Awake()
    {
        hud = FindObjectOfType<HUD>();
        timeEvent = FindObjectOfType<TimeEvent>();
        clock = FindObjectOfType<ClockBehaviour>();
        alertPanel = FindObjectOfType<AlertPanel>();
        debugPanel = FindObjectOfType<DebugPanel>();
        dashboard = FindObjectOfType<Dashboard>();

        statusBehaviour = GetComponentInChildren<StatusBehaviour>();
        agent = GetComponentInChildren<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        VaccinesTaken = new List<DataVaccine>();
        IsCallIML = false;
        Activity = Activity.None;

        // REMOVIDO: checkDeadDelegate - agora criado em Sicken() quando SicknessGot é garantidamente não-null

        SetData();
    }

    public void ChangeHealthConditionSick(DataSickness sickness)
    {
        Sicken(sickness);
    }

    private bool CheckDeath(int lethality)
    {
        // Verificação defensiva - não deve acontecer mas protege contra edge cases
        if (SicknessGot == null || Health != HealthCondition.Sick)
            return false;

        if (UnityEngine.Random.Range(0, 100) <= lethality)
        {
            Die();
            return true;
        }

        return false;
    }

    private void SetData()
    {
        FullName = dataNames.GetRandomFullName();

        Age = UnityEngine.Random.Range(10, 60);
        ImuneSystem = UnityEngine.Random.Range(40, 80);

        Health = HealthCondition.Healthy;

        hud.UpdateHealthy();
        hud.UpdateSick();

        IsPermissionSick = true;
    }

    public void SetVaccineWantTake(DataVaccine vaccine)
    {
        vaccineWantTake = vaccine;
    }

    private void Sicken(DataSickness sickness)
    {
        SicknessGot = sickness;
        Health = HealthCondition.Sick;

        statusBehaviour.SetSick();
        if(anim)
            anim.SetBool("isSick", true);

        hud.UpdateSick();
        hud.UpdateHealthy();

        secGotSick = clock.CurrentSec;

        // Criar delegate aqui quando SicknessGot é garantidamente não-null
        checkDeadDelegate = () => CheckDeath(SicknessGot.lethality);
        timeEvent.AddActionEveryDay(ref checkDeadDelegate, secGotSick);

        dashboard.InsertData(TypeEventDashboard.Sick, clock.CurrentDay);
    }

    public void Heal()
    {
        Debug.Log("Heal");
        SicknessGot = null;
        Health = HealthCondition.Healthy;

        statusBehaviour.SetHeal();
        if (anim)
            anim.SetBool("isSick", false);

        hud.UpdateSick();
        hud.UpdateHealthy();

        timeEvent.RemoveActionEveryDay(ref checkDeadDelegate, secGotSick);

        dashboard.InsertData(TypeEventDashboard.Heal, clock.CurrentDay);
    }

    public void InMedicalTreatment()
    {
        hud.UpdateSick();
        hud.UpdateHealthy();

        timeEvent.RemoveActionEveryDay(ref checkDeadDelegate, secGotSick);
    }

    public void RetunTheLaboratory()
    {
        hud.UpdateSick();
        hud.UpdateHealthy();

        secGotSick = clock.CurrentSec;
        timeEvent.AddActionEveryDay(ref checkDeadDelegate, secGotSick);
    }

    public DataVaccine TakeVaccine()
    {
        VaccinesTaken.Add(vaccineWantTake);

        hud.UpdateSick();
        hud.UpdateHealthy();

        // REMOVIDO: Não adicionar death check para vacinação
        // Death checks são gerenciados apenas por Sicken() e removidos por Heal()

        dashboard.InsertData(TypeEventDashboard.Heal, clock.CurrentDay);

        return vaccineWantTake;
    }

    private void Die()
    {
        Debug.Log("Die");
        Health = HealthCondition.Dead;

        agent.isStopped = true;

        statusBehaviour.SetDead();
        if (anim)
            anim.SetBool("isDead", true);

        hud.UpdateDead();
        hud.UpdateSick();

        diseaseSpawned = Instantiate(prefabDisease, transform);
        diseaseSpawned.transform.localPosition = Vector3.zero;
        diseaseSpawned.Sickness = SicknessGot;
        diseaseSpawned.gameObject.SetActive(true);

        alertPanel.SpawnAlertDead(this);

        timeEvent.RemoveActionEveryDay(ref checkDeadDelegate, secGotSick);

        dashboard.InsertData(TypeEventDashboard.Dead, clock.CurrentDay);
    }
}

public enum HealthCondition
{
    Healthy,
    Sick,
    Dead
}

public enum Activity
{
    None,
    Queue
}