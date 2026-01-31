using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalRoute : MonoBehaviour
{
    [SerializeField] private Transform sidewalk;

    private PointRoute[] allPointsSidewalk;

    private void Awake()
    {
        allPointsSidewalk = sidewalk.GetComponentsInChildren<PointRoute>();
    }

    public PointRoute GetPointCloserSidewalk(Vector3 currentPosition)
    {
        PointRoute closerPoint = allPointsSidewalk[0];

        float currentDistanceCloser = Vector3.Distance(currentPosition, allPointsSidewalk[0].transform.position);

        for (int i = 1; i < allPointsSidewalk.Length; i++)
        {
            float distanceNewPoint = Vector3.Distance(currentPosition, allPointsSidewalk[i].transform.position);
            if (currentDistanceCloser > distanceNewPoint)
            {
                closerPoint = allPointsSidewalk[i];
                currentDistanceCloser = distanceNewPoint;
            }
        }

        return closerPoint;
    }
}
