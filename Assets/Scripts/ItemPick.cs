using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemPick : MonoBehaviour
{
    GameObject bolsa;
    SpriteRenderer bolsaSpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        bolsa = GameObject.Find("Player/Bolsa");
        bolsaSpriteRenderer = bolsa.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Item"))
        {
            bolsaSpriteRenderer.color = collision.gameObject.GetComponent<SpriteRenderer>().color; ;
        }else if (collision.gameObject.tag.Equals("NPC"))
        {
            Color npcColor = collision.gameObject.GetComponent<SpriteRenderer>().color;
            if (npcColor == bolsaSpriteRenderer.color)
            {
                collision.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                bolsaSpriteRenderer.color = Color.gray;
            }
        }
        
    }
}
