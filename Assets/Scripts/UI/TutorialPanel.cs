using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Painel de tutorial que exibe instruções antes do jogo começar.
///
/// Mostra uma sequência de telas com informações sobre:
/// - Objetivo do jogo
/// - Como funcionam as doenças e transmissão
/// - Como usar o Hospital, Laboratório e Centro de Saúde
/// - Controles e interface
///
/// O jogador navega pelas telas e só então o jogo inicia.
/// </summary>
public class TutorialPanel : MonoBehaviour
{
    #region Configuração

    [Header("Painéis de Tutorial")]
    /// <summary>
    /// Array de GameObjects, cada um representando uma tela de tutorial.
    /// Serão exibidos em sequência.
    /// </summary>
    [SerializeField]
    private GameObject[] tutorialPages;

    [Header("Botões de Navegação")]
    /// <summary>
    /// Botão para voltar à tela anterior.
    /// </summary>
    [SerializeField]
    private Button previousButton;

    /// <summary>
    /// Botão para avançar para próxima tela ou iniciar jogo.
    /// </summary>
    [SerializeField]
    private Button nextButton;

    /// <summary>
    /// Botão para pular tutorial e ir direto ao jogo.
    /// </summary>
    [SerializeField]
    private Button skipButton;

    [Header("Textos dos Botões")]
    /// <summary>
    /// Texto do botão próximo (muda para "Jogar" na última tela).
    /// </summary>
    [SerializeField]
    private Text nextButtonText;

    #endregion

    #region Campos Privados

    /// <summary>
    /// Índice da página atual do tutorial.
    /// </summary>
    private int currentPageIndex = 0;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização - configura botões e mostra primeira página.
    /// </summary>
    private void Start()
    {
        // Configura listeners dos botões
        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousPage);

        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);

        if (skipButton != null)
            skipButton.onClick.AddListener(SkipTutorial);

        // Mostra primeira página
        ShowPage(0);
    }

    #endregion

    #region Navegação

    /// <summary>
    /// Exibe uma página específica do tutorial.
    /// </summary>
    /// <param name="pageIndex">Índice da página a exibir</param>
    private void ShowPage(int pageIndex)
    {
        // Valida índice
        if (tutorialPages == null || tutorialPages.Length == 0)
            return;

        // Esconde todas as páginas
        for (int i = 0; i < tutorialPages.Length; i++)
        {
            if (tutorialPages[i] != null)
                tutorialPages[i].SetActive(false);
        }

        // Mostra página atual
        currentPageIndex = Mathf.Clamp(pageIndex, 0, tutorialPages.Length - 1);
        if (tutorialPages[currentPageIndex] != null)
            tutorialPages[currentPageIndex].SetActive(true);

        // Atualiza estado dos botões
        UpdateButtons();
    }

    /// <summary>
    /// Atualiza visibilidade e texto dos botões baseado na página atual.
    /// </summary>
    private void UpdateButtons()
    {
        // Botão anterior: esconde na primeira página
        if (previousButton != null)
            previousButton.gameObject.SetActive(currentPageIndex > 0);

        // Botão próximo: muda texto na última página
        if (nextButtonText != null)
        {
            if (currentPageIndex >= tutorialPages.Length - 1)
                nextButtonText.text = "Jogar!";
            else
                nextButtonText.text = "Próximo";
        }
    }

    /// <summary>
    /// Avança para a próxima página ou inicia o jogo.
    /// </summary>
    public void NextPage()
    {
        if (currentPageIndex >= tutorialPages.Length - 1)
        {
            // Última página - inicia o jogo
            StartGame();
        }
        else
        {
            // Avança para próxima página
            ShowPage(currentPageIndex + 1);
        }
    }

    /// <summary>
    /// Volta para a página anterior.
    /// </summary>
    public void PreviousPage()
    {
        if (currentPageIndex > 0)
            ShowPage(currentPageIndex - 1);
    }

    /// <summary>
    /// Pula o tutorial e inicia o jogo diretamente.
    /// </summary>
    public void SkipTutorial()
    {
        StartGame();
    }

    #endregion

    #region Controle do Jogo

    /// <summary>
    /// Inicia o jogo carregando a cena principal.
    /// </summary>
    private void StartGame()
    {
        // Para música do menu se existir
        if (SoundManager.Instance != null)
            SoundManager.Instance.Stop();

        // Carrega cena do jogo
        SceneManager.LoadScene("Cidade");
    }

    /// <summary>
    /// Abre o painel de tutorial.
    /// Chamado pelo Menu quando o jogador clica em Jogar.
    /// </summary>
    public void OpenTutorial()
    {
        gameObject.SetActive(true);
        ShowPage(0);
    }

    /// <summary>
    /// Fecha o painel de tutorial.
    /// </summary>
    public void CloseTutorial()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
