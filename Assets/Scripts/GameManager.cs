using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Gerenciador principal do jogo - controla estados globais e pausa.
///
/// Responsabilidades:
/// - Gerenciar estado de pausa do jogo
/// - Controlar animações de todos os objetos na cena
/// - Parar/retomar movimento dos personagens (NavMeshAgent)
///
/// Padrão: Singleton (acesso via GameManager.Instance)
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Campos Estáticos

    /// <summary>
    /// Cache de todos os Animators na cena.
    /// Populado no Start() para evitar buscas repetidas.
    /// </summary>
    private static Animator[] animationsInScene;

    /// <summary>
    /// Velocidades originais de cada Animator.
    /// Usado para restaurar após despausar.
    /// </summary>
    private static List<float> prevSpeedAnim;

    #endregion

    #region Referências Serializadas

    /// <summary>
    /// Referência ao gerenciador de personagens.
    /// Usado para pausar/retomar movimento dos NPCs.
    /// </summary>
    [SerializeField] private CharactersManager characterManager;

    /// <summary>
    /// Referência ao gerenciador de velocidade do jogo.
    /// </summary>
    [SerializeField] private SpeedManager speedManager;

    #endregion

    #region Singleton

    private static GameManager instance;

    /// <summary>
    /// Instância única do GameManager (Singleton).
    /// Usa FindObjectOfType como fallback se não estiver definido.
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            if (!instance)
                instance = FindObjectOfType<GameManager>();

            return instance;
        }
    }

    #endregion

    #region Campos de Estado

    /// <summary>
    /// Velocidade do jogo antes de pausar.
    /// Usado para restaurar a velocidade correta ao despausar.
    /// </summary>
    private float previewSpeedIAs = 1f;

    /// <summary>
    /// Estado interno de pausa do jogo.
    /// </summary>
    private bool pause = false;

    #endregion

    #region Propriedades

    /// <summary>
    /// Estado de pausa do jogo.
    ///
    /// Quando alterado para TRUE:
    /// - Para todas as animações (speed = 0)
    /// - Para todos os NavMeshAgents (isStopped = true)
    /// - Salva e zera a velocidade do jogo
    ///
    /// Quando alterado para FALSE:
    /// - Restaura velocidades originais das animações
    /// - Retoma NavMeshAgents de personagens vivos
    /// - Restaura velocidade do jogo
    /// </summary>
    public bool Pause
    {
        get { return pause; }
        set
        {
            // Só processa se o valor realmente mudou
            if (value != pause)
            {
                pause = value;

                if (pause)
                {
                    // PAUSAR JOGO

                    // Para todas as animações
                    for (int i = 0; i < animationsInScene.Length; i++)
                        animationsInScene[i].speed = 0;

                    // Para todos os personagens
                    for (int i = 0; i < characterManager.Characters.Length; i++)
                        characterManager.Characters[i].GetComponent<NavMeshAgent>().isStopped = true;

                    // Salva e zera velocidade
                    previewSpeedIAs = speedManager.SpeedGame;
                    speedManager.SpeedGame = 0;
                }
                else
                {
                    // RETOMAR JOGO

                    // Restaura velocidades das animações
                    for (int i = 0; i < animationsInScene.Length; i++)
                        animationsInScene[i].speed = prevSpeedAnim[i];

                    // Retoma apenas personagens vivos (não mortos)
                    for (int i = 0; i < characterManager.Characters.Length; i++)
                    {
                        var health = characterManager.Characters[i].Health;
                        if (health == HealthCondition.Healthy || health == HealthCondition.Sick)
                            characterManager.Characters[i].GetComponent<NavMeshAgent>().isStopped = false;
                    }

                    // Restaura velocidade do jogo
                    speedManager.SpeedGame = previewSpeedIAs;
                }
            }
        }
    }

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização do GameManager.
    /// Cacheia todos os Animators e suas velocidades originais.
    /// </summary>
    private void Start()
    {
        // Encontra todos os Animators na cena
        animationsInScene = FindObjectsOfType<Animator>();
        prevSpeedAnim = new List<float>();

        // Salva velocidade original de cada Animator
        for (int i = 0; i < animationsInScene.Length; i++)
        {
            prevSpeedAnim.Add(animationsInScene[i].speed);
        }
    }

    #endregion

    #region Métodos Estáticos de Animação

    /// <summary>
    /// Para todas as animações da cena imediatamente.
    /// Chamado pelo SpeedManager ao pausar o jogo.
    /// </summary>
    public static void PauseAllAnimations()
    {
        for (int i = 0; i < animationsInScene.Length; i++)
            animationsInScene[i].speed = 0;
    }

    /// <summary>
    /// Retoma todas as animações com suas velocidades originais.
    /// Chamado pelo SpeedManager ao despausar o jogo.
    /// </summary>
    public static void PlayAllAnimations()
    {
        for (int i = 0; i < animationsInScene.Length; i++)
            animationsInScene[i].speed = prevSpeedAnim[i];
    }

    #endregion
}
