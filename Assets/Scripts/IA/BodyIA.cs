using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Componente visual e de interação do corpo de um personagem.
///
/// Responsabilidades:
/// - Exibir círculo visual ao redor do personagem
/// - Detectar colisões com outros personagens (transmissão de doenças)
/// - Responder a cliques para selecionar o personagem
/// - Manter referência à IA pai
///
/// Sistema de transmissão interpessoal:
/// Quando um personagem DOENTE entra em contato com um SAUDÁVEL,
/// há chance de transmissão da doença baseada em:
/// - Transmissibilidade da doença
/// - Sistema imunológico da vítima
/// - Vacinas que a vítima possui
///
/// Hierarquia esperada:
/// IA (pai)
/// └── Body (este script)
///     └── CharacterStatus
///     └── SphereCollider (trigger para detecção)
///     └── LineRenderer (círculo visual)
/// </summary>
[RequireComponent(typeof(CharacterStatus))]
public class BodyIA : MonoBehaviour
{
    #region Configurações Visuais

    /// <summary>
    /// Raio do círculo visual ao redor do personagem.
    /// </summary>
    [SerializeField]
    [Range(0, 100)]
    private int radius = 2;

    /// <summary>
    /// Número de segmentos do círculo (mais = mais suave).
    /// </summary>
    [SerializeField]
    [Range(0, 100)]
    private int segments = 40;

    /// <summary>
    /// Largura da linha do círculo.
    /// </summary>
    [SerializeField]
    [Range(0, 10)]
    private float widhtLine = 4f;

    /// <summary>
    /// Deslocamento do círculo em relação ao centro do personagem.
    /// </summary>
    [SerializeField]
    private Vector3 offsetCircle = Vector3.zero;

    #endregion

    #region Componentes

    /// <summary>
    /// LineRenderer para desenhar o círculo visual.
    /// </summary>
    private LineRenderer line;

    /// <summary>
    /// Collider esférico para detecção de proximidade.
    /// </summary>
    private SphereCollider sphere;

    /// <summary>
    /// Status de saúde deste personagem.
    /// </summary>
    private CharacterStatus characterStatus;

    #endregion

    #region Referências de Sistemas

    /// <summary>
    /// Comportamento da câmera - para focar neste personagem quando clicado.
    /// </summary>
    private CameraBehaviour cameraBehaviour;

    /// <summary>
    /// Sistema de tempo - para verificar limite diário de transmissões.
    /// </summary>
    private ClockBehaviour clock;

    /// <summary>
    /// Painel de alertas para notificações de transmissão.
    /// </summary>
    private AlertPanel alertPanel;

    #endregion

    #region Propriedades

    /// <summary>
    /// Referência à IA pai que controla este personagem.
    /// Usado por outros sistemas para acessar a movimentação.
    /// </summary>
    public IA IA { get; private set; }

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização precoce - obtém componentes e referências.
    /// </summary>
    private void Awake()
    {
        // Obtém referências globais
        alertPanel = FindObjectOfType<AlertPanel>();
        cameraBehaviour = FindObjectOfType<CameraBehaviour>();
        clock = FindObjectOfType<ClockBehaviour>();

        // Obtém componentes locais
        characterStatus = GetComponent<CharacterStatus>();
        line = GetComponent<LineRenderer>();
        sphere = GetComponent<SphereCollider>();

        // Obtém referência à IA pai
        IA = GetComponentInParent<IA>();
    }

    /// <summary>
    /// Inicialização - configura componentes visuais e collider.
    /// </summary>
    private void Start()
    {
        // Configura LineRenderer
        line.positionCount = segments + 1;
        line.startWidth = widhtLine;
        line.endWidth = widhtLine;
        line.useWorldSpace = false;

        // Configura collider de detecção
        sphere.radius = radius;

        // Desenha círculo visual
        SpawnCircle();
    }

    #endregion

    #region Interação

    /// <summary>
    /// Responde ao clique do jogador no personagem.
    /// Faz a câmera focar neste personagem (se não pausado).
    /// </summary>
    private void OnMouseDown()
    {
        if (!GameManager.Instance.Pause)
            cameraBehaviour.SetTarget(transform);
    }

    #endregion

    #region Transmissão de Doenças

    /// <summary>
    /// Detecta colisão com outro personagem e processa transmissão.
    ///
    /// Sistema de transmissão interpessoal:
    /// 1. Verifica se ESTE personagem está doente
    /// 2. Verifica se o OUTRO personagem está saudável
    /// 3. Verifica limite diário de transmissões
    /// 4. Calcula chance: transmissibilidade - sistema imune
    /// 5. Verifica se a vítima tem vacina contra a doença
    /// 6. Se passar em todas as verificações, transmite a doença
    /// </summary>
    /// <param name="other">Collider do outro personagem</param>
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se ESTE personagem está doente e colidiu com Body_IA
        if (characterStatus.Health == HealthCondition.Sick && other.tag == "Body_IA")
        {
            CharacterStatus victimCharacter = other.GetComponent<CharacterStatus>();

            // Verifica se a vítima está saudável
            if (victimCharacter.Health != HealthCondition.Healthy)
                return;

            // Verifica limite diário de transmissões
            if (CharacterStatus.NumberAventsSicknessInday >= clock.CurrentDay)
                return;

            // Calcula chance de infecção
            // Fórmula: transmissibilidade da doença - sistema imune da vítima
            int percentGetSick = characterStatus.SicknessGot.transmissibility - victimCharacter.ImuneSystem;

            // Rola a chance
            if (UnityEngine.Random.Range(0, 100) <= percentGetSick)
            {
                // Verifica se a vítima tem vacina contra esta doença
                foreach (var vaccinesTaken in victimCharacter.VaccinesTaken)
                {
                    for (int i = 0; i < vaccinesTaken.prevents.Length; i++)
                    {
                        if (characterStatus.SicknessGot == vaccinesTaken.prevents[i])
                        {
                            // Vítima é imune - não transmite
                            return;
                        }
                    }
                }

                // Transmite a doença para a vítima
                victimCharacter.ChangeHealthConditionSick(characterStatus.SicknessGot);

                // Cria alerta de transmissão interpessoal
                alertPanel.SpawnAlertInterpersonalInfection(victimCharacter, GetComponent<CharacterStatus>());

                // Incrementa contador diário de transmissões
                CharacterStatus.NumberAventsSicknessInday++;
            }
        }
    }

    #endregion

    #region Geração Visual

    /// <summary>
    /// Desenha o círculo visual ao redor do personagem.
    ///
    /// Cria pontos em intervalos angulares regulares
    /// formando um polígono que aproxima um círculo.
    /// </summary>
    private void SpawnCircle()
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;

        float angle = 20f;

        // Gera pontos do círculo
        for (int i = 0; i < (segments + 1); i++)
        {
            // Calcula posição usando trigonometria
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            // Define posição do ponto no LineRenderer
            line.SetPosition(i, new Vector3(
                x + offsetCircle.x,
                y + offsetCircle.y,
                z + offsetCircle.z
            ));

            // Avança o ângulo para o próximo ponto
            angle += (360f / segments);
        }
    }

    #endregion
}
