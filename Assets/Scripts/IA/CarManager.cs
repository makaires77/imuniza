using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    [SerializeField] CarIA[] iaCars;

    private void Start()
    {
        for (int i = 0; i < iaCars.Length; i++)
            iaCars[i].carEngine.Setpath(iaCars[i].carPath.GeneratePath());
    }
}

[System.Serializable]
public struct CarIA
{
    public CarEngine carEngine;
    public CarPath carPath;
}
