using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Hospital : MonoBehaviour, IRouteGlobal
{
    [SerializeField] private GameObject iconPatitents = null;
    [SerializeField] private TextMeshProUGUI countPatientText = null;
    [SerializeField] private HospitalPanel panel = null;
    [SerializeField] private ActionPanel statusPanel = null;
    [Space]
    [SerializeField] private int dayTreat = 4;
    [SerializeField] private int costTotalTreat = 80;
    [Space]
    [SerializeField] private Transform queueStart = null;
    [SerializeField] private DataVaccineManager vaccineManager = null;

    private Vector3 posEndQueue = Vector3.zero;
    private int costPerDay = 0;
    private int numPatients = 0;
    public int NumberBed { get; set; }
    public int NumberMaxBed { get { return 8; } }

    private List<CharacterStatus> charactersInQueue = new List<CharacterStatus>();
    private ClockBehaviour clock;
    private TimeEvent timeEvent;
    private AlertPanel alertPanel;

    private void Awake()
    {
        clock = FindObjectOfType<ClockBehaviour>();
        timeEvent = FindObjectOfType<TimeEvent>();
        alertPanel = FindObjectOfType<AlertPanel>();
    }

    private void Start()
    {
        costPerDay = costTotalTreat / dayTreat;
        posEndQueue = queueStart.position + (Vector3.back * 2);
    }

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

    public void TreatPatient(Transform patient)
    {
        CharacterStatus characterPatient = patient.GetComponent<CharacterStatus>();

        if (NumberBed > NumPatients)
        {
            NumPatients++;
            iconPatitents.SetActive(true);

            GameObject patienteTreat = patient.GetComponent<BodyIA>().IA.gameObject;
            patienteTreat.SetActive(false);

            characterPatient.InMedicalTreatment();
            BedItem bed = panel.OccupyBed(characterPatient, dayTreat);

            int dayFinishTreat = clock.CurrentDay + dayTreat;
            timeEvent.AddActionInSpecificSecAndDay(() => Treat(patienteTreat, bed), clock.CurrentSec, dayFinishTreat);

            int daysCostMoney = clock.CurrentDay + 1;

            timeEvent.AddActionInSpecificSecAndDay(() => Diagnose(patient), clock.CurrentSec, daysCostMoney);

            for (int i = daysCostMoney; i < dayFinishTreat; i++)
                timeEvent.AddActionInSpecificSecAndDay(() => Cost(bed), clock.CurrentSec, i);

            panel.OpenPanel();
            statusPanel.Show(patient.transform, false);
        }
        else
            AddPatientInQueue(characterPatient);
    }
	
	public void TreatPatientInQueue()
	{
		if (charactersInQueue.Count > 0) {
            TreatPatient(charactersInQueue[0].transform);
            RemovePatientInQueue(charactersInQueue[0]);
        }
	}

    private bool Treat(GameObject patient, BedItem bed)
    {
        CharacterStatus characterPatient = patient.GetComponentInChildren<CharacterStatus>();
        characterPatient.Activity = Activity.None;
        patient.SetActive(true);

        TakeVaccine(characterPatient);
        characterPatient.Heal();

        --NumPatients;
        bed.Vacate();

        alertPanel.SpawnAlertHospital(characterPatient.GetComponent<BodyIA>().transform);

        if (NumPatients == 0)
            iconPatitents.SetActive(false);

		TreatPatientInQueue();

        return true;
    }

    public void AddPatientInQueue(CharacterStatus character)
    {
        Debug.Log("Add in queue XD");
        character.Activity = Activity.Queue;
        posEndQueue += (Vector3.back * 2);

        character.GetComponent<NavMeshAgent>().SetDestination(posEndQueue);

        charactersInQueue.Add(character);
    }

    private void RemovePatientInQueue(CharacterStatus character)
    {
        charactersInQueue.Remove(character);
    }

    private bool Cost(BedItem bed)
    {
        MoneyManager.CurrentMoney -= costPerDay;
        bed.DaysTratment -= 1;
        return true;
    }

    private bool Diagnose(Transform patient)
    {
        CharacterStatus status = patient.GetComponentInChildren<CharacterStatus>();
        status.IsDiagnosed = true;

        alertPanel.SpawnAlertDiagnosis(patient.GetComponentInChildren<BodyIA>().transform);

        return true;
    }

    private bool TakeVaccine(CharacterStatus patient)
    {
        if (patient.SicknessGot != null) {
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
	
	private void OnMouseDown()
	{
		panel.OpenPanel();
	}
}
