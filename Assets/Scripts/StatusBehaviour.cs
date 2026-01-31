using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusBehaviour : MonoBehaviour
{
    [SerializeField] private Sprite sickImg = null;
    [SerializeField] private Sprite deadImg = null;
    [SerializeField] private Sprite imlImg = null;
    [Space]
    [SerializeField] private Sprite hospitalImg = null;
    [SerializeField] private Sprite healthCenterImg = null;
    [SerializeField] private Sprite laboratorioImg = null;

    [SerializeField] private TextMeshPro numberQueueIML = null;

    private CameraBehaviour cameraBehaviour;
    private SpriteRenderer spriteRenderer = null;


    private void Awake()
    {
        cameraBehaviour = FindObjectOfType<CameraBehaviour>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        transform.LookAt(cameraBehaviour.transform);
    }

    public void SetHeal()
    {
        spriteRenderer.sprite = null;
    }

    public void SetSick()
    {
        spriteRenderer.sprite = sickImg;
    }

    public void SetDead()
    {
        spriteRenderer.sprite = deadImg;
    }

    public void SetIML(int numberQueue)
    {
        spriteRenderer.sprite = imlImg;
        numberQueueIML.text = numberQueue.ToString();
        numberQueueIML.gameObject.SetActive(true);
    }

    public void SetHospitalDestination()
    {
        spriteRenderer.sprite = hospitalImg;
    }

    public void SetHealthCenterDestination()
    {
        spriteRenderer.sprite = healthCenterImg;
    }

    public void SetLaboratorioDestination()
    {
        spriteRenderer.sprite = laboratorioImg;
    }
}
