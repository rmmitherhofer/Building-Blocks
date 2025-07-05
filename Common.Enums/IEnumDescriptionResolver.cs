using System.Reflection;

namespace Common.Enums;

/// <summary>
/// Interface para resolução de descrições de campos de enumeração.
/// Implementações devem retornar uma descrição legível com base nos atributos presentes no campo do enum.
/// </summary>
public interface IEnumDescriptionResolver
{
    /// <summary>
    /// Retorna a descrição do campo do enum, se disponível.
    /// </summary>
    /// <param name="field">Campo do enum para o qual a descrição será recuperada.</param>
    /// <returns>Descrição do campo, ou null se nenhuma for encontrada.</returns>
    string? GetDescription(FieldInfo field);
}
