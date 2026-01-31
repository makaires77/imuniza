using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IMLItem : MonoBehaviour
{
    [SerializeField] private Text nameTxt;

    private ActionPanel statusPanel;

    public CharacterStatus CharacterDead { get; set; }

    private void Awake()
    {
        statusPanel = FindObjectOfType<ActionPanel>();
    }

    public void UpdateDisplay()
    {
        nameTxt.text = CharacterDead.FullName;
    }

    public void ShowInfo()
    {
        statusPanel.Show(CharacterDead.transform, false);
    }
}
