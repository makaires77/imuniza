using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class ActionPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelMenu = null;
    [SerializeField] private VaccineMenu vaccineMenu = null;
    [SerializeField] private GameObject menuActionStatus = null;
    [SerializeField] private Button[] buttonsMenu = null;
    [SerializeField] private Button buttonDeadMenu = null;
    [Space]
    [SerializeField] private Text textFullName = null;
    [SerializeField] private Text textAge = null;
    [SerializeField] private Text textImmuniSystem = null;
    [SerializeField] private Text textHealthCondition = null;
    [Space]
    [SerializeField] private VaccineInventary inventaryVaccine = null;
    [SerializeField] private SimptomInventary inventarySymptom = null;

    private IML iml = null;
    private CameraBehaviour cameraBehaviour;
    private Transform target = null;

    private IA agent;

    private void Awake()
    {
        cameraBehaviour = FindObjectOfType<CameraBehaviour>();
        iml = FindObjectOfType<IML>();
    }

    private void Update()
    {
        if (!GameManager.Instance.Pause)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

                if (hit.transform && hit.transform.tag == "Body_IA")
                    Show(hit.transform);
                else if (hit.transform && target != null && hit.transform.gameObject.layer != LayerMask.NameToLayer("UI"))
                {
                    Hide();
                    cameraBehaviour.SetTarget(null);
                }
            }

            if (target != null)
            {
                transform.position = target.position;
                transform.LookAt(cameraBehaviour.transform.position);
            }
        }
    }

    public void SetDestination(Transform destination)
    {
        if (agent == null)
            Debug.Log($"Agent null: {target.parent.name}");


        agent.NextWaypointTarget(destination);

        if (destination.GetComponent<IRouteGlobal>().GetType() == typeof(Laboratory))
            agent.GetComponentInChildren<StatusBehaviour>().SetLaboratorioDestination();
        else if (destination.GetComponent<IRouteGlobal>().GetType() == typeof(Hospital))
            agent.GetComponentInChildren<StatusBehaviour>().SetHospitalDestination();
        else if (destination.GetComponent<IRouteGlobal>().GetType() == typeof(HealthCenter))
            agent.GetComponentInChildren<StatusBehaviour>().SetHealthCenterDestination();
    }

    public void Show(Transform targetNew, bool activeButtons = true)
    {
        agent = targetNew.GetComponent<BodyIA>().IA;
        target = targetNew;

        if (agent == null)
            Debug.Log($"Agent null: {targetNew.parent.name}");

        CharacterStatus status = targetNew.GetComponentInChildren<CharacterStatus>(true);
        textFullName.text = status.FullName;
        textAge.text = status.Age.ToString();
        textImmuniSystem.text = status.ImuneSystem.ToString() + "%";

        switch (status.Health)
        {
            case HealthCondition.Healthy:
                textHealthCondition.text = "Saudável";
                break;
            case HealthCondition.Sick:
                textHealthCondition.text = "Doente";
                break;
            case HealthCondition.Dead:
                textHealthCondition.text = "Morto";
                break;
        }

        inventaryVaccine.ResetInfo();
        inventarySymptom.ResetInfo();

        foreach (var vaccines in status.VaccinesTaken)
        {
            for (int i = 0; i < vaccines.prevents.Length; i++)
                inventaryVaccine.CheckVaccines(vaccines.prevents[i]);
        }

        if (status.Health == HealthCondition.Sick)
        {
            foreach (var symptoms in status.SicknessGot.symptoms)
                inventarySymptom.CheckSimptom(symptoms);
        }

        panelMenu.SetActive(true);
        if (activeButtons)
            StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        HideVaccineMenu();
        FadeOut();
        panelMenu.SetActive(false);
    }

    public void HideHealthMenu()
    {
        HideVaccineMenu();
        FadeOut();
    }

    public void HideVaccineMenu()
    {
        vaccineMenu.Hide();
        vaccineMenu.gameObject.SetActive(false);
    }

    public void OpenVaccineMenu()
    {
        FadeOut();
        vaccineMenu.gameObject.SetActive(true);
        vaccineMenu.Show(target.GetComponentInChildren<CharacterStatus>());
    }

    private IEnumerator FadeIn()
    {
        if (target.GetComponentInChildren<CharacterStatus>().Health == HealthCondition.Dead)
        {
            buttonDeadMenu.gameObject.SetActive(true);
            buttonDeadMenu.interactable = true;
            Color color = buttonDeadMenu.image.color;

            for (int x = 0; x <= 255; x += 15)
            {
                color.a = x / 255f;
                buttonDeadMenu.image.color = color;
                yield return new WaitForSeconds(0.00001f);
            }
        }
        else
        {
            int numButtons = target.GetComponentInChildren<CharacterStatus>().Health == HealthCondition.Sick ?
                buttonsMenu.Length : buttonsMenu.Length - 1;

            menuActionStatus.SetActive(true);
            for (int i = 0; i < numButtons; i++)
            {
                buttonsMenu[i].interactable = true;
                Color color = buttonsMenu[i].image.color;

                for (int x = 0; x <= 255; x += 15)
                {
                    color.a = x / 255f;
                    buttonsMenu[i].image.color = color;
                    yield return new WaitForSeconds(0.00001f);
                }
            }
        }
    }

    public void CallIML()
    {
        if (!target.GetComponentInChildren<CharacterStatus>().IsCallIML)
            iml.CallCollect(target);

        FadeOut();
    }

    private void FadeOut()
    {
        StopAllCoroutines();

        buttonDeadMenu.interactable = false;
        Color color = buttonDeadMenu.image.color;
        color.a = 0f;
        buttonDeadMenu.image.color = color;
        buttonDeadMenu.gameObject.SetActive(false);

        for (int i = 0; i < buttonsMenu.Length; i++)
        {
            buttonsMenu[i].interactable = false;
            color = buttonsMenu[i].image.color;
            color.a = 0f;
            buttonsMenu[i].image.color = color;
        }
        menuActionStatus.SetActive(false);
    }
}
