using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button playButton = null;

    private void Start()
    {
        playButton.onClick.AddListener(() => PlayGame());
    }

    private void PlayGame()
    {
        SoundManager.Instance.Stop();
        SceneManager.LoadScene("Cidade");
    }
}
