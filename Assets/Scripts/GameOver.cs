using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistema de condições de vitória e derrota do jogo.
///
/// Responsabilidades:
/// - Verificar se o jogador venceu
/// - Verificar se o jogador perdeu
/// - Exibir painéis de vitória/derrota
///
/// Condições:
/// VITÓRIA:
/// - Todos os personagens estão saudáveis (Health == Healthy)
/// - Todos os personagens possuem todas as vacinas disponíveis
///
/// DERROTA:
/// - Todos os personagens morreram (Health == Dead)
///
/// Nota: As verificações são habilitadas apenas após o spawn
/// dos personagens (EnableWinLose == true) para evitar
/// falsas detecções durante a inicialização.
/// </summary>
public class GameOver : MonoBehaviour
{
    #region Referências

    /// <summary>
    /// Gerenciador de personagens - para acessar lista de personagens.
    /// </summary>
    [SerializeField]
    private CharactersManager charactersManager;

    /// <summary>
    /// Gerenciador de vacinas - para saber quantas vacinas existem no jogo.
    /// </summary>
    [SerializeField]
    private DataVaccineManager vaccineManager;

    [Space]
    /// <summary>
    /// Painel exibido quando o jogador vence.
    /// </summary>
    [SerializeField]
    private WinPanel winPanel;

    /// <summary>
    /// Painel exibido quando o jogador perde.
    /// </summary>
    [SerializeField]
    private LosePanel losePanel;

    #endregion

    #region Propriedades

    /// <summary>
    /// Flag que habilita a verificação de vitória/derrota.
    ///
    /// Definido como true pelo CharactersManager após o spawn
    /// de todos os personagens. Isso previne falsas detecções
    /// durante a fase de inicialização do jogo.
    /// </summary>
    public bool EnableWinLose { get; set; } = false;

    #endregion

    #region Verificações de Fim de Jogo

    /// <summary>
    /// Verifica se o jogador venceu o jogo.
    ///
    /// Condições de vitória (todas devem ser verdadeiras):
    /// 1. EnableWinLose == true (jogo iniciado)
    /// 2. Todos os personagens spawned
    /// 3. Todos os personagens estão saudáveis
    /// 4. Todos os personagens possuem todas as vacinas
    ///
    /// Chamado pela HUD quando o estado de saúde muda.
    /// </summary>
    public void CheckWin()
    {
        // Verifica se a verificação está habilitada
        if (!EnableWinLose)
            return;

        // Verifica se todos os personagens foram spawned
        if (charactersManager.Characters.Length < charactersManager.MaxCharactersinGame())
            return;

        bool isWin = true;

        // Verifica cada personagem
        foreach (var character in charactersManager.Characters)
        {
            if (character.VaccinesTaken != null)
            {
                // Verifica se tem todas as vacinas E está saudável
                if (character.VaccinesTaken.Count < vaccineManager.vaccines.Length ||
                    character.Health != HealthCondition.Healthy)
                {
                    isWin = false;
                    break;
                }
            }
        }

        // Se venceu, mostra painel de vitória
        if (isWin)
            winPanel.Show();
    }

    /// <summary>
    /// Verifica se o jogador perdeu o jogo.
    ///
    /// Condição de derrota:
    /// - Todos os personagens estão mortos (Health == Dead)
    ///
    /// Chamado pela HUD quando o estado de saúde muda.
    /// </summary>
    public void CheckLose()
    {
        bool isLose = true;

        // Verifica se algum personagem ainda está vivo
        foreach (var character in charactersManager.Characters)
        {
            if (character.Health != HealthCondition.Dead)
            {
                isLose = false;
                break;
            }
        }

        // Se perdeu, mostra painel de derrota
        if (isLose)
            losePanel.Show();
    }

    #endregion
}
