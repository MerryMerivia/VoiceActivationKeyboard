using System.Linq;

namespace VAK.Helpers
{
    public static class XAMLHelper
    {
        public static bool NegateBool(bool value) => !value;

        public static bool All(params bool[] values) => values.All(e => e);

        public static bool IsStringEmpty(string str) => string.IsNullOrEmpty(str);

        public static bool IsStringNotEmpty(string str) => !string.IsNullOrEmpty(str);

        public static string Concatenate(params string[] values) => string.Join(" - ", values);
    }
}
