using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    AsyncOperation asycLoadMenu;

    private void Start()
    {
        asycLoadMenu = SceneManager.LoadSceneAsync(1);
        asycLoadMenu.allowSceneActivation = false;
    }

    public void ActivedScene()
    {
        asycLoadMenu.allowSceneActivation = true;
    }
}
