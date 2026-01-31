using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VaccineItem : MonoBehaviour
{
    [SerializeField] private Image iconVaccine = null;

    public DataSickness Data { get; private set; }

    public void SetData(DataSickness data)
    {
        Data = data;
        iconVaccine.sprite = Data.icon;
        iconVaccine.enabled = false;
    }

    public void HaveVaccine()
    {
        iconVaccine.enabled = true;
    }
    public void ResetVaccine()
    {
        iconVaccine.enabled = false;
    }
}
