using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEvent : MonoBehaviour
{
    public delegate bool ActionClockDelegate();
    public delegate void ActionEndDay();
    public delegate void ActionPerSecond();

    public Dictionary<int, ActionClockDelegate> actionInSecEveryDay = new Dictionary<int, ActionClockDelegate>();
    public Dictionary<int, Dictionary<int, ActionClockDelegate>> actionInDaySecundSpecific = new Dictionary<int, Dictionary<int, ActionClockDelegate>>();

    public ActionPerSecond actionPerSecond;
    public ActionEndDay actionEndDay;

    public void AddActionPerSec(ref ActionPerSecond action)
    {
        actionPerSecond += action;
    }

    public void RemoveActionPerSec(ref ActionPerSecond action)
    {
        actionPerSecond -= action;
    }

    public void AddActionEveryDay(ref ActionClockDelegate action, int sec)
    {
        if (actionInSecEveryDay.ContainsKey(sec))
            actionInSecEveryDay[sec] += action;
        else
            actionInSecEveryDay.Add(sec, action);
    }

    public void RemoveActionEveryDay(ref ActionClockDelegate action, int sec)
    {
        if (actionInSecEveryDay.ContainsKey(sec))
        {
            actionInSecEveryDay[sec] -= action;

            if (actionInSecEveryDay[sec] == null)
                actionInSecEveryDay.Remove(sec);
        }
    }

    public void AddActionInSpecificSec(ref ActionClockDelegate action, int sec)
    {
        if (actionInSecEveryDay.ContainsKey(sec))
            actionInSecEveryDay[sec] += action;
        else
            actionInSecEveryDay.Add(sec, action);
    }

    public void RemovedActionInSpecificSec(ref ActionClockDelegate action, int sec)
    {
        if (actionInSecEveryDay.ContainsKey(sec))
            actionInSecEveryDay[sec] -= action;
    }

    public void AddActionInSpecificSecAndDay(ActionClockDelegate action, int sec, int day)
    {
        if (actionInDaySecundSpecific.ContainsKey(day))
        {
            if (actionInDaySecundSpecific[day] != null && actionInDaySecundSpecific[day].ContainsKey(sec))
                actionInDaySecundSpecific[day][sec] += action;
            else if (actionInDaySecundSpecific[day] == null)
            {
                actionInDaySecundSpecific[day] = new Dictionary<int, ActionClockDelegate>();
                actionInDaySecundSpecific[day].Add(sec, action);
            }
            else
                actionInDaySecundSpecific[day].Add(sec, action);
        }
        else
        {
            actionInDaySecundSpecific.Add(day, new Dictionary<int, ActionClockDelegate>());
            actionInDaySecundSpecific[day].Add(sec, action);
        }
    }

    /// <summary>
    /// Remove todos os eventos agendados para dias anteriores ao dia especificado.
    /// Previne memory leak de eventos passados acumulados.
    /// </summary>
    public void CleanupPastDays(int currentDay)
    {
        List<int> daysToRemove = new List<int>();

        foreach (int day in actionInDaySecundSpecific.Keys)
        {
            if (day < currentDay)
            {
                daysToRemove.Add(day);
            }
        }

        foreach (int day in daysToRemove)
        {
            actionInDaySecundSpecific.Remove(day);
        }
    }

    /// <summary>
    /// Limpa ações de um segundo específico após execução.
    /// </summary>
    public void ClearDaySecondActions(int day, int second)
    {
        if (actionInDaySecundSpecific.TryGetValue(day, out var secDict))
        {
            secDict.Remove(second);

            if (secDict.Count == 0)
            {
                actionInDaySecundSpecific.Remove(day);
            }
        }
    }
}
