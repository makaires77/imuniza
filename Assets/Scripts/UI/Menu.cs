using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador do menu principal do jogo.
///
/// Responsabilidades:
/// - Gerenciar botão de jogar
/// - Abrir painel de tutorial antes de iniciar
/// - Opcionalmente ir direto ao jogo (se tutorial já foi visto)
///
/// Fluxo:
/// Menu → [Clica Jogar] → Tutorial → [Termina/Pula] → Jogo
/// </summary>
public class Menu : MonoBehaviour
{
    #region Configuração

    [Header("Botões")]
    /// <summary>
    /// Botão principal para iniciar o jogo.
    /// </summary>
    [SerializeField]
    private Button playButton = null;

    /// <summary>
    /// Botão para jogar sem ver o tutorial (opcional).
    /// </summary>
    [SerializeField]
    private Button playWithoutTutorialButton = null;

    [Header("Painéis")]
    /// <summary>
    /// Painel de tutorial a ser exibido antes do jogo.
    /// Se null, vai direto para o jogo.
    /// </summary>
    [SerializeField]
    private TutorialPanel tutorialPanel = null;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização - configura listeners dos botões.
    /// </summary>
    private void Start()
    {
        // Botão principal - abre tutorial ou jogo
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClick);

        // Botão de pular tutorial (opcional)
        if (playWithoutTutorialButton != null)
            playWithoutTutorialButton.onClick.AddListener(PlayGameDirectly);
    }

    #endregion

    #region Ações dos Botões

    /// <summary>
    /// Chamado quando o jogador clica em Jogar.
    /// Abre o tutorial se existir, senão vai direto ao jogo.
    /// </summary>
    private void OnPlayButtonClick()
    {
        if (tutorialPanel != null)
        {
            // Mostra tutorial primeiro
            tutorialPanel.OpenTutorial();
        }
        else
        {
            // Vai direto ao jogo
            PlayGameDirectly();
        }
    }

    /// <summary>
    /// Inicia o jogo diretamente, sem tutorial.
    /// </summary>
    public void PlayGameDirectly()
    {
        // Para música do menu
        if (SoundManager.Instance != null)
            SoundManager.Instance.Stop();

        // Carrega cena do jogo
        SceneManager.LoadScene("Cidade");
    }

    #endregion
}
