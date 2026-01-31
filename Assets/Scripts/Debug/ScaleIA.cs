using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleIA : MonoBehaviour
{
	[SerializeField] private InputField inputField = null;
	
	[SerializeField] private Transform characterMom = null;
	private IA[] characters;

	private void Awake()
	{
		characters = characterMom.GetComponentsInChildren<IA>();
	}

	public void ChangeScale()
	{
		float scaleNew = (float)Convert.ToDouble(inputField.text);
		for(int i = 0; i < characters.Length; i++)
		{
			characters[i].transform.localScale = new Vector3(scaleNew, scaleNew, scaleNew);
		}
	}
}
