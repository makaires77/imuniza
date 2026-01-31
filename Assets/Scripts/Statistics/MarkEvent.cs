using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkEvent : MonoBehaviour
{
    [SerializeField] private Text markNumberText = null;

    public int NumberEvents
    {
        get
        {
            return int.Parse(markNumberText.text);
        }
        set
        {
            markNumberText.text = value.ToString();
        }
    }
}
