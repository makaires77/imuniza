using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BedItem : MonoBehaviour
{
    [SerializeField] private Text textDays = null;
    [SerializeField] private Image characterImage = null;
    [SerializeField] private Image sickIcon = null;
    [SerializeField] private GameObject iconObj = null;

    [Space]
    [SerializeField] private Sprite freeBedImage;
    [SerializeField] private Sprite busyBedImage;

    public CharacterStatus Character { get; private set; }
    public int DaysTratment { get; set; }

    public bool IsBusyBed { get; private set; }

    private ActionPanel statusPanel;

    private void Awake()
    {
        statusPanel = FindObjectOfType<ActionPanel>();
    }

    public void ShowStatusPatient()
    {
        if(IsBusyBed)
            statusPanel.Show(Character.transform, false);
    }


    public void UpdateDisplay()
    {
        Debug.Log("BED busy: " + IsBusyBed);
        if(IsBusyBed)
        {
            characterImage.sprite = busyBedImage;
            iconObj.SetActive(true);

            SetDayMissingConclusion(DaysTratment);
        }
        else
        {
            characterImage.sprite = freeBedImage;
            iconObj.SetActive(false);

            textDays.text = string.Empty;
        }
    }

    public void Occupy(CharacterStatus character, int days)
    {
        IsBusyBed = true;
        DaysTratment = days;

        Character = character;

        characterImage.sprite = Character.SicknessGot.icon;
    }

    public void Vacate()
    {
        IsBusyBed = false;
    }

    public void SetDayMissingConclusion(int daysMissing)
    {
        DaysTratment = daysMissing;
        string textOutput = daysMissing.ToString();
        textOutput += (daysMissing == 1) ? " dia" : " dias";

        textDays.text = textOutput;
    }
}
