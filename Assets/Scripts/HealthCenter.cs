using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthCenter : MonoBehaviour, IRouteGlobal
{
    [SerializeField] private GameObject iconPatitents = null;
    [SerializeField] private TextMeshProUGUI countPatientText = null;
    [Space]
    [SerializeField] private int dayTreat = 1;
    [SerializeField] private int costTotalTreat = 10;
    private int costPerDay = 0;

    private TimeEvent timeEvent;
    private ClockBehaviour clock;
    private AlertPanel alertPanel;

    private int numPatients = 0;


    private void Awake()
    {
        timeEvent = FindObjectOfType<TimeEvent>();
        clock = FindObjectOfType<ClockBehaviour>();
        alertPanel = FindObjectOfType<AlertPanel>();
    }

    private void Start()
    {
        costPerDay = costTotalTreat / dayTreat;
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
        NumPatients++;
        iconPatitents.SetActive(true);


        GameObject patienteTreat = patient.GetComponentInParent<IA>().gameObject;
        patienteTreat.SetActive(false);

        patient.GetComponent<CharacterStatus>().InMedicalTreatment();

        int day = clock.CurrentDay + dayTreat;
        timeEvent.AddActionInSpecificSecAndDay(() => Treat(patienteTreat), clock.CurrentSec, day);

        int daysCostMoney = clock.CurrentDay + 1;
        for (int i = 0; i < dayTreat; i++)
        {
            timeEvent.AddActionInSpecificSecAndDay(() => Cost(), clock.CurrentSec, daysCostMoney);
            daysCostMoney++;
        }
    }

    private bool Treat(GameObject patient)
    {
        --NumPatients;
        patient.SetActive(true);
        DataVaccine vaccine = patient.GetComponentInChildren<CharacterStatus>().TakeVaccine();

        alertPanel.SpawnAlertHelthCenter(patient.GetComponentInChildren<BodyIA>().transform, vaccine);

        if (NumPatients == 0)
            iconPatitents.SetActive(false);

        switch (patient.GetComponentInChildren<CharacterStatus>().Health)
        {
            case HealthCondition.Healthy:
                patient.GetComponentInChildren<CharacterStatus>().GetComponentInChildren<StatusBehaviour>().SetHeal();
                break;
            case HealthCondition.Sick:
                patient.GetComponentInChildren<CharacterStatus>().GetComponentInChildren<StatusBehaviour>().SetSick();
                break;
        }

        return true;
    }

    private bool Cost()
    {
        MoneyManager.CurrentMoney -= costPerDay;
        return true;
    }
}
