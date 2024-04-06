using System.Text.RegularExpressions;
using System.ComponentModel;

namespace ExtensionMethods
{
    public static class StringUtils
    {
        public static string Regplace(
            this string str, string pattern, string replacement
        )
        {
            return Regex.Replace(str, pattern, replacement);
        }

        public static bool Match(this string str, string pattern)
        {
            return Regex.IsMatch(str, pattern);
        }

        public static string Join(this string[] strArray, string glue)
        {
            return string.Join(glue, strArray);
        }

        public static int IndexOf(this string[] strArray, string find)
        {
            return Array.IndexOf(strArray, find);
        }
    }

    // Instead of DynObject extend objects? (Maybe - this is a quick test...)
    public static class ObjectUtils
    {

        public static object? GetObj(this object obj, string prop)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
            PropertyDescriptor desc = properties.Find(prop, false)!;
            return desc != null ? desc.GetValue(obj)! : null;
        }

    }
}