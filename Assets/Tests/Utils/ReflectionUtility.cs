using System.Reflection;

namespace Tests.Utils
{
    /// <summary>Чтение/запись private-полей в тестах.</summary>
    public static class ReflectionUtility
    {
        public static void SetField<T>(object obj, string name, T val)
        {
            var f = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            f?.SetValue(obj, val);
        }

        public static T GetField<T>(object obj, string name)
        {
            var f = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)f?.GetValue(obj);
        }
    }
}
