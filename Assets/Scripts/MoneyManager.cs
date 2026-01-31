using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MoneyManager : MonoBehaviour
{
    private static Text display;
    private static TimeEvent timeEvent;

    private static int againWithHealthy = 0;

    private void Awake()
    {
        display = GetComponentInChildren<Text>();
        timeEvent = FindObjectOfType<TimeEvent>();
    }

    private void Start()
    {
        CurrentMoney = 400;

        timeEvent.actionEndDay += () => UpdateMoney();
    }

    private static int currentMoney;
    public static int CurrentMoney
    {
        get
        {
            return currentMoney;
        }
        set
        {
            currentMoney = value;
            display.text = "R$ " + currentMoney.ToString();
        }
    }

    public static int UpdateAgainWithHealthy(int numberHealthy)
    {
        int againValue = numberHealthy * 10;
        againWithHealthy = againValue;

        return againValue;
    }

    private static void UpdateMoney()
    {
        CurrentMoney += againWithHealthy;
    }
}
