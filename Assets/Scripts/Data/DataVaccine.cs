using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataVaccine", menuName = "Data/Vaccine")]
public class DataVaccine : ScriptableObject
{
    public new string name;
    public Sprite icon;
    public DataSickness[] prevents;
}
