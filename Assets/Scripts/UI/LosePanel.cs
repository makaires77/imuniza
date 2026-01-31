using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LosePanel : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
