using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AlertAffectedButton : MonoBehaviour
{
    [SerializeField] private AlertEffectedType alertType = AlertEffectedType.FocusAlert;
    [SerializeField] private Image icon = null;
    [SerializeField] private PopUp popup = null;

    private CameraBehaviour cameraBehaviour;
    private ActionPanel actionPanel;

    public CharacterStatus AffectedTarget { private get; set; }
    public CharacterStatus TransmitterCharacter { private get; set; }

    private Dictionary<AlertEffectedType, Action> popUpType = new Dictionary<AlertEffectedType, Action>();

    private void Awake()
    {
        cameraBehaviour = FindObjectOfType<CameraBehaviour>();
        actionPanel = FindObjectOfType<ActionPanel>();
    }

    private void Start()
    {
        popUpType.Add(AlertEffectedType.FocusAlert, () => PopUpFocus());
        popUpType.Add(AlertEffectedType.InterpersonalAlert, () => PopUpInterpersonal());
        popUpType.Add(AlertEffectedType.DeadAlert, () => PopUpDead());
    }

    public void UpdateInfo()
    {
        icon.sprite = AffectedTarget.SicknessGot.icon;
    }

    public void OpenPopUp()
    {
        popUpType[alertType]();
    }

    private void PopUpFocus()
    {
        string message = AffectedTarget.FullName + " ficou doente";
        popup.ShowAlert(message, () => DestroyImmediate(gameObject), () => FindAffected());
    }

    private void PopUpInterpersonal()
    {
        string message = AffectedTarget.FullName + " contraiu doença de " + TransmitterCharacter.FullName;
        popup.ShowAlert(message, () => DestroyImmediate(gameObject), () => FindAffected());
    }

    private void PopUpDead()
    {
        string msgIsDiagnosed = " morreu da doença ";
        string msgNotDiagnosed = " morreu sem um diagnóstico";
        string message = AffectedTarget.IsDiagnosed ? AffectedTarget.FullName + msgIsDiagnosed + AffectedTarget .SicknessGot.name :
            AffectedTarget.FullName + msgNotDiagnosed;
        popup.ShowAlert(message, () => DestroyImmediate(gameObject), () => FindAffected());
    }

    private void FindAffected()
    {
        actionPanel.Hide();
        cameraBehaviour.SetTarget(AffectedTarget.transform);
        actionPanel.Show(AffectedTarget.transform, false);

        DestroyImmediate(gameObject);
    }

    public enum AlertEffectedType
    {
        FocusAlert,
        InterpersonalAlert,
        DeadAlert
    }
}
