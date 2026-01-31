using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistema central de gerenciamento de eventos baseados em tempo.
///
/// Este componente é responsável por agendar e executar ações em momentos específicos
/// do ciclo de tempo do jogo. Permite agendar eventos que ocorrem:
/// - A cada segundo do jogo
/// - No final de cada dia
/// - Em um segundo específico todos os dias (ex: verificação de morte)
/// - Em um dia e segundo específicos (ex: fim de tratamento hospitalar)
///
/// Utilizado por: ClockBehaviour, CharacterStatus, Hospital, Laboratory, HealthCenter
/// </summary>
public class TimeEvent : MonoBehaviour
{
    #region Delegates

    /// <summary>
    /// Delegate para ações do relógio que retornam sucesso/falha.
    /// Retorna true se a ação foi executada com sucesso.
    /// </summary>
    public delegate bool ActionClockDelegate();

    /// <summary>
    /// Delegate para ações executadas no final de cada dia.
    /// Usado para atualizar economia, estatísticas, etc.
    /// </summary>
    public delegate void ActionEndDay();

    /// <summary>
    /// Delegate para ações executadas a cada segundo do jogo.
    /// Usado para atualizações contínuas como barras de progresso.
    /// </summary>
    public delegate void ActionPerSecond();

    #endregion

    #region Dicionários de Eventos

    /// <summary>
    /// Eventos que ocorrem em um segundo específico TODOS os dias.
    /// Chave: segundo do dia (0 a totalDayTime)
    /// Valor: delegate com as ações a executar
    /// Exemplo: Verificação diária de morte de personagens doentes
    /// </summary>
    public Dictionary<int, ActionClockDelegate> actionInSecEveryDay = new Dictionary<int, ActionClockDelegate>();

    /// <summary>
    /// Eventos que ocorrem em um dia E segundo específicos (uma única vez).
    /// Chave externa: número do dia
    /// Chave interna: segundo do dia
    /// Valor: delegate com as ações a executar
    /// Exemplo: Fim de tratamento hospitalar no dia 5, segundo 100
    /// </summary>
    public Dictionary<int, Dictionary<int, ActionClockDelegate>> actionInDaySecundSpecific = new Dictionary<int, Dictionary<int, ActionClockDelegate>>();

    #endregion

    #region Eventos Públicos

    /// <summary>
    /// Evento disparado a cada segundo do jogo.
    /// Inscreva-se para receber atualizações contínuas.
    /// </summary>
    public ActionPerSecond actionPerSecond;

    /// <summary>
    /// Evento disparado ao final de cada dia.
    /// Usado para cálculos de economia e estatísticas diárias.
    /// </summary>
    public ActionEndDay actionEndDay;

    #endregion

    #region Métodos de Registro - Eventos por Segundo

    /// <summary>
    /// Adiciona uma ação para ser executada a cada segundo.
    /// </summary>
    /// <param name="action">Referência ao delegate da ação</param>
    public void AddActionPerSec(ref ActionPerSecond action)
    {
        actionPerSecond += action;
    }

    /// <summary>
    /// Remove uma ação do evento por segundo.
    /// </summary>
    /// <param name="action">Referência ao delegate da ação</param>
    public void RemoveActionPerSec(ref ActionPerSecond action)
    {
        actionPerSecond -= action;
    }

    #endregion

    #region Métodos de Registro - Eventos Diários Recorrentes

    /// <summary>
    /// Adiciona uma ação para ser executada em um segundo específico TODOS os dias.
    /// Útil para verificações periódicas como chance de morte de personagens doentes.
    /// </summary>
    /// <param name="action">Referência ao delegate da ação</param>
    /// <param name="sec">Segundo do dia em que a ação será executada (0 a totalDayTime)</param>
    public void AddActionEveryDay(ref ActionClockDelegate action, int sec)
    {
        if (actionInSecEveryDay.ContainsKey(sec))
            actionInSecEveryDay[sec] += action;
        else
            actionInSecEveryDay.Add(sec, action);
    }

    /// <summary>
    /// Remove uma ação do evento diário recorrente.
    /// Chamado quando um personagem é curado ou morre.
    /// </summary>
    /// <param name="action">Referência ao delegate da ação</param>
    /// <param name="sec">Segundo do dia onde a ação estava registrada</param>
    public void RemoveActionEveryDay(ref ActionClockDelegate action, int sec)
    {
        if (actionInSecEveryDay.ContainsKey(sec))
        {
            actionInSecEveryDay[sec] -= action;

            // Remove a entrada se não houver mais ações neste segundo
            if (actionInSecEveryDay[sec] == null)
                actionInSecEveryDay.Remove(sec);
        }
    }

    /// <summary>
    /// Alias para AddActionEveryDay - adiciona ação em segundo específico.
    /// </summary>
    public void AddActionInSpecificSec(ref ActionClockDelegate action, int sec)
    {
        if (actionInSecEveryDay.ContainsKey(sec))
            actionInSecEveryDay[sec] += action;
        else
            actionInSecEveryDay.Add(sec, action);
    }

    /// <summary>
    /// Alias para RemoveActionEveryDay - remove ação de segundo específico.
    /// </summary>
    public void RemovedActionInSpecificSec(ref ActionClockDelegate action, int sec)
    {
        if (actionInSecEveryDay.ContainsKey(sec))
            actionInSecEveryDay[sec] -= action;
    }

    #endregion

    #region Métodos de Registro - Eventos Únicos (Dia + Segundo)

    /// <summary>
    /// Agenda uma ação para ser executada uma única vez em um dia e segundo específicos.
    /// Ideal para eventos programados como fim de tratamentos médicos.
    ///
    /// Exemplo de uso:
    /// - Paciente entra no hospital no dia 1, segundo 50
    /// - Tratamento dura 4 dias
    /// - Agenda cura para dia 5, segundo 50
    /// </summary>
    /// <param name="action">Delegate da ação a executar</param>
    /// <param name="sec">Segundo do dia</param>
    /// <param name="day">Número do dia</param>
    public void AddActionInSpecificSecAndDay(ActionClockDelegate action, int sec, int day)
    {
        // Verifica se já existe um dicionário para este dia
        if (actionInDaySecundSpecific.ContainsKey(day))
        {
            // Dia existe - verifica se o segundo já tem ações
            if (actionInDaySecundSpecific[day] != null && actionInDaySecundSpecific[day].ContainsKey(sec))
            {
                // Adiciona à lista de ações existente neste segundo
                actionInDaySecundSpecific[day][sec] += action;
            }
            else if (actionInDaySecundSpecific[day] == null)
            {
                // Dicionário do dia é null - recria
                actionInDaySecundSpecific[day] = new Dictionary<int, ActionClockDelegate>();
                actionInDaySecundSpecific[day].Add(sec, action);
            }
            else
            {
                // Segundo não existe ainda - cria nova entrada
                actionInDaySecundSpecific[day].Add(sec, action);
            }
        }
        else
        {
            // Dia não existe - cria estrutura completa
            actionInDaySecundSpecific.Add(day, new Dictionary<int, ActionClockDelegate>());
            actionInDaySecundSpecific[day].Add(sec, action);
        }
    }

    #endregion

    #region Métodos de Limpeza

    /// <summary>
    /// Remove todos os eventos agendados para dias anteriores ao dia atual.
    /// IMPORTANTE: Previne memory leak causado pelo acúmulo de eventos passados.
    /// Deve ser chamado ao início de cada novo dia.
    /// </summary>
    /// <param name="currentDay">Número do dia atual</param>
    public void CleanupPastDays(int currentDay)
    {
        // Coleta dias a remover (não pode modificar dicionário durante iteração)
        List<int> daysToRemove = new List<int>();

        foreach (int day in actionInDaySecundSpecific.Keys)
        {
            if (day < currentDay)
            {
                daysToRemove.Add(day);
            }
        }

        // Remove os dias passados
        foreach (int day in daysToRemove)
        {
            actionInDaySecundSpecific.Remove(day);
        }
    }

    /// <summary>
    /// Limpa as ações de um segundo específico após sua execução.
    /// Chamado pelo ClockBehaviour imediatamente após executar as ações.
    /// </summary>
    /// <param name="day">Número do dia</param>
    /// <param name="second">Segundo do dia</param>
    public void ClearDaySecondActions(int day, int second)
    {
        if (actionInDaySecundSpecific.TryGetValue(day, out var secDict))
        {
            secDict.Remove(second);

            // Remove o dia inteiro se não houver mais eventos
            if (secDict.Count == 0)
            {
                actionInDaySecundSpecific.Remove(day);
            }
        }
    }

    #endregion
}
