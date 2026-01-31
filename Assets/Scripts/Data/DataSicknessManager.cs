using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataSicknessManager", menuName = "Data/Sickness_Manager")]
public class DataSicknessManager : ScriptableObject
{
    public DataSickness[] sickness;
}
