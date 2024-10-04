using System.ComponentModel;
using System.Globalization;

namespace Extensoes;

public static class EnumExtensions
{
    public static string? Description<T>(this T e) where T : IConvertible
    {
        string? descricao = null;

        if (e is Enum)
        {
            Type type = e.GetType();

            foreach (int valor in Enum.GetValues(type))
            {
                if (valor == e.ToInt32(CultureInfo.InvariantCulture))
                {
                    var memInfo = type.GetMember(type.GetEnumName(valor));
                    var descriptionAttribute = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (descriptionAttribute.Length > 0)
                        descricao = ((DescriptionAttribute)descriptionAttribute[0]).Description;

                    break;
                }
            }
        }
        return descricao;
    }

    public static T EnumValue<T>(this string descricao)
    {
        var type = typeof(T);

        if (!type.IsEnum) throw new InvalidOperationException("Tipo deve ser um Enum");

        foreach (var field in type.GetFields())
        {
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            if (attribute != null)
            {
                if (attribute.Description.ToUpper() == descricao.ToUpper())
                    return (T)field.GetValue(null);

                if (field.Name.ToUpper() == descricao.ToUpper())
                    return (T)field.GetValue(null);
            }
            else
            {
                if (field.Name.ToUpper() == descricao.ToUpper())
                    return (T)field.GetValue(null);
            }
        }
        throw new ArgumentException($"Nao foi localizado nenhuma descricao {descricao} no enum {typeof(T)}");
    }
}