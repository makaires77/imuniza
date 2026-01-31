using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controlador de inteligência artificial de um personagem (NPC).
///
/// Responsabilidades:
/// - Controlar a movimentação do personagem usando NavMesh
/// - Alternar entre caminhada aleatória e destino específico
/// - Ajustar velocidade baseado no SpeedManager
///
/// Comportamentos:
/// 1. CAMINHADA ALEATÓRIA: O personagem caminha entre pontos de rota
///    conectados, simulando uma população em movimento.
///
/// 2. DESTINO ESPECÍFICO: Quando direcionado (ex: para tratamento),
///    o personagem vai até o destino e depois retorna à caminhada aleatória.
///
/// Hierarquia esperada:
/// IA (este script) ← GameObject pai
/// └── Body (visual + NavMeshAgent)
///     └── CharacterStatus
/// </summary>
public class IA : MonoBehaviour
{
    #region Componentes

    /// <summary>
    /// NavMeshAgent que controla o movimento físico.
    /// Encontrado no GameObject filho "Body".
    /// </summary>
    private NavMeshAgent body;

    /// <summary>
    /// Status de saúde do personagem.
    /// </summary>
    private CharacterStatus characterStatus;

    /// <summary>
    /// Animator para controlar animações de caminhada.
    /// </summary>
    private Animator animator;

    #endregion

    #region Referências de Sistemas

    /// <summary>
    /// Sistema de rotas globais - contém pontos de navegação.
    /// </summary>
    private GlobalRoute route;

    /// <summary>
    /// Ponto de rota atual onde o personagem está ou se dirige.
    /// </summary>
    private PointRoute currentPoint;

    /// <summary>
    /// Painel de alertas para notificações.
    /// </summary>
    private AlertPanel alertPanel;

    /// <summary>
    /// Painel de ações para interação com personagem.
    /// </summary>
    private ActionPanel actionpanel;

    #endregion

    #region Campos de Estado

    /// <summary>
    /// Indica se está em modo de caminhada aleatória.
    /// False quando está indo para um destino específico.
    /// </summary>
    private bool followRandomPath = true;

    /// <summary>
    /// Referência ao destino atual (Hospital, Laboratório, etc).
    /// Usado para chamar TreatPatient quando chegar.
    /// </summary>
    private IRouteGlobal routeTarget;

    /// <summary>
    /// Velocidade padrão do NavMeshAgent.
    /// Salva no Start para cálculos de velocidade variável.
    /// </summary>
    private float speedDefault = 4;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização precoce - obtém componentes e referências.
    /// </summary>
    private void Awake()
    {
        // Obtém componentes nos filhos
        body = transform.GetComponentInChildren<NavMeshAgent>(true);
        animator = GetComponentInChildren<Animator>(true);
        characterStatus = transform.GetComponentInChildren<CharacterStatus>(true);

        // Obtém referências globais
        route = FindObjectOfType<GlobalRoute>();
        actionpanel = FindObjectOfType<ActionPanel>();
        alertPanel = FindObjectOfType<AlertPanel>();
    }

    /// <summary>
    /// Inicialização - posiciona personagem e inicia caminhada.
    /// </summary>
    private void Start()
    {
        // Salva velocidade padrão
        speedDefault = body.speed;

        // Encontra ponto de rota mais próximo
        currentPoint = route.GetPointCloserSidewalk(transform.position);

        // Inicia em modo de caminhada aleatória
        followRandomPath = true;

        // Teleporta para o ponto de rota (evita problemas de NavMesh)
        body.Warp(currentPoint.transform.position);

        // Inicia caminhada
        NextWaypointRandomPath();
    }

    /// <summary>
    /// Atualização - verifica se chegou ao destino para continuar caminhando.
    /// </summary>
    private void Update()
    {
        // Verifica se chegou ao destino
        // Condições: sem path pendente, perto do destino, não parado, sem atividade especial
        if (!body.pathPending &&
            body.remainingDistance < 1 &&
            !body.isStopped &&
            characterStatus.Activity == Activity.None)
        {
            NextWaypointRandomPath();
        }
    }

    #endregion

    #region Navegação

    /// <summary>
    /// Processa a chegada ao waypoint atual e define o próximo.
    ///
    /// Comportamento:
    /// - Se estava indo para um destino específico: chama TreatPatient
    /// - Se está em caminhada aleatória: escolhe próximo ponto aleatório
    /// </summary>
    private void NextWaypointRandomPath()
    {
        if (!followRandomPath)
        {
            // Chegou ao destino específico
            followRandomPath = true;

            // Atualiza ponto atual para o mais próximo
            currentPoint = route.GetPointCloserSidewalk(transform.position);

            // Chama método de tratamento do destino (Hospital, etc)
            routeTarget.TreatPatient(body.transform);
        }
        else
        {
            // Caminhada aleatória - escolhe próximo ponto conectado
            currentPoint = currentPoint.GetRandomPoint();
            body.SetDestination(currentPoint.transform.position);
        }
    }

    /// <summary>
    /// Define um destino específico para o personagem.
    ///
    /// Usado quando o jogador envia o personagem para tratamento.
    /// O personagem caminha até o destino e, ao chegar,
    /// TreatPatient é chamado automaticamente.
    /// </summary>
    /// <param name="target">Transform do destino (Hospital, Laboratório, etc)</param>
    public void NextWaypointTarget(Transform target)
    {
        // Reseta a posição atual (corrige bugs de NavMesh)
        body.Warp(body.transform.position);

        Debug.Log($"Set Waypoint for {gameObject.name}");

        // Define destino no NavMeshAgent
        body.SetDestination(target.position);

        // Salva referência ao destino para chamar TreatPatient depois
        routeTarget = target.GetComponent<IRouteGlobal>();

        // Desativa modo de caminhada aleatória
        followRandomPath = false;
    }

    /// <summary>
    /// Atualiza a velocidade do personagem baseado no multiplicador global.
    ///
    /// Chamado pelo SpeedManager quando a velocidade do jogo muda.
    /// Ajusta tanto a velocidade do NavMeshAgent quanto a animação.
    /// </summary>
    /// <param name="speedNew">Multiplicador de velocidade (0.5, 1.0, 2.0)</param>
    public void SpeedUpdate(float speedNew)
    {
        // Atualiza velocidade do NavMeshAgent
        body.speed = speedDefault * speedNew;

        // Atualiza velocidade da animação
        animator.SetFloat("SpeedAnimation", speedNew);
    }

    #endregion
}
