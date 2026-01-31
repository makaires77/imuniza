using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zona de transmissão de doença no mundo do jogo.
///
/// Este componente representa uma área circular onde uma doença pode
/// se espalhar para personagens saudáveis que entrarem nela.
/// Usado para simular focos de infecção no ambiente.
///
/// Mecânicas:
/// - Área circular visual (LineRenderer) indicando zona de perigo
/// - Collider trigger para detectar personagens que entram na zona
/// - Transmissão baseada na transmissibilidade da doença vs. sistema imune
/// - Clique para ver informações da doença
///
/// Utilizado por:
/// - Focos iniciais de doença na cena
/// - DiseaseHumanPoint (corpos mortos que infectam)
/// </summary>
public class Disease : MonoBehaviour
{
    #region Configuração da Doença

    /// <summary>
    /// Dados da doença que esta zona transmite.
    /// ScriptableObject com transmissibilidade, letalidade, sintomas, etc.
    /// </summary>
    [SerializeField] private DataSickness sicknessData = null;

    /// <summary>
    /// Propriedade para acessar/modificar os dados da doença.
    /// </summary>
    public DataSickness Sickness
    {
        get { return sicknessData; }
        set { sicknessData = value; }
    }

    #endregion

    #region Configuração Visual do Círculo

    [Space]
    /// <summary>
    /// Raio da zona de infecção em unidades do jogo.
    /// Personagens dentro deste raio podem ser infectados.
    /// </summary>
    [SerializeField]
    [Range(0, 100)]
    private int radius = 40;

    /// <summary>
    /// Número de segmentos do círculo visual.
    /// Mais segmentos = círculo mais suave, mais custo de renderização.
    /// </summary>
    [SerializeField]
    [Range(0, 100)]
    private int segments = 40;

    /// <summary>
    /// Largura da linha do círculo visual.
    /// </summary>
    [SerializeField]
    [Range(0, 10)]
    private float widhtLine = 4f;

    /// <summary>
    /// Deslocamento do círculo em relação ao centro do objeto.
    /// Útil para ajustar posição visual.
    /// </summary>
    [SerializeField]
    private Vector3 offsetCircle = Vector3.zero;

    #endregion

    #region Configuração do Ícone

    [Space]
    /// <summary>
    /// Sprite que exibe o ícone da doença no centro da zona.
    /// Ajuda o jogador a identificar qual doença está presente.
    /// </summary>
    [SerializeField]
    private SpriteRenderer iconSickness = null;

    #endregion

    #region Configuração de Raycast

    [Space]
    /// <summary>
    /// Distância Z para cálculo de plano de raycast (detecção de clique).
    /// </summary>
    [SerializeField]
    private float m_DistanceZ = 10f;

    /// <summary>
    /// Vetor de distância da câmera para o plano de raycast.
    /// </summary>
    private Vector3 distanceFromCamera;

    #endregion

    #region Componentes

    /// <summary>
    /// LineRenderer para desenhar o círculo visual.
    /// </summary>
    private LineRenderer line;

    /// <summary>
    /// Collider para detectar personagens entrando na zona.
    /// </summary>
    private BoxCollider sphere;

    /// <summary>
    /// Sprite renderer do ícone (referência adicional).
    /// </summary>
    private SpriteRenderer icon;

    #endregion

    #region Referências de Sistemas

    /// <summary>
    /// Painel de alertas para mostrar notificações de infecção.
    /// </summary>
    private AlertPanel alertPanel;

    /// <summary>
    /// Painel que mostra detalhes da doença quando clicada.
    /// </summary>
    private PanelSickness panelSickness;

    /// <summary>
    /// Referência à câmera principal.
    /// </summary>
    private Camera cameraBehaviour;

    /// <summary>
    /// Referência ao sistema de tempo.
    /// </summary>
    private ClockBehaviour clock;

    /// <summary>
    /// Plano usado para calcular raycasts de clique.
    /// </summary>
    private Plane plane;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização precoce - obtém referências aos componentes e sistemas.
    /// </summary>
    private void Awake()
    {
        // Encontra referências globais
        alertPanel = FindObjectOfType<AlertPanel>();
        panelSickness = FindObjectOfType<PanelSickness>();
        cameraBehaviour = FindObjectOfType<Camera>();
        clock = FindObjectOfType<ClockBehaviour>();

        // Obtém componentes locais
        line = GetComponent<LineRenderer>();
        sphere = GetComponent<BoxCollider>();
    }

    /// <summary>
    /// Inicialização - configura visual e collider da zona.
    /// </summary>
    private void Start()
    {
        // Define ícone visual da doença
        iconSickness.sprite = sicknessData.icon;

        // Configura LineRenderer para desenhar o círculo
        line.positionCount = segments + 1;
        line.startWidth = widhtLine;
        line.endWidth = widhtLine;
        line.useWorldSpace = false;

        // Configura tamanho do collider trigger
        sphere.size = new Vector3(radius, 1, radius);

        // Desenha o círculo visual
        SpawnCircle();

        // Configura plano para detecção de cliques
        distanceFromCamera = new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y,
            Camera.main.transform.position.z - m_DistanceZ
        );
        plane = new Plane(Vector3.forward, distanceFromCamera);
    }

    /// <summary>
    /// Atualização - detecta cliques na zona de doença.
    ///
    /// Quando o jogador clica na zona, abre o painel de informações
    /// da doença para que ele possa ver os detalhes (sintomas, letalidade, etc.).
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Cria raio da câmera para o ponto do clique
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Atualiza posição do plano
            distanceFromCamera = new Vector3(
                Camera.main.transform.position.x,
                Camera.main.transform.position.y,
                Camera.main.transform.position.z - m_DistanceZ
            );
            plane.SetNormalAndPosition(Vector3.forward, distanceFromCamera);

            // Tenta detectar hit no plano frontal
            if (plane.Raycast(ray, out float dist))
            {
                Vector3 posTarget = ray.GetPoint(dist);
                ray.direction = (posTarget - ray.origin).normalized;
                Debug.DrawRay(ray.origin, ray.direction * 99999, Color.red);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // Verifica se clicou em uma zona de doença
                    if (hit.transform.tag == "Diseases")
                        panelSickness.Show(sicknessData);
                }
            }
            else
            {
                // Tenta com plano atrás da câmera (fallback)
                distanceFromCamera = new Vector3(
                    Camera.main.transform.position.x,
                    Camera.main.transform.position.y,
                    Camera.main.transform.position.z + m_DistanceZ
                );
                plane.SetNormalAndPosition(Vector3.forward, distanceFromCamera);

                if (plane.Raycast(ray, out dist))
                {
                    Vector3 posTarget = ray.GetPoint(dist);
                    ray.direction = (posTarget - ray.origin).normalized;
                    Debug.DrawRay(ray.origin, ray.direction * 99999, Color.red);

                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform.tag == "Diseases")
                            panelSickness.Show(sicknessData);
                    }
                }
            }
        }
    }

    #endregion

    #region Sistema de Transmissão

    /// <summary>
    /// Detecta quando um personagem entra na zona de infecção.
    ///
    /// Condições para transmissão:
    /// - Personagem deve ter CharacterStatus
    /// - Personagem deve estar saudável (Health == Healthy)
    /// - Personagem deve ter permissão para ficar doente (IsPermissionSick)
    /// </summary>
    /// <param name="other">Collider do objeto que entrou na zona</param>
    private void OnTriggerEnter(Collider other)
    {
        CharacterStatus cs = other.GetComponent<CharacterStatus>();

        // Verifica se é um personagem saudável que pode ser infectado
        if (cs && cs.Health == HealthCondition.Healthy && cs.IsPermissionSick)
        {
            Transmission(cs);
        }
    }

    /// <summary>
    /// Tenta transmitir a doença para um personagem.
    ///
    /// A transmissão é probabilística:
    /// - Chance = transmissibilidade da doença - sistema imune do personagem
    /// - Se a doença é transmitida, cria alerta visual
    /// </summary>
    /// <param name="character">Status do personagem a ser potencialmente infectado</param>
    private void Transmission(CharacterStatus character)
    {
        // Tenta transmitir usando a lógica do DataSickness
        if (sicknessData.Transmission(character, clock.CurrentDay))
        {
            // Transmissão bem-sucedida - cria alerta
            alertPanel.SpawnAlertFocus(character);
        }
    }

    #endregion

    #region Geração Visual

    /// <summary>
    /// Desenha o círculo visual usando o LineRenderer.
    ///
    /// Cria pontos ao redor do centro em intervalos angulares regulares,
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
