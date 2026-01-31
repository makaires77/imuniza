using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMLPanel : MonoBehaviour
{
    [SerializeField] private IMLItem itemIML = null;
    [SerializeField] private Transform grid = null;
    [SerializeField] private GameObject panel = null;

    private IML iml;

    private void Awake()
    {
        iml = FindObjectOfType<IML>();
    }

    public void Show()
    {
        IMLItem[] oldCharacters = grid.GetComponentsInChildren<IMLItem>();
        for (int i = 0; i < oldCharacters.Length; i++)
            DestroyImmediate(oldCharacters[i].gameObject);
		
		if(iml.Characters.Count > 0)
		{
			IMLItem curCharacterInQueue;
			for (int i = iml.CurrentIndexCharacter; i < iml.Characters.Count; i++)
			{
				curCharacterInQueue = Instantiate(itemIML, grid);
				curCharacterInQueue.UpdateDisplay();

				curCharacterInQueue.gameObject.SetActive(true);
			}
		}

        panel.SetActive(true);
    }
}
