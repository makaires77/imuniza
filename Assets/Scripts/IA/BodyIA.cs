using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStatus))]
public class BodyIA : MonoBehaviour
{
    [SerializeField] [Range(0, 100)] private int radius = 2;
    [SerializeField] [Range(0, 100)] private int segments = 40;
    [SerializeField] [Range(0, 10)] private float widhtLine = 4f;
    [SerializeField] private Vector3 offsetCircle = Vector3.zero;

    private LineRenderer line;
    private SphereCollider sphere;
    private CameraBehaviour cameraBehaviour;
    private CharacterStatus characterStatus;
    private ClockBehaviour clock;

    private AlertPanel alertPanel;

    public IA IA { get; private set; }

    private void Awake()
    {
        alertPanel = FindObjectOfType<AlertPanel>();
        cameraBehaviour = FindObjectOfType<CameraBehaviour>();
        clock = FindObjectOfType<ClockBehaviour>();

        characterStatus = GetComponent<CharacterStatus>();
        line = GetComponent<LineRenderer>();
        sphere = GetComponent<SphereCollider>();

        IA = GetComponentInParent<IA>();
    }

    private void Start()
    {
        line.positionCount = segments + 1;
        line.startWidth = widhtLine;
        line.endWidth = widhtLine;
        line.useWorldSpace = false;

        sphere.radius = radius;

        SpawnCircle();
    }

    private void OnMouseDown()
    {
        if(!GameManager.Instance.Pause)
            cameraBehaviour.SetTarget(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(characterStatus.Health == HealthCondition.Sick && other.tag == "Body_IA")
        {
            CharacterStatus victimCharacter = other.GetComponent<CharacterStatus>();
            if (victimCharacter.Health != HealthCondition.Healthy || CharacterStatus.NumberAventsSicknessInday >= clock.CurrentDay)
                return;

            int percentGetSick = characterStatus.SicknessGot.transmissibility - victimCharacter.ImuneSystem;

            if((UnityEngine.Random.Range(0, 100) <= percentGetSick))
            {
                foreach (var vaccinesTaken in victimCharacter.VaccinesTaken)
                {
                    for (int i = 0; i < vaccinesTaken.prevents.Length; i++)
                    {
                        if (characterStatus.SicknessGot == vaccinesTaken.prevents[i])
                            return;
                    }
                }

                victimCharacter.ChangeHealthConditionSick(characterStatus.SicknessGot);
                alertPanel.SpawnAlertInterpersonalInfection(victimCharacter, GetComponent<CharacterStatus>());

                CharacterStatus.NumberAventsSicknessInday++;
            }
        }
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
}
