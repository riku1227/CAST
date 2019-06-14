using System;
using System.Reflection;

namespace CAST
{
    public class Util
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

        public static void invokePrivateSetter(Type type, object instance, String fieldName, object value)
        {
            var method = type.GetMethod("set_" + fieldName, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(instance, new object[1] { value });
        }
    }
}
