using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaccineInventary : MonoBehaviour
{
    [SerializeField] private DataSicknessManager sickness = null;
    [SerializeField] private VaccineItem vaccineItemPrefab = null;
    [SerializeField] private Transform grid = null;

    public List<VaccineItem> Items { get; private set; }

    public void ResetInfo()
    {
        if(Items == null)
        {
            Items = new List<VaccineItem>();

            for (int i = 0; i < sickness.sickness.Length; i++)
            {
                VaccineItem vaccineItem = Instantiate(vaccineItemPrefab, grid);
                vaccineItem.SetData(sickness.sickness[i]);
                vaccineItem.gameObject.SetActive(true);

                Items.Add(vaccineItem);
            }
        }

        foreach (var vaccine in Items)
            vaccine.ResetVaccine();
    }

    public void CheckVaccines(DataSickness sickness)
    {
        foreach (var vaccine in Items)
        {
            if (vaccine.Data == sickness)
            {
                vaccine.HaveVaccine();
                break;
            }
        }
    }
}
