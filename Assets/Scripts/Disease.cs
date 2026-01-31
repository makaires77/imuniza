using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disease : MonoBehaviour
{
    [SerializeField] private DataSickness sicknessData = null;
    public DataSickness Sickness { get { return sicknessData; } set { sicknessData = value; } }
    [Space]
    [SerializeField] [Range(0, 100)] private int radius = 40;
    [SerializeField] [Range(0, 100)] private int segments = 40;
    [SerializeField] [Range(0, 10)] private float widhtLine = 4f;
    [SerializeField] private Vector3 offsetCircle = Vector3.zero;
    [Space]
    [SerializeField] private SpriteRenderer iconSickness = null;
    [Space]
    [SerializeField] private float m_DistanceZ = 10f;
    Vector3 distanceFromCamera;

    private LineRenderer line;
    private BoxCollider sphere;
    private SpriteRenderer icon;

    private AlertPanel alertPanel;
    private PanelSickness panelSickness;
    private Camera cameraBehaviour;
    private ClockBehaviour clock;

    Plane plane;

    private void Awake()
    {
        alertPanel = FindObjectOfType<AlertPanel>();
        panelSickness = FindObjectOfType<PanelSickness>();
        cameraBehaviour = FindObjectOfType<Camera>();
        clock = FindObjectOfType<ClockBehaviour>();

        line = GetComponent<LineRenderer>();
        sphere = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        iconSickness.sprite = sicknessData.icon;

        line.positionCount = segments + 1;
        line.startWidth = widhtLine;
        line.endWidth = widhtLine;
        line.useWorldSpace = false;

        sphere.size = new Vector3(radius, 1, radius);

        SpawnCircle();

        distanceFromCamera = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - m_DistanceZ);
        plane = new Plane(Vector3.forward, distanceFromCamera);
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            distanceFromCamera = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - m_DistanceZ);
            plane.SetNormalAndPosition(Vector3.forward, distanceFromCamera);

            if (plane.Raycast(ray, out float dist))
            {
                Vector3 posTarget = ray.GetPoint(dist);
                ray.direction = (posTarget - ray.origin).normalized;
                Debug.DrawRay(ray.origin, ray.direction * 99999, Color.red);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Diseases")
                        panelSickness.Show(sicknessData);
                }
            }
            else
            {
                distanceFromCamera = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + m_DistanceZ);
                plane.SetNormalAndPosition(Vector3.forward, distanceFromCamera);

                if (plane.Raycast(ray, out dist))
                {
                    Vector3 posTarget = ray.GetPoint(dist);
                    ray.direction = (posTarget - ray.origin).normalized;

                    Debug.DrawRay(ray.origin, ray.direction * 99999, Color.red);

                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform.tag == "Diseases")
                            panelSickness.Show(sicknessData);
                    }
                }
            }
        }

        /*
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.tag == "Diseases")
                    panelSickness.Show(sicknessData);
            }
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterStatus cs = other.GetComponent<CharacterStatus>();
        if (cs && cs.Health == HealthCondition.Healthy && cs.IsPermissionSick)
            Transmission(cs);
    }

    private void SpawnCircle()
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x + offsetCircle.x, y + offsetCircle.y, z + offsetCircle.z));

            angle += (360f / segments);
        }
    }

    private void Transmission(CharacterStatus character)
    {        
        if (sicknessData.Transmission(character, clock.CurrentDay))
            alertPanel.SpawnAlertFocus(character);
    }
}
