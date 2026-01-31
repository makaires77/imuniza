using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPath : MonoBehaviour
{
    [SerializeField] private Color lineColor;
    private List<Transform> nodesViewDebug = new List<Transform>();

    public List<Transform> GeneratePath()
    {
        Transform[] pathTransform = GetComponentsInChildren<Transform>();

        List<Transform> path = new List<Transform>();
        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != transform)
                path.Add(pathTransform[i]);
        }

        return path;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = lineColor;

        Transform[] pathTransform = GetComponentsInChildren<Transform>();
        nodesViewDebug = new List<Transform>();

        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != transform)
                nodesViewDebug.Add(pathTransform[i]);
        }

        for (int i = 0; i < nodesViewDebug.Count; i++)
        {
            Vector3 currentNode = nodesViewDebug[i].position;
            Vector3 previousNode = Vector3.zero;

            previousNode = i > 0 ? nodesViewDebug[i - 1].position : nodesViewDebug[nodesViewDebug.Count - 1].position;

            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawSphere(currentNode, 0.3f);
        }

    }
}
