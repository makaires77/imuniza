using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
	[SerializeField] private SkinnedMeshRenderer smrBody = null;

	[SerializeField] private MeshRenderer[] hairType = null;	

	private void Awake()
	{
		int randomOffsetFirstCol = Random.Range(0, 3);
		int randomOffsetSecondCol = Random.Range(0, 3);

		smrBody.material.SetTextureOffset("_MainTex", new Vector2(0.25f * randomOffsetFirstCol, 0.25f * randomOffsetSecondCol));
	
		int randomHeair = Random.Range(0, hairType.Length - 1);
		
		for(int i = 0; i < hairType.Length; i++)
			hairType[i].gameObject.SetActive(false);
		hairType[randomHeair].gameObject.SetActive(true);
	}
}
