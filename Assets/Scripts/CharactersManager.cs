using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerenciador central de personagens (NPCs) do jogo.
///
/// Responsabilidades principais:
/// - Criar (spawn) personagens no início do jogo
/// - Manter cache de todos os personagens para acesso eficiente
/// - Fornecer listas filtradas por estado de saúde
/// - Gerenciar referências às IAs de movimento
///
/// Performance:
/// Este componente usa cache para evitar chamadas repetidas a
/// GetComponentsInChildren, que é custoso em termos de performance.
/// O cache é atualizado apenas quando necessário (após spawn).
///
/// Hierarquia esperada:
/// CharactersManager (este script)
/// └── Character_0 (prefab instanciado)
///     └── IA (componente de IA)
///     └── Body (visual)
///         └── CharacterStatus (status de saúde)
/// └── Character_1
/// └── Character_N...
/// </summary>
public class CharactersManager : MonoBehaviour
{
    #region Configurações

    /// <summary>
    /// Número de personagens a criar no início do jogo.
    /// Quanto mais personagens, mais complexa a simulação de epidemia.
    /// </summary>
    [SerializeField]
    private int numberCharactersInGame = 10;

    /// <summary>
    /// Prefab do personagem a ser instanciado.
    /// Deve conter: IA, NavMeshAgent, CharacterStatus, BodyIA, etc.
    /// </summary>
    [SerializeField]
    private GameObject prefCharacter = null;

    #endregion

    #region Referências de Sistemas

    [Space]
    /// <summary>
    /// Componente de Game Over - habilitado após spawn dos personagens.
    /// </summary>
    [SerializeField]
    private GameOver gameOver = null;

    /// <summary>
    /// HUD - atualizada com contadores após spawn.
    /// </summary>
    [SerializeField]
    private HUD hud = null;

    /// <summary>
    /// Sistema de rotas globais - contém pontos de spawn e navegação.
    /// </summary>
    [SerializeField]
    private GlobalRoute globalRoute = null;

    #endregion

    #region Campos Privados

    /// <summary>
    /// Array de pontos de rota disponíveis para spawn.
    /// Obtido do GlobalRoute no Awake.
    /// </summary>
    private PointRoute[] points = null;

    /// <summary>
    /// Cache de todos os CharacterStatus.
    /// Evita chamadas repetidas a GetComponentsInChildren.
    /// </summary>
    private CharacterStatus[] cachedCharacters = null;

    /// <summary>
    /// Array de todas as IAs de personagens.
    /// Usado pelo SpeedManager para atualizar velocidades.
    /// </summary>
    private IA[] ias;

    #endregion

    #region Propriedades Públicas

    /// <summary>
    /// Array de IAs de todos os personagens.
    /// Usado pelo SpeedManager para atualizar velocidade de movimento.
    /// </summary>
    public IA[] IAs
    {
        get
        {
            return ias;
        }
    }

    /// <summary>
    /// Array de todos os personagens (cache).
    /// Atualiza automaticamente se o cache estiver vazio.
    /// </summary>
    public CharacterStatus[] Characters
    {
        get
        {
            if (cachedCharacters == null)
            {
                RefreshCharacterCache();
            }
            return cachedCharacters;
        }
    }

    /// <summary>
    /// Array de personagens saudáveis.
    /// Filtrado em tempo real a partir do cache.
    /// </summary>
    public CharacterStatus[] CharactersHealthy
    {
        get
        {
            List<CharacterStatus> result = new List<CharacterStatus>();
            foreach (var c in Characters)
            {
                if (c.Health == HealthCondition.Healthy)
                    result.Add(c);
            }
            return result.ToArray();
        }
    }

    /// <summary>
    /// Array de personagens doentes.
    /// Filtrado em tempo real a partir do cache.
    /// </summary>
    public CharacterStatus[] CharactersSick
    {
        get
        {
            List<CharacterStatus> result = new List<CharacterStatus>();
            foreach (var c in Characters)
            {
                if (c.Health == HealthCondition.Sick)
                    result.Add(c);
            }
            return result.ToArray();
        }
    }

    /// <summary>
    /// Array de personagens mortos.
    /// Filtrado em tempo real a partir do cache.
    /// </summary>
    public CharacterStatus[] CharactersDead
    {
        get
        {
            List<CharacterStatus> result = new List<CharacterStatus>();
            foreach (var c in Characters)
            {
                if (c.Health == HealthCondition.Dead)
                    result.Add(c);
            }
            return result.ToArray();
        }
    }

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização precoce - obtém pontos de spawn do GlobalRoute.
    /// </summary>
    private void Awake()
    {
        points = globalRoute.GetComponentsInChildren<PointRoute>();
    }

    /// <summary>
    /// Inicialização - cria os personagens.
    /// </summary>
    private void Start()
    {
        SpawnCharacters();
    }

    #endregion

    #region Métodos Públicos

    /// <summary>
    /// Retorna o número máximo de personagens configurado.
    /// </summary>
    /// <returns>Número de personagens no jogo</returns>
    public int MaxCharactersinGame()
    {
        return numberCharactersInGame;
    }

    /// <summary>
    /// Atualiza o cache de personagens.
    /// Chamado após spawn ou quando necessário reconstruir o cache.
    /// </summary>
    public void RefreshCharacterCache()
    {
        cachedCharacters = GetComponentsInChildren<CharacterStatus>();
    }

    #endregion

    #region Métodos Privados

    /// <summary>
    /// Cria todos os personagens do jogo.
    ///
    /// Processo:
    /// 1. Instancia N cópias do prefab como filhos deste objeto
    /// 2. Nomeia cada personagem sequencialmente (Character_0, Character_1, etc.)
    /// 3. Posiciona cada personagem em um ponto de rota aleatório
    /// 4. Atualiza o cache de personagens e IAs
    /// 5. Atualiza contadores na HUD
    /// 6. Habilita condições de vitória/derrota
    /// </summary>
    private void SpawnCharacters()
    {
        // Instancia personagens
        for (int i = 0; i < numberCharactersInGame; i++)
        {
            // Cria instância como filho deste objeto
            GameObject characterCurrent = Instantiate(prefCharacter, transform);
            characterCurrent.name = "Character_" + i;

            // Posiciona em ponto aleatório
            characterCurrent.transform.position = points[Random.Range(0, points.Length - 1)].transform.position;
        }

        // Atualiza caches após spawn
        RefreshCharacterCache();
        ias = GetComponentsInChildren<IA>();

        // Atualiza contadores na HUD
        hud.UpdateHealthy();
        hud.UpdateSick();
        hud.UpdateDead();

        // Habilita verificação de condições de vitória/derrota
        gameOver.EnableWinLose = true;
    }

    #endregion
}
