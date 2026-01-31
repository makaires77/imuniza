using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColumnDay : MonoBehaviour
{
    [SerializeField] private Text dayText = null;
    [SerializeField] private Slider sliderBar = null;

    public int ValueInDay { get; set; }
    public int Day
    {
        get
        {
            return int.Parse(dayText.text);
        }
        set
        {
            dayText.text = value.ToString();
        }
    }

    public void SetHeightBar(int maxValue)
    {
        sliderBar.value = (float)ValueInDay / (float)maxValue;
    }
}
