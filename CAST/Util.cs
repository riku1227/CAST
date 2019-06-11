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

        public static object getPrivateField(Type type, object instance, String fieldName)
        {
            FieldInfo info = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return info.GetValue(instance);
        }

        public static void setPrivateField(Type type, object instance, String fieldName, object value)
        {
            FieldInfo info = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            info.SetValue(instance, value);
        }
    }
}
