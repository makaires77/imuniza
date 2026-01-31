using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ponto de rota para navegação de personagens.
///
/// Cada PointRoute representa um nó em um grafo de navegação.
/// Os personagens caminham entre esses pontos, criando movimento
/// natural pelas calçadas e ruas da cidade.
///
/// Sistema de conexões:
/// - Cada ponto tem uma lista de pontos conectados (vizinhos)
/// - A IA escolhe aleatoriamente um vizinho como próximo destino
/// - As conexões são visualizadas como linhas no Editor (Gizmos)
///
/// Configuração no Editor:
/// 1. Posicione os PointRoutes nas calçadas
/// 2. Configure os vizinhos no Inspector (relativePoints)
/// 3. As linhas coloridas mostram as conexões
/// </summary>
public class PointRoute : MonoBehaviour
{
    #region Configuração

    /// <summary>
    /// Lista de pontos conectados a este ponto.
    /// A IA pode caminhar para qualquer um desses pontos.
    /// Configure no Inspector arrastando outros PointRoutes.
    /// </summary>
    [SerializeField]
    private List<PointRoute> relativePoints = null;

    /// <summary>
    /// Cor das linhas de conexão exibidas no Editor.
    /// Útil para diferenciar tipos de rotas visualmente.
    /// </summary>
    [SerializeField]
    private Color colorLine = Color.blue;

    #endregion

    #region Propriedades

    /// <summary>
    /// Array de pontos conectados (somente leitura).
    /// </summary>
    public PointRoute[] RelativePoints => relativePoints.ToArray();

    #endregion

    #region Métodos Públicos

    /// <summary>
    /// Retorna um ponto conectado aleatório.
    ///
    /// Usado pela IA para escolher o próximo destino
    /// durante a caminhada aleatória.
    /// </summary>
    /// <returns>Um PointRoute aleatório da lista de conexões</returns>
    public PointRoute GetRandomPoint()
    {
        int indexRandom = Random.Range(0, relativePoints.Count);
        PointRoute point = relativePoints[indexRandom];

        return point;
    }

    #endregion

    #region Editor

    /// <summary>
    /// Desenha linhas de conexão no Editor (Scene View).
    ///
    /// Facilita a visualização e configuração da rede de rotas.
    /// Só é executado no Editor, não afeta o jogo em runtime.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = colorLine;

        for (int i = 0; i < relativePoints.Count; i++)
        {
            if (relativePoints[i])
                Gizmos.DrawLine(transform.position, relativePoints[i].transform.position);
        }
    }

    #endregion
}
