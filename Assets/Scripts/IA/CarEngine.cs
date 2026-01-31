using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    [SerializeField] private float maxSteerAngle = 45f;
    [SerializeField] private float maxMotorTorque = 80f;
    [SerializeField] private float maxSpeed = 50f;

    [Space]
    [SerializeField] private WheelCollider wheelFl;
    [SerializeField] private WheelCollider wheelFR;

    float currentSpeed = 0;
    
    private List<Transform> nodes;
    private int currentNode = 0;

    public void Setpath(List<Transform> path)
    {
        nodes = path;
    }

    private void FixedUpdate()
    {
        if (nodes != null)
        {
            ApplySteer();
            Drive();
            CheckWaypointDistance();
        }
    }

    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        wheelFl.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
    }

    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * wheelFl.radius * wheelFl.rpm * 60 / 1000;

        if(currentSpeed < maxSpeed)
        {
            wheelFl.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;
        }
        else
        {
            wheelFl.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }
    }

    private void CheckWaypointDistance()
    {
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < 1f)
        {
            if (currentNode == nodes.Count - 1)
                currentNode = 0;
            else
                currentNode++;
        }
    }
}
