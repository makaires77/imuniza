using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Laboratory : MonoBehaviour, IRouteGlobal
{
    [SerializeField] private TextMeshProUGUI countPatientText = null;
    [SerializeField] private GameObject iconPatients = null;
    [Space]
    [SerializeField] private int dayTreat = 1;
    [SerializeField] private int costTotalTreat = 15;
    private int costPerDay = 0;

    private ClockBehaviour clock;
    private TimeEvent timeEvent;
    private AlertPanel alertPanel;

    private int numPatients = 0;
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

            if (iconPatients != null)
                iconPatients.SetActive(numPatients > 0);
        }
    }

    private void Awake()
    {
        alertPanel = FindObjectOfType<AlertPanel>();
        clock = FindObjectOfType<ClockBehaviour>();
        timeEvent = FindObjectOfType<TimeEvent>();
    }

    private void Start()
    {
        costPerDay = costTotalTreat / dayTreat;
    }

    public void TreatPatient(Transform patient)
    {
        NumPatients++;

        CharacterStatus characterPatient = patient.GetComponent<CharacterStatus>();
        BodyIA bodyIA = patient.GetComponent<BodyIA>();

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

        // Esconder paciente durante exame
        patientGO.SetActive(false);

        // Pausar verificações de morte durante tratamento
        characterPatient.InMedicalTreatment();

        // Agendar término do exame
        int dayFinishTreat = clock.CurrentDay + dayTreat;
        timeEvent.AddActionInSpecificSecAndDay(
            () => Treat(patientGO, characterPatient),
            clock.CurrentSec,
            dayFinishTreat
        );

        // Cobrar custo
        timeEvent.AddActionInSpecificSecAndDay(
            () => Cost(),
            clock.CurrentSec,
            clock.CurrentDay + 1
        );
    }

    private bool Treat(GameObject patient, CharacterStatus status)
    {
        NumPatients--;
        patient.SetActive(true);

        // Marcar como diagnosticado
        status.IsDiagnosed = true;

        // Retornar paciente (ainda doente, mas diagnosticado)
        status.RetunTheLaboratory();

        BodyIA bodyIA = patient.GetComponentInChildren<BodyIA>();
        if (bodyIA != null)
            alertPanel.SpawnAlertDiagnosis(bodyIA.transform);

        return true;
    }

    private bool Cost()
    {
        MoneyManager.CurrentMoney -= costPerDay;
        return true;
    }
}
