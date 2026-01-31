using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NPCSickness : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Color[] sickColors;
    
    [Tooltip("Minimum time to change color")]
    [Range (1.0f,10f)]
    public float minRandomTime;

    [Tooltip("Maximum time to change color")]
    [Range(1.0f, 10f)]
    public float maxRandomTime;

    [SerializeField]
    private float timeCounter = 0;

    [SerializeField]
    private float pickedTime;
    

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        pickedTime = Random.Range(minRandomTime, maxRandomTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (spriteRenderer.color == Color.green)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter > pickedTime)
            {
                Color newColor = sickColors[Random.Range(0,sickColors.Length)];
                spriteRenderer.color = newColor;
                timeCounter = 0;
                pickedTime = Random.Range(minRandomTime, maxRandomTime);
            }
        }
        
        
    }
}
