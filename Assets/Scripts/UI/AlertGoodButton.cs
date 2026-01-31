using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertGoodButton : MonoBehaviour
{
    [SerializeField] private AlertGoodType alertType = AlertGoodType.Diagnosis;
    [SerializeField] private PopUp popup = null;

    private CameraBehaviour cameraBehaviour;
    private ActionPanel actionPanel;

    public CharacterStatus AffectedTarget { private get; set; }
    public DataVaccine VaccineTake { private get; set; }

    private Dictionary<AlertGoodType, Action> popUpType = new Dictionary<AlertGoodType, Action>();


    private void Awake()
    {
        cameraBehaviour = FindObjectOfType<CameraBehaviour>();
        actionPanel = FindObjectOfType<ActionPanel>();
    }

    private void Start()
    {
        popUpType.Add(AlertGoodType.Diagnosis, () => PopUpDiagnosis());
        popUpType.Add(AlertGoodType.Hospital, () => PopUpHospital());
        popUpType.Add(AlertGoodType.Health_Center, () => PopUpHealthCenter());
    }

    public void OpenPopUp()
    {
        popUpType[alertType]();
    }

    private void PopUpDiagnosis()
    {
        string message = AffectedTarget.FullName + " foi diagnosticado " + ((AffectedTarget.SicknessGot == null) ? 
            "sem doença" :  
            "com " + AffectedTarget.SicknessGot.name + " e está em tratamento");
        popup.ShowAlert(message, () => DestroyImmediate(gameObject), () => FindAffected());
    }

    private void PopUpHospital()
    {
        string message = AffectedTarget.FullName + " está finalmente curado(a)";
        popup.ShowAlert(message, () => DestroyImmediate(gameObject), () => FindAffected());
    }

    private void PopUpHealthCenter()
    {
        string message = AffectedTarget.FullName + " se vacinou e agora está imune a " + VaccineTake.prevents[0].name;
        popup.ShowAlert(message, () => DestroyImmediate(gameObject), () => FindAffected());
    }

    private void FindAffected()
    {
        actionPanel.Hide();

        cameraBehaviour.SetTarget(AffectedTarget.transform);

        DestroyImmediate(gameObject);
    }

    public enum AlertGoodType
    {
        Diagnosis,
        Hospital,
        Health_Center
    }
}
