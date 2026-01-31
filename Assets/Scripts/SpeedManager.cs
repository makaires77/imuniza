using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gerenciador de velocidade do tempo do jogo.
///
/// Responsabilidades:
/// - Controlar a velocidade do tempo (0.5x, 1x, 2x, pausa)
/// - Gerenciar os botões de controle de velocidade na UI
/// - Sincronizar velocidade com os sistemas de animação e IA
///
/// O sistema de tempo do jogo NÃO usa Time.timeScale do Unity,
/// pois isso conflita com as coroutines que usam WaitForSeconds.
/// Em vez disso, cada sistema (ClockBehaviour, IAs) ajusta seus
/// intervalos baseado no valor de SpeedGame.
///
/// Velocidades disponíveis:
/// - 0.5x (Lento) - Mais tempo para planejar
/// - 1.0x (Normal) - Velocidade padrão
/// - 2.0x (Rápido) - Para acelerar partes tranquilas
/// - 0.0x (Pausa) - Jogo completamente pausado
///
/// Requer: CharactersManager, CarManager, GameManager
/// </summary>
public class SpeedManager : MonoBehaviour
{
    #region Referências de UI

    /// <summary>
    /// Botão para pausar o jogo (velocidade 0x).
    /// </summary>
    [SerializeField] private Button pauseButton = null;

    /// <summary>
    /// Botão para velocidade normal (1x).
    /// </summary>
    [SerializeField] private Button playButton = null;

    /// <summary>
    /// Botão para velocidade dobrada (2x).
    /// </summary>
    [SerializeField] private Button twoTimesButton = null;

    /// <summary>
    /// Botão para velocidade reduzida (0.5x).
    /// </summary>
    [SerializeField] private Button halfButton = null;

    #endregion

    #region Referências de Sistemas

    [Space]
    /// <summary>
    /// Gerenciador de personagens - para atualizar velocidade das IAs.
    /// </summary>
    [SerializeField] private CharactersManager characterManager = null;

    /// <summary>
    /// Gerenciador de veículos (ambulância).
    /// </summary>
    [SerializeField] private CarManager carManager = null;

    /// <summary>
    /// Gerenciador principal do jogo - para sincronizar pausa.
    /// </summary>
    [SerializeField] private GameManager gameManager = null;

    #endregion

    #region Propriedade de Velocidade

    /// <summary>
    /// Velocidade atual do jogo (valor estático para acesso global).
    /// Valores: 0 (pausado), 0.5 (lento), 1 (normal), 2 (rápido)
    /// </summary>
    private static float speedGame = 1f;

    /// <summary>
    /// Velocidade atual do jogo.
    ///
    /// Quando alterada, atualiza automaticamente a velocidade
    /// de todas as IAs dos personagens.
    ///
    /// Nota: Outros sistemas (ClockBehaviour) leem este valor
    /// diretamente para ajustar seus intervalos.
    /// </summary>
    public float SpeedGame
    {
        get
        {
            return speedGame;
        }
        set
        {
            speedGame = value;

            // Atualiza velocidade de todas as IAs
            if (characterManager.IAs != null)
            {
                for (int i = 0; i < characterManager.IAs.Length; i++)
                {
                    characterManager.IAs[i].SpeedUpdate(speedGame);
                }
            }
        }
    }

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização precoce - obtém referências.
    /// </summary>
    private void Awake()
    {
        characterManager = FindObjectOfType<CharactersManager>();
    }

    /// <summary>
    /// Inicialização - configura listeners dos botões.
    /// Inicia o jogo em velocidade normal (1x).
    /// </summary>
    private void Start()
    {
        // Configura listeners dos botões
        pauseButton.onClick.AddListener(() => SpeedTimesPauseButton());
        playButton.onClick.AddListener(() => SpeedTimesPlayButton());
        twoTimesButton.onClick.AddListener(() => SpeedTimesTwoButton());
        halfButton.onClick.AddListener(() => SpeedTimesHalfButton());

        // Inicia em velocidade normal
        SpeedTimesPlayButton();
    }

    #endregion

    #region Métodos de Controle de Velocidade

    /// <summary>
    /// Define velocidade para 2x (rápido).
    ///
    /// Útil para acelerar partes do jogo onde não há
    /// muita ação ou o jogador quer ver resultados mais rápido.
    ///
    /// Ações:
    /// - Retoma todas as animações
    /// - Remove flags de pausa
    /// - Define SpeedGame = 2
    /// - Desabilita botão 2x (já ativo)
    /// </summary>
    public void SpeedTimesTwoButton()
    {
        // Retoma sistema de jogo
        GameManager.PlayAllAnimations();
        ClockBehaviour.PauseEvent = false;
        gameManager.Pause = false;

        // Define velocidade dobrada
        SpeedGame = 2f;

        // Atualiza estado dos botões
        twoTimesButton.interactable = false;
        halfButton.interactable = true;
        playButton.interactable = true;
        pauseButton.interactable = true;

        // NOTA: Time.timeScale NÃO é usado pois conflita
        // com as coroutines que usam WaitForSeconds customizado
    }

    /// <summary>
    /// Pausa o jogo completamente (velocidade 0x).
    ///
    /// O jogo fica completamente parado, permitindo que o
    /// jogador planeje suas ações sem pressão de tempo.
    ///
    /// Ações:
    /// - Para todas as animações
    /// - Define flags de pausa em todos os sistemas
    /// - Define SpeedGame = 0
    /// - Desabilita botão de pausa (já ativo)
    /// </summary>
    public void SpeedTimesPauseButton()
    {
        // Para sistema de jogo
        GameManager.PauseAllAnimations();
        ClockBehaviour.PauseEvent = true;
        gameManager.Pause = true;

        // Define velocidade zero (pausado)
        SpeedGame = 0f;

        // Atualiza estado dos botões
        halfButton.interactable = true;
        twoTimesButton.interactable = true;
        playButton.interactable = true;
        pauseButton.interactable = false;

        // NOTA: Time.timeScale NÃO é usado pois conflita
        // com as coroutines que usam WaitForSeconds customizado
    }

    /// <summary>
    /// Define velocidade normal (1x).
    ///
    /// Velocidade padrão do jogo, balanceada para
    /// permitir reação adequada aos eventos.
    ///
    /// Ações:
    /// - Retoma todas as animações
    /// - Remove flags de pausa
    /// - Define SpeedGame = 1
    /// - Desabilita botão play (já ativo)
    /// </summary>
    public void SpeedTimesPlayButton()
    {
        // Retoma sistema de jogo
        GameManager.PlayAllAnimations();
        ClockBehaviour.PauseEvent = false;
        gameManager.Pause = false;

        // Define velocidade normal
        SpeedGame = 1f;

        // Atualiza estado dos botões
        halfButton.interactable = true;
        twoTimesButton.interactable = true;
        pauseButton.interactable = true;
        playButton.interactable = false;

        // NOTA: Time.timeScale NÃO é usado pois conflita
        // com as coroutines que usam WaitForSeconds customizado
    }

    /// <summary>
    /// Define velocidade reduzida (0.5x).
    ///
    /// Útil quando o jogador precisa de mais tempo para
    /// reagir a eventos sem pausar completamente.
    ///
    /// Ações:
    /// - Retoma todas as animações
    /// - Remove flags de pausa
    /// - Define SpeedGame = 0.5
    /// - Desabilita botão 0.5x (já ativo)
    /// </summary>
    public void SpeedTimesHalfButton()
    {
        // Retoma sistema de jogo
        GameManager.PlayAllAnimations();
        ClockBehaviour.PauseEvent = false;
        gameManager.Pause = false;

        // Define velocidade reduzida
        SpeedGame = 0.5f;

        // Atualiza estado dos botões
        halfButton.interactable = false;
        twoTimesButton.interactable = true;
        playButton.interactable = true;
        pauseButton.interactable = true;

        // NOTA: Time.timeScale NÃO é usado pois conflita
        // com as coroutines que usam WaitForSeconds customizado
    }

    #endregion
}
