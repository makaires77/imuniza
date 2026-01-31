using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerenciador de veículos (ambulâncias) do jogo.
///
/// Responsabilidades:
/// - Inicializar todos os veículos com suas rotas
/// - Manter referências aos sistemas de carros
///
/// Sistema de veículos:
/// Os carros (ambulâncias) seguem rotas predefinidas pela cidade.
/// Cada CarIA contém:
/// - CarEngine: Motor que controla o movimento
/// - CarPath: Definição da rota a seguir
///
/// Configuração:
/// 1. Adicione CarEngine e CarPath aos GameObjects de veículos
/// 2. Configure as rotas nos CarPaths
/// 3. Adicione os CarIAs ao array iaCars neste componente
/// </summary>
public class CarManager : MonoBehaviour
{
    #region Configuração

    /// <summary>
    /// Array de veículos a serem gerenciados.
    /// Configure no Inspector adicionando CarIAs.
    /// </summary>
    [SerializeField]
    private CarIA[] iaCars;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização - configura rotas de todos os veículos.
    /// </summary>
    private void Start()
    {
        // Inicializa cada veículo com sua rota
        for (int i = 0; i < iaCars.Length; i++)
        {
            iaCars[i].carEngine.Setpath(iaCars[i].carPath.GeneratePath());
        }
    }

    #endregion
}

/// <summary>
/// Estrutura que associa um motor de carro com sua rota.
///
/// Usado pelo CarManager para configurar veículos.
/// </summary>
[System.Serializable]
public struct CarIA
{
    /// <summary>
    /// Motor que controla o movimento do veículo.
    /// </summary>
    public CarEngine carEngine;

    /// <summary>
    /// Componente que define a rota do veículo.
    /// </summary>
    public CarPath carPath;
}
