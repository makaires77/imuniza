using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador da tela de splash/abertura do jogo.
///
/// Responsabilidades:
/// - Exibir tela de splash inicial
/// - Carregar a próxima cena (Menu) em background
/// - Iniciar o áudio quando o usuário interagir
///
/// IMPORTANTE - WebGL:
/// Navegadores bloqueiam áudio automático. O som só pode
/// iniciar após uma interação do usuário (clique/toque).
/// Por isso, exibimos uma mensagem pedindo para clicar.
/// </summary>
public class SplashScreen : MonoBehaviour
{
    #region Campos Privados

    /// <summary>
    /// Operação de carregamento assíncrono da próxima cena.
    /// </summary>
    private AsyncOperation asyncLoadMenu;

    /// <summary>
    /// Indica se o usuário já interagiu (clicou).
    /// </summary>
    private bool userInteracted = false;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização - começa a carregar o Menu em background.
    /// </summary>
    private void Start()
    {
        // Carrega Menu em background mas não ativa ainda
        asyncLoadMenu = SceneManager.LoadSceneAsync(1);
        asyncLoadMenu.allowSceneActivation = false;
    }

    /// <summary>
    /// Atualização - detecta clique do usuário para iniciar áudio e prosseguir.
    /// </summary>
    private void Update()
    {
        // Aguarda interação do usuário
        if (!userInteracted)
        {
            if (Input.GetMouseButtonDown(0) || Input.anyKeyDown || Input.touchCount > 0)
            {
                userInteracted = true;
                OnUserInteraction();
            }
        }
    }

    #endregion

    #region Interação

    /// <summary>
    /// Chamado quando o usuário interage pela primeira vez.
    /// Inicia o áudio e prepara para prosseguir.
    /// </summary>
    private void OnUserInteraction()
    {
        // Inicia o áudio (agora permitido pelo navegador)
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.Play();
        }

        // Aguarda um momento e vai para o Menu
        StartCoroutine(GoToMenuAfterDelay(0.5f));
    }

    /// <summary>
    /// Aguarda um delay e então ativa a cena do Menu.
    /// </summary>
    private IEnumerator GoToMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ActivedScene();
    }

    /// <summary>
    /// Ativa a cena do Menu (chamado após carregamento e interação).
    /// </summary>
    public void ActivedScene()
    {
        asyncLoadMenu.allowSceneActivation = true;
    }

    #endregion
}
