using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(LineRenderer))]
public class NavigationDebugger : MonoBehaviour
{
    private NavMeshAgent agent;
    private LineRenderer lineDebug;

    public bool ActiveLineDebug { get; set; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        lineDebug = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        ActiveLineDebug = true;
    }

    public void Update()
    {
        if(ActiveLineDebug && agent.hasPath)
        {
            lineDebug.positionCount = agent.path.corners.Length;
            lineDebug.SetPositions(agent.path.corners);
            lineDebug.enabled = true;
        }
        else
            lineDebug.enabled = false;
    }
}
