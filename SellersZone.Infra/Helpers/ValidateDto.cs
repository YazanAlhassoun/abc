using System;

namespace SellersZone.Infra.Helpers
{
    public class DtoValidator
    {
        public static bool AreAnyNullOrEmpty(params object[] values)
        {
            foreach (var value in values)
            {
                if (value == null)
                {
                    return true;
                }

                if (value is string strValue && string.IsNullOrWhiteSpace(strValue))
                {
                    return true;
                }

                // Check if value is of a type that is nullable or has no value
                Type valueType = value.GetType();

                if (IsNullableType(valueType))
                {
                    // Handle nullable value types
                    object nonNullableValue = Convert.ChangeType(value, Nullable.GetUnderlyingType(valueType) ?? valueType);

                    if (nonNullableValue == null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsNullableType(Type type)
        {
            return !type.IsValueType || (Nullable.GetUnderlyingType(type) != null);
        }
    }
}
