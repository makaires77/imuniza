using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Tab : MonoBehaviour
{
    [SerializeField] private Text textButton = null;
    private Graphs graphs;

    private Action showAction;

    public void SetData(string dataType, Graphs graphs, Action showAction)
    {
        textButton.text = dataType;
        this.graphs = graphs;

        this.showAction += showAction;
    }

    public void Show()
    {
        showAction?.Invoke();
        graphs.gameObject.SetActive(true);
    }

    public void Close()
    {
        graphs.gameObject.SetActive(false);
    }
}
