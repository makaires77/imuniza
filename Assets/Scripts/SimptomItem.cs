using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimptomItem : MonoBehaviour
{
    [SerializeField] private Image iconSimptom = null;

    public DataSymptoms Data { get; private set; }

    public void SetData(DataSymptoms data)
    {
        Data = data;
        iconSimptom.sprite = Data.icon;
        iconSimptom.enabled = false;
    }

    public void HaveSimptom()
    {
        iconSimptom.enabled = true;
    }
    public void ResetSimptom()
    {
        iconSimptom.enabled = false;
    }
}
