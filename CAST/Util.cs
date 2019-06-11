using System;
using System.Reflection;

namespace CAST
{
    class Util
    {
        public static object getPrivateStaticField(Type type, String fieldName)
        {
            FieldInfo info = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            return info.GetValue(null);
        }

        public static void setPrivateStaticField(Type type, String fieldName, object value)
        {
            FieldInfo info = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            info.SetValue(null, value);
        }
    }
}
