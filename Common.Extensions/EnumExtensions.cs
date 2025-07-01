using System.ComponentModel;
using System.Globalization;

namespace Extensoes;

public static class EnumExtensions
{
    public static string? Description<T>(this T e) where T : IConvertible
    {
        string? description = null;

        if (e is Enum)
        {
            Type type = e.GetType();

            foreach (int value in Enum.GetValues(type))
            {
                if (value == e.ToInt32(CultureInfo.InvariantCulture))
                {
                    var memInfo = type.GetMember(type.GetEnumName(value));
                    var descriptionAttribute = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (descriptionAttribute.Length > 0)
                        description = ((DescriptionAttribute)descriptionAttribute[0]).Description;

                    break;
                }
            }
        }
        return description;
    }

    public static T EnumValue<T>(this string description)
    {
        var type = typeof(T);

        if (!type.IsEnum) throw new InvalidOperationException($"{type} is not an enum type.");

        foreach (var field in type.GetFields())
        {
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            if (attribute != null)
            {
                if (attribute.Description.ToUpper() == description.ToUpper())
                    return (T)field.GetValue(null);

                if (field.Name.ToUpper() == description.ToUpper())
                    return (T)field.GetValue(null);
            }
            else
            {
                if (field.Name.ToUpper() == description.ToUpper())
                    return (T)field.GetValue(null);
            }
        }
        throw new ArgumentException($"description {description} not found in enum {typeof(T)}");
    }
}