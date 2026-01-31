using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TensorGrid : MonoBehaviour
{

    [SerializeField] private int width = 10;
    [SerializeField] private int heigth = 10;
    [Space]
    [SerializeField] private float spacingCells = 1.5f;
    [Space]
    [SerializeField] private Transform firstLayer;

    public List<List<Transform>> TrackingMain { get; private set; }

    private void Awake()
    {
        TrackingMain = new List<List<Transform>>();
    }
    void Start()
    {
        GameObject tensorPoint = new GameObject();
        tensorPoint.AddComponent(typeof(TensorPoint));

        float spacingX = 0;
        for (int x = 0; x < width; x++)
        {
            TrackingMain.Add(new List<Transform>());

            float spacingY = 0f;
            for (int y = 0; y < heigth; y++)
            {
                GameObject obj = Instantiate(tensorPoint, firstLayer);
                obj.name = x + "_" + y;
                obj.transform.position = new Vector3(spacingX, 0.0f, spacingY);
                TensorPoint tp = obj.GetComponent<TensorPoint>();
                tp.direction = Vector3.left;
                tp.pos = obj.transform.position;

                spacingY += spacingCells;

                TrackingMain[x].Add(obj.transform);
            }
            spacingX += spacingCells;
        }

        Destroy(tensorPoint);
    }

    private void OnDrawGizmos()
    {
        float spacingX = 0;
        for (int x = 0; x < width; x++)
        {
            float spacingY = 0f;
            for (int y = 0; y < heigth; y++)
            {
                DrawArrowGizmos(new Vector3(spacingX, 0.0f, spacingY), Vector3.forward);
                spacingY += spacingCells;
            }
            spacingX += spacingCells;
        }
    }

    public static void DrawArrowGizmos(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }
}
