using System.Collections.Generic;

namespace Estuite.Domain
{
    public static class ObjectExtensions
    {
        public static bool IsNullOrEmpty<T>(this T value)
        {
            //
            // http://stackoverflow.com/questions/1895761/test-for-equality-to-the-default-value
            //
            var stringValue = value as string;
            if (stringValue != null) return string.IsNullOrEmpty(stringValue);
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }
    }
}