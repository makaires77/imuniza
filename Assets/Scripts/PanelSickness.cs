using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelSickness : MonoBehaviour
{
    [SerializeField] private GameObject panel = null;
    [Space]
    [SerializeField] private Text textName = null;
    [SerializeField] private Text textTransmissibility = null;
    [SerializeField] private Text textLethality = null;

    public void Show(DataSickness sickness)
    {
        textName.text = sickness.name;
        textTransmissibility.text = sickness.transmissibility.ToString() + "%";
        textLethality.text = sickness.lethality.ToString() + "%";
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
