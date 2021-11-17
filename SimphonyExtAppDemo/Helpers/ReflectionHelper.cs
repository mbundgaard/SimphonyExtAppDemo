using System;
using System.Reflection;

namespace SimphonyExtAppDemo.Helpers
{
    public static class ReflectionHelper
    {
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            var objType = obj.GetType();

            var propInfo = objType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (propInfo == null)
                throw new ArgumentOutOfRangeException("propertyName", $"Couldn't find property {propertyName} in type {objType.FullName}");

            return propInfo.GetValue(obj, null);
        }
    }
}
