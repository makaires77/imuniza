using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUp : MonoBehaviour
{
    [SerializeField] private Text descriptionText = null;
    [SerializeField] private Button leftButton = null;
    [SerializeField] private Button rightButton = null;

    public void ShowQuestionYesNo(string msg, Action yesAction, Action noAction = null)
    {
        SetBehaviour(msg, 
            () => { yesAction(); gameObject.SetActive(false); }, "Sim",
            () => { if (noAction != null) noAction(); gameObject.SetActive(false); }, "Não");
    }

    public void ShowAlert(string msg, Action okAction, Action goAction)
    {
        SetBehaviour(msg, 
            () => { okAction(); gameObject.SetActive(false); }, "OK",
            () => { goAction(); gameObject.SetActive(false); }, "IR");
    }

    private void SetBehaviour(string msg, Action leftAction, string leftButtonText, Action rightAction, string rightButtonText)
    {
        descriptionText.text = msg;

        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();

        leftButton.GetComponentInChildren<Text>().text = leftButtonText;
        rightButton.GetComponentInChildren<Text>().text = rightButtonText;

        leftButton.onClick.AddListener(() => { if (leftAction != null) leftAction(); });
        rightButton.onClick.AddListener(() => { if (rightAction != null) rightAction(); });

        gameObject.SetActive(true);
    }
}
