using System.Collections;
using System.Collections.Generic;
using UnityEngine;    
using UnityEditor;
using System;

#if UNITY_EDITOR

/*
public class MapData : MonoBehaviour
{
    private static Dictionary<Type, Action<GameObject>> compBehaviour = new Dictionary<Type, Action<GameObject>>();

    [MenuItem("Map/Extract")]
    private static void Extract()
    {
        compBehaviour = new Dictionary<Type, Action<GameObject>>();
        compBehaviour.Add(typeof(MeshFilter), MeshData);
        compBehaviour.Add(typeof(UnityTile), TextureData);

        GameObject map = Selection.gameObjects[0];
        Component[] componentsInMap = map.GetComponentsInChildren(typeof(Component));

	    for(int i = 0; i < componentsInMap.Length; i++) 
	    {
	        Action<GameObject> behaviour;
	        if(compBehaviour.TryGetValue(componentsInMap[i].GetType(), out behaviour))
	            behaviour(componentsInMap[i].gameObject);
            else
	        Debug.Log("Component not valided: " + componentsInMap[i].GetType());
	    }
    }

    private static void MeshData(GameObject obj)
    {
        Debug.Log("Find Mesh");
    }

    private static void TextureData(GameObject obj)
    {
        UnityTile ut = obj.GetComponent<UnityTile>();
        Texture2D tex = ut.GetRasterData();

        byte[] data = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Map-Texture/" + obj.name.Replace("/", "-") + ".png", data);

        Debug.Log("Find material");
    }
}
*/

#endif