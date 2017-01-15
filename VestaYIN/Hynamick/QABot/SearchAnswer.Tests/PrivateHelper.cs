// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTestHelper.cs" company="Vesta">
//   Copyright.
// </copyright>
// <summary>
//   Defines the UnitTestHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Vesta.Common.TestTools.UnitTesting
{
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class PrivateHelper
    {
        public static object NullParameter = null;

        public static object InvokeStatic<T>(string methodName, params object[] args)
        {
            var privateType = new PrivateType(typeof(T));
            return privateType.InvokeStatic(
                methodName,
                BindingFlags.Static | BindingFlags.NonPublic,
                args,
                System.Globalization.CultureInfo.CurrentCulture);
        }

        public static object InvokePrivateMethod(this object instance, string methodName, params object[] args)
        {
            var privateObject = new PrivateObject(instance);
            return privateObject.Invoke(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic,
                args,
                System.Globalization.CultureInfo.CurrentCulture);
        }

        public static object GetPrivateField(this object instance, string fieldName)
        {
            var privateObject = new PrivateObject(instance);
            return privateObject.GetField(fieldName, 
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        }

        public static void SetPrivateField(this object instance, string fieldName, object fieldValue)
        {
            var privateObject = new PrivateObject(instance);
            privateObject.SetField(fieldName, 
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField, fieldValue);
        }
    }
}
