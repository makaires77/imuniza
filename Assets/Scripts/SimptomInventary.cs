using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimptomInventary : MonoBehaviour
{
    [SerializeField] private DataSymptomsManager simptoms = null;

    [SerializeField] private SimptomItem simptomItemPrefab = null;
    [SerializeField] private Transform grid = null;

    public List<SimptomItem> Items { get; private set; }

    public void ResetInfo()
    {
        if (Items == null)
        {
            Items = new List<SimptomItem>();

            for (int i = 0; i < simptoms.simptoms.Length; i++)
            {
                SimptomItem simptomItem = Instantiate(simptomItemPrefab, grid);
                simptomItem.SetData(simptoms.simptoms[i]);
                simptomItem.gameObject.SetActive(true);

                Items.Add(simptomItem);
            }
        }

        foreach (var item in Items)
            item.ResetSimptom();
    }

    public void CheckSimptom(DataSymptoms simptomCurrent)
    {
        foreach (var simptom in Items)
        {
            if (simptom.Data == simptomCurrent)
            {
                simptom.HaveSimptom();
                break;
            }
        }
    }
}
