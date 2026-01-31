using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface para destinos de rota global (estabelecimentos de saúde).
///
/// Implementada por:
/// - Hospital: Tratamento completo + vacinação
/// - Laboratory: Diagnóstico de doenças
/// - HealthCenter: Vacinação preventiva
///
/// Esta interface permite que a IA dos personagens envie pacientes
/// para qualquer estabelecimento de saúde sem conhecer os detalhes
/// de implementação de cada um.
///
/// Padrão: Strategy Pattern
/// A IA chama TreatPatient() e o estabelecimento específico
/// decide como tratar o paciente.
/// </summary>
public interface IRouteGlobal
{
    /// <summary>
    /// Inicia o tratamento/atendimento de um paciente.
    ///
    /// Chamado pela IA quando o personagem chega ao estabelecimento.
    /// Cada implementação define seu próprio fluxo:
    /// - Hospital: Internação, diagnóstico, cura, vacinação
    /// - Laboratory: Exame, diagnóstico, liberação (ainda doente)
    /// - HealthCenter: Vacinação preventiva, liberação
    /// </summary>
    /// <param name="patient">Transform do personagem a ser tratado</param>
    void TreatPatient(Transform patient);
}
