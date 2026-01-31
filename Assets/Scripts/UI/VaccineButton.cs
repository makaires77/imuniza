using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VaccineButton : MonoBehaviour
{
    [SerializeField] private Image icon = null;
    private Button button;

    public DataVaccine VaccineTake { get; private set; }

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void SetData(DataVaccine vaccine)
    {
        VaccineTake = vaccine;

        icon.sprite = VaccineTake.icon;
    }

    public void SetBehaviourCharacter(CharacterStatus character)
    {
        button.onClick.AddListener(() => character.SetVaccineWantTake(VaccineTake));
    }
}
