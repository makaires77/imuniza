using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseHumanPoint : MonoBehaviour
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

    private LineRenderer line;
    private BoxCollider colliderArea;
    private SpriteRenderer icon;

    private AlertPanel alertPanel;
    private Camera cameraBehaviour;
    private ClockBehaviour clock;

    private void Awake()
    {
        alertPanel = FindObjectOfType<AlertPanel>();
        cameraBehaviour = FindObjectOfType<Camera>();
        clock = FindObjectOfType<ClockBehaviour>();

        line = GetComponent<LineRenderer>();
        colliderArea = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        iconSickness.sprite = sicknessData.icon;

        line.positionCount = segments + 1;
        line.startWidth = widhtLine;
        line.endWidth = widhtLine;
        line.useWorldSpace = false;

        colliderArea.size = new Vector3(radius, 0.5f, radius);

        SpawnCircle();
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterStatus cs = other.GetComponent<CharacterStatus>();
        if (cs && cs.Health == HealthCondition.Healthy)
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
