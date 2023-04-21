using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Millionaires
{
    public static class Tools
    {
        public static Stage LastGuaranteed(this Stage stage)
        {
            FieldInfo field = typeof(Stage).GetField(stage.ToString());
            GuaranteedAttribute attribute = field.GetCustomAttribute<GuaranteedAttribute>();

            var guaranteed = attribute?.Value ?? false;

            if (guaranteed)
                return stage;

            int intStage = (int)stage;

            if (intStage < 2)
                return Stage.Zero;
            if (intStage < 7)
                return Stage.Second;
            return Stage.Seventh;
        }

        public static string Prize( this Stage stage)
        {
            Type enumType = typeof(Stage);
            string stageName = Enum.GetName(enumType, stage);

            StringValueAttribute stringValueAttribute = enumType
                .GetField(stageName)
                .GetCustomAttributes(typeof(StringValueAttribute), false)
                .FirstOrDefault() as StringValueAttribute;

            if (stringValueAttribute != null)
            {
                return stringValueAttribute.Value;
            }

            return stage.ToString(); // Zwróć nazwę enuma jeśli atrybut n
        }
    }
}
