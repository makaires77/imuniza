using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaboratorioPanel : MonoBehaviour
{
    [SerializeField] private DataVaccineManager vaccines = null;
    [Space]
    [SerializeField] private GameObject panel = null;
    [SerializeField] private Transform grid = null;
    [SerializeField] private VacinneSearchItem vaccineItem = null;

    public List<VacinneSearchItem> vaccineItems = new List<VacinneSearchItem>();

    private void Start()
    {
        SpawnVaccines();
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    private void SpawnVaccines()
    {
        for (int i = 0; i < vaccines.vaccines.Length; i++)
        {
            VacinneSearchItem searchItem = Instantiate(vaccineItem, grid);
            searchItem.SetData(vaccines.vaccines[i]);

            searchItem.gameObject.SetActive(true);
            vaccineItems.Add(searchItem);
        }
    }

    public void DisableAllVaccinesForSearch()
    {
        foreach (var vaccine in vaccineItems)
        {
            vaccine.GetComponentInChildren<Button>().interactable = false;
        }
    }

    public void EnableAllVaccinesForSearch()
    {
        foreach (var vaccine in vaccineItems)
        {
            vaccine.GetComponentInChildren<Button>().interactable = true;
        }
    }
}
