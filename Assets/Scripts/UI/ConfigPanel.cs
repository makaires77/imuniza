using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfigPanel : MonoBehaviour
{
    [SerializeField] private Button buttonMenu = null;
    [SerializeField] private Slider soundSlider = null;
    [SerializeField] private Button buttonQuit = null;
    [Space]
    [SerializeField] private GameManager gameManager = null;

    private void Start()
    {
        buttonMenu.onClick.AddListener(() => LoadMenu());
        soundSlider.onValueChanged.AddListener((float changevalue) => AudioManager.ChangeVolume(changevalue));
        buttonQuit.onClick.AddListener(() => Application.Quit());
    }

    private void OnEnable()
    {
        gameManager.Pause = true;
    }

    private void OnDisable()
    {
        gameManager.Pause = false;
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}
