using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRoute : MonoBehaviour
{
    [SerializeField] private List<PointRoute> relativePoints = null;
    [SerializeField] private Color colorLine = Color.blue;
    //[SerializeField] private Reference reference = Reference.None;
    //public Reference ReferencePoint => reference;

    public PointRoute[] RelativePoints => relativePoints.ToArray();

    public PointRoute GetRandomPoint()
    {
        int indexRandom = Random.Range(0, relativePoints.Count);
        PointRoute point = relativePoints[indexRandom];

        return point;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = colorLine;

        for (int i = 0; i < relativePoints.Count; i++)
            if(relativePoints[i])
                Gizmos.DrawLine(transform.position, relativePoints[i].transform.position);
    }

    /*
    public enum Reference
    {
        None,
        Hospital,
        HealthCenter,
        Laboratory
    }
    */
}
