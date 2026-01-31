using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashboard : MonoBehaviour
{
    [SerializeField] private Tab tab = null;
    [SerializeField] private Transform contentTabs = null;

    [SerializeField] private Graphs graphs = null;
    [SerializeField] private Transform contentGraphs = null;

    [Space]
    [SerializeField] private GameObject panel = null;

    private Dictionary<string, Tab> tabsCollection = new Dictionary<string, Tab>();
    private Dictionary<string, Graphs> graphsCollection = new Dictionary<string, Graphs>();

    public void ShowDashboard()
    {
        foreach (var tab in tabsCollection)
        {
            tab.Value.Show();
            break;
        }

        panel.SetActive(true);
    }

    public void InsertData(string typeEvent, int day)
    {
        if(graphsCollection.ContainsKey(typeEvent))
        {
            graphsCollection[typeEvent].AddValue(day);
        }
        else
        {
            Tab tabNew = Instantiate(tab, contentTabs);
            tabsCollection.Add(typeEvent, tabNew);

            Graphs graphsNew = Instantiate(graphs, contentGraphs);
            graphsCollection.Add(typeEvent, graphsNew);

            tabNew.SetData(typeEvent, graphsNew, () => CloseAllTabs());
            tabNew.gameObject.SetActive(true);

            graphsNew.AddValue(day);
        }
    }

    private void CloseAllTabs()
    {
        foreach (var tab in tabsCollection)
            tab.Value.Close();
    }
}

public struct TypeEventDashboard
{
    public const string Healthy = "Saudávels";
    public const string Heal = "Curado";
    public const string Sick = "Doentes";
    public const string Dead = "Mortos";
}