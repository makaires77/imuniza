using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IA : MonoBehaviour
{
    private NavMeshAgent body;
    private CharacterStatus characterStatus;
    private Animator animator;

    private GlobalRoute route;
    private PointRoute currentPoint;

    private AlertPanel alertPanel;
    private ActionPanel actionpanel;

    private bool followRandomPath = true;
    IRouteGlobal routeTarget;
	
	private float speedDefault = 4;

    private void Awake()
    {
        body = transform.GetComponentInChildren<NavMeshAgent>(true);
        animator = GetComponentInChildren<Animator>(true);
        characterStatus = transform.GetComponentInChildren<CharacterStatus>(true);

        route = FindObjectOfType<GlobalRoute>();
        actionpanel = FindObjectOfType<ActionPanel>();
        alertPanel = FindObjectOfType<AlertPanel>();
    }

    private void Start()
    {
		speedDefault = body.speed;
		
        currentPoint = route.GetPointCloserSidewalk(transform.position);

        followRandomPath = true;
        body.Warp(currentPoint.transform.position);
        NextWaypointRandomPath();
    }

    private void Update()
    {
        if (!body.pathPending && body.remainingDistance < 1 && !body.isStopped && characterStatus.Activity == Activity.None)
            NextWaypointRandomPath();
    }

    private void NextWaypointRandomPath()
    {
        if (!followRandomPath)
        {
            followRandomPath = true;
            currentPoint = route.GetPointCloserSidewalk(transform.position);
            routeTarget.TreatPatient(body.transform);
        }
        else
        {
            currentPoint = currentPoint.GetRandomPoint();
            body.SetDestination(currentPoint.transform.position);
        }
    }

    public void NextWaypointTarget(Transform target)
    {
        body.Warp(body.transform.position);
        Debug.Log($"Set Waypoint for {gameObject.name}");
        body.SetDestination(target.position);

        routeTarget = target.GetComponent<IRouteGlobal>();

        followRandomPath = false;
    }
	
	public void SpeedUpdate(float speedNew)
	{
		body.speed = speedDefault * speedNew;
        animator.SetFloat("SpeedAnimation", speedNew);
    }
}
