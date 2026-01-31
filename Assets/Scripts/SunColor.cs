using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunColor : MonoBehaviour
{
    [SerializeField] private Light lightSun = null;
    [SerializeField] private Material[] materialInGame = null;
    [SerializeField] private Material[] materialEmission = null;
    [SerializeField] private Color colorEmissionLigth = Color.white;
    [SerializeField] private Gradient colorGradient = null;
    [Space]
    [SerializeField] private ClockBehaviour clockBehaviour = null;

    void Update()
    {
        float percentageDay = (float)clockBehaviour.CurrentSec / clockBehaviour.TotalDaySec;

        lightSun.color = colorGradient.Evaluate(percentageDay);

        for (int i = 0; i < materialInGame.Length; i++)
            materialInGame[i].SetColor("_MainColor", colorGradient.Evaluate(percentageDay));

        if (colorGradient.Evaluate(percentageDay) == colorGradient.colorKeys[0].color)
        {
            for (int i = 0; i < materialInGame.Length; i++)
                materialInGame[i].SetColor("_EmissionColor", colorEmissionLigth);
        }
        else if(materialEmission[0].GetColor("_EmissionColor") == colorEmissionLigth)
        {
            for (int i = 0; i < materialInGame.Length; i++)
                materialInGame[i].SetColor("_EmissionColor", Color.black);
        }
    }

    private void OnApplicationQuit()
    {
        float percentageDay = 0.5f;

        lightSun.color = colorGradient.Evaluate(percentageDay);

        for (int i = 0; i < materialInGame.Length; i++)
            materialInGame[i].SetColor("_MainColor", colorGradient.Evaluate(percentageDay));

        if (colorGradient.Evaluate(percentageDay) == colorGradient.colorKeys[0].color)
        {
            for (int i = 0; i < materialInGame.Length; i++)
                materialInGame[i].SetColor("_EmissionColor", colorEmissionLigth);
        }
        else if (materialEmission[0].GetColor("_EmissionColor") == colorEmissionLigth)
        {
            for (int i = 0; i < materialInGame.Length; i++)
                materialInGame[i].SetColor("_EmissionColor", Color.black);
        }
    }
}
