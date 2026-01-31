using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Gerenciador do sistema econômico do jogo.
///
/// Responsabilidades:
/// - Controlar o dinheiro do jogador
/// - Atualizar display de dinheiro na UI
/// - Calcular ganhos diários baseados em personagens saudáveis
///
/// Economia do jogo:
/// - Dinheiro inicial: R$ 400
/// - Ganho diário: R$ 10 por personagem saudável
/// - Custos: Tratamentos no Hospital, Laboratório e Centro de Saúde
///
/// Padrão: Singleton estático (acessível globalmente via MoneyManager.CurrentMoney)
/// </summary>
public class MoneyManager : MonoBehaviour
{
    #region Campos Estáticos

    /// <summary>
    /// Referência ao componente Text que exibe o dinheiro.
    /// </summary>
    private static Text display;

    /// <summary>
    /// Referência ao sistema de eventos de tempo.
    /// </summary>
    private static TimeEvent timeEvent;

    /// <summary>
    /// Valor do ganho diário baseado em personagens saudáveis.
    /// Calculado como: número de saudáveis × 10
    /// </summary>
    private static int againWithHealthy = 0;

    /// <summary>
    /// Dinheiro atual do jogador.
    /// </summary>
    private static int currentMoney;

    #endregion

    #region Propriedades

    /// <summary>
    /// Dinheiro atual do jogador.
    ///
    /// Quando alterado, atualiza automaticamente o display na UI.
    /// Formato: "R$ XXX"
    ///
    /// Pode ser acessado globalmente:
    /// MoneyManager.CurrentMoney += 100; // Adiciona dinheiro
    /// MoneyManager.CurrentMoney -= 50;  // Remove dinheiro
    /// </summary>
    public static int CurrentMoney
    {
        get
        {
            return currentMoney;
        }
        set
        {
            currentMoney = value;
            display.text = "R$ " + currentMoney.ToString();
        }
    }

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização precoce - obtém referências.
    /// </summary>
    private void Awake()
    {
        display = GetComponentInChildren<Text>();
        timeEvent = FindObjectOfType<TimeEvent>();
    }

    /// <summary>
    /// Inicialização - define dinheiro inicial e registra evento de fim de dia.
    ///
    /// Ao final de cada dia, o jogador recebe dinheiro baseado no número
    /// de personagens saudáveis na cidade.
    /// </summary>
    private void Start()
    {
        // Define dinheiro inicial
        CurrentMoney = 400;

        // Registra evento para receber dinheiro ao fim de cada dia
        timeEvent.actionEndDay += () => UpdateMoney();
    }

    #endregion

    #region Métodos de Economia

    /// <summary>
    /// Atualiza o valor do ganho diário baseado em personagens saudáveis.
    ///
    /// Fórmula: ganho = número de saudáveis × 10
    ///
    /// Chamado pela HUD quando o número de saudáveis muda.
    /// Representa a "contribuição" da população saudável para a economia.
    /// </summary>
    /// <param name="numberHealthy">Número de personagens saudáveis</param>
    /// <returns>Valor calculado do ganho diário</returns>
    public static int UpdateAgainWithHealthy(int numberHealthy)
    {
        int againValue = numberHealthy * 10;
        againWithHealthy = againValue;

        return againValue;
    }

    /// <summary>
    /// Processa o ganho de dinheiro ao final do dia.
    ///
    /// Adiciona ao dinheiro do jogador o valor calculado por
    /// UpdateAgainWithHealthy (R$ 10 por personagem saudável).
    ///
    /// Este método incentiva o jogador a manter a população saudável,
    /// criando um ciclo de feedback positivo na economia do jogo.
    /// </summary>
    private static void UpdateMoney()
    {
        CurrentMoney += againWithHealthy;
    }

    #endregion
}
