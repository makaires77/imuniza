using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VaccineMenu : MonoBehaviour
{
    [SerializeField] private VaccineButton prefButton = null;
    [SerializeField] private DataVaccineManager vaccineManager = null;
    [SerializeField] private LaboratorioPanel laboratorioPanel = null;
    [Space]
    [SerializeField] [Range(0, 100)] private int radius = 40;

    private List<VaccineButton> buttonVaccine = new List<VaccineButton>();
    private int numButtons = 0;
    private bool isCreatedButton = false;

    public void Show(CharacterStatus character)
    {
        if (!isCreatedButton)
        {
            isCreatedButton = true;

            numButtons = vaccineManager.vaccines.Length;

            float x = 0f;
            float y = 0f;
            float z = 0f;

            float angle = 0;

            for (int i = 0; i < numButtons; i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

                VaccineButton button = Instantiate(prefButton, transform);
                button.transform.localPosition = new Vector3(x, y, z);

                button.SetData(vaccineManager.vaccines[i]);
                button.gameObject.SetActive(true);
                button.SetBehaviourCharacter(character);

                buttonVaccine.Add(button);

                angle += (360f / numButtons);
            }
        }
        else
        {
            foreach (var button in buttonVaccine)
            {
                button.gameObject.SetActive(true);
            }
        }

        foreach (var button in buttonVaccine)
        {
            bool isVaccineAvaliable = false;
            foreach (var vaccine in laboratorioPanel.vaccineItems)
            {
                if(button.VaccineTake.name == vaccine.vaccineData.name)
                {
                    isVaccineAvaliable = vaccine.GetNumVaccinesAvailable() > 0;
                    break;
                }
            }

            button.GetComponent<Button>().interactable = isVaccineAvaliable;
        }
    }

    public void Hide()
    {
        if (isCreatedButton)
        {
            foreach (var button in buttonVaccine)
            {
                button.gameObject.SetActive(false);
            }
        }
    }
}