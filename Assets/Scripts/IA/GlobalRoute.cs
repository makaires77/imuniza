using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistema de rotas globais para navegação de personagens.
///
/// Responsabilidades:
/// - Manter referências a todos os pontos de rota nas calçadas
/// - Fornecer o ponto de rota mais próximo de uma posição
///
/// Este sistema trabalha em conjunto com PointRoute para criar
/// uma rede de pontos conectados onde os personagens podem caminhar.
///
/// Estrutura típica na cena:
/// GlobalRoute (este script)
/// └── Sidewalk (Transform com pontos de calçada)
///     └── PointRoute_1
///     └── PointRoute_2
///     └── PointRoute_N...
/// </summary>
public class GlobalRoute : MonoBehaviour
{
    #region Configuração

    /// <summary>
    /// Transform pai que contém todos os pontos de rota das calçadas.
    /// Os filhos devem ter o componente PointRoute.
    /// </summary>
    [SerializeField]
    private Transform sidewalk;

    #endregion

    #region Campos Privados

    /// <summary>
    /// Cache de todos os pontos de rota nas calçadas.
    /// </summary>
    private PointRoute[] allPointsSidewalk;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização precoce - cacheia todos os pontos de rota.
    /// </summary>
    private void Awake()
    {
        allPointsSidewalk = sidewalk.GetComponentsInChildren<PointRoute>();
    }

    #endregion

    #region Métodos Públicos

    /// <summary>
    /// Encontra o ponto de rota mais próximo de uma posição.
    ///
    /// Usado pela IA dos personagens para:
    /// - Encontrar ponto inicial ao spawnar
    /// - Retornar à rede de rotas após chegar em um destino
    ///
    /// Algoritmo: Busca linear comparando distâncias (O(n))
    /// </summary>
    /// <param name="currentPosition">Posição de origem</param>
    /// <returns>PointRoute mais próximo da posição</returns>
    public PointRoute GetPointCloserSidewalk(Vector3 currentPosition)
    {
        // Assume primeiro ponto como mais próximo
        PointRoute closerPoint = allPointsSidewalk[0];
        float currentDistanceCloser = Vector3.Distance(currentPosition, allPointsSidewalk[0].transform.position);

        // Compara com todos os outros pontos
        for (int i = 1; i < allPointsSidewalk.Length; i++)
        {
            float distanceNewPoint = Vector3.Distance(currentPosition, allPointsSidewalk[i].transform.position);

            // Se encontrar ponto mais próximo, atualiza
            if (currentDistanceCloser > distanceNewPoint)
            {
                closerPoint = allPointsSidewalk[i];
                currentDistanceCloser = distanceNewPoint;
            }
        }

        return closerPoint;
    }

    #endregion
}
