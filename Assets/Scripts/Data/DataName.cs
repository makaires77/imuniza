using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataName", menuName = "Data/Name", order = 1)]
public class DataName : ScriptableObject
{
    public string[] names;
    public string[] lastNames;

    public string GetRandomFullName()
    {
        string fullName = string.Empty;
        int indexRandom = UnityEngine.Random.Range(0, names.Length - 1);
        fullName = names[indexRandom];
        fullName += " ";

        indexRandom = UnityEngine.Random.Range(0, lastNames.Length - 1);
        fullName += lastNames[indexRandom];

        return fullName;
    }
}
