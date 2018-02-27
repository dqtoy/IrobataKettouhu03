using System.Collections.Generic;

namespace CCGKit
{
    /// <summary>
    /// Miscellaneous reflection utilities.
    /// その他の反射ユーティリティ。
    /// </summary>
    public static class ReflectionHelpers
    {
        /// <summary>
        /// Returns all the types derived from the specified type.
        /// 指定された型から派生したすべての型を返します。
        /// </summary>
        /// <param name="aAppDomain">The app domain.</param>
        /// <param name="aType">The base type.</param>
        /// <returns>All the types derived from the specified type.</returns>
        /// 指定された型から派生したすべての型
        public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
        {
            var result = new List<System.Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(aType))
                    {
                        result.Add(type);
                    }
                }
            }
            return result.ToArray();
        }
    }
}
