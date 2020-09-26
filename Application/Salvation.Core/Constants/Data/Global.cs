using System;
using System.ComponentModel;
using System.Reflection;

namespace Salvation.Core.Constants.Data
{
    public enum Spec
    {
        None = 0,
        HolyPriest = 257
    }

    public enum Covenant
    {
        [Description("None")]
        None = 0,
        [Description("Kyrian")]
        Kyrian = 1,
        [Description("Venthyr")]
        Venthyr = 2,
        [Description("Night Fae")]
        NightFae = 3,
        [Description("Necrolord")]
        Necrolord = 4,
    }

    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T value)
        where T : Enum
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }
}
