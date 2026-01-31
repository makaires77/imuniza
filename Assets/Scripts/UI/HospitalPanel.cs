using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HospitalPanel : MonoBehaviour
{
    [SerializeField] private Text statusBeds;
    [SerializeField] private GameObject panel = null;
    [SerializeField] private BedItem prefabBedPatients = null;
    [SerializeField] private GameObject buySlotButton = null;
    [SerializeField] private Transform grid = null;
    [Space]
    [SerializeField] private PopUp popUp = null;

    private List<BedItem> beds = new List<BedItem>();
    private int costBuySlot = 50;


    private Hospital hospital;

    private void Awake()
    {
        hospital = FindObjectOfType<Hospital>();
    }

    private void Start()
    {
        InstanceBed();

        buySlotButton.transform.SetAsLastSibling();
    }

    public void OpenPanel()
    {
        UpdateBedDisplay();

        buySlotButton.transform.SetAsLastSibling();

        panel.SetActive(true);
    }

    public void OpenPopUpBuyBed()
    {
        popUp.ShowQuestionYesNo("Deseja comprar um slot por R$ " + costBuySlot + "?", () => BuySlotBed(), null);
    }

    private void BuySlotBed()
    {
        if(costBuySlot > MoneyManager.CurrentMoney)
            return;

        InstanceBed();

        MoneyManager.CurrentMoney -= costBuySlot;
        costBuySlot += costBuySlot;

        buySlotButton.transform.SetAsLastSibling();
		
        if (hospital.NumberBed == hospital.NumberMaxBed)
            buySlotButton.SetActive(false);
		
		hospital.TreatPatientInQueue();

        UpdateBedDisplay();
    }

    private void InstanceBed()
    {
        hospital.NumberBed++;
        BedItem bed = Instantiate(prefabBedPatients, grid);
        beds.Add(bed);

        bed.gameObject.SetActive(true);
    }

    private void UpdateBedDisplay()
    {
        int numBedFree = 0;
        foreach (var bed in beds)
        {
            if (!bed.IsBusyBed)
                numBedFree++;
        }

        statusBeds.text = "ocupados: " + (hospital.NumberBed - numBedFree) + "/" + hospital.NumberBed;

        foreach (var bed in beds)
            bed.UpdateDisplay();
    }

    public BedItem OccupyBed(CharacterStatus character, int days)
    {
        foreach (var bed in beds)
        {
            if (!bed.IsBusyBed)
            {
                bed.Occupy(character, days);
                UpdateBedDisplay();
                return bed;
            }
        }

        return null;
    }

    public void VacateBed(CharacterStatus character)
    {
        foreach (var bed in beds)
        {
            if (bed.IsBusyBed && bed.Character == character)
                bed.Vacate();
        }

        UpdateBedDisplay();
    }
}
