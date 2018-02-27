using System;
using System.Collections.Generic;
using System.Linq;

#if !UNITY_EDITOR && UNITY_WSA
// For System.Reflection.TypeExtensions
using System.Reflection;
// For Reflection Extensions in non-win10 builds.
using FullSerializer.Internal;
#endif

namespace FullSerializer {
    public static class fsTypeExtensions {
        /// <summary>
        /// Returns a pretty name for the type in the style of one that you'd see in C# without the namespace.
        /// ネームスペースのないC＃で見られるようなスタイルで、その型のかなりの名前を返します。
        /// </summary>
        public static string CSharpName(this Type type) {
            return CSharpName(type, /*includeNamespace:*/false);
        }

        public static string CSharpName(this Type type, bool includeNamespace, bool ensureSafeDeclarationName) {
            var name = CSharpName(type, includeNamespace);
            if (ensureSafeDeclarationName) name = name.Replace('>', '_').Replace('<', '_').Replace('.', '_');
            return name;
        }

        /// <summary>
        /// Returns a pretty name for the type in the style of one that you'd see in C#.
        /// あなたがC＃で見られるようなスタイルの、きれいな名前を返します。
        /// </summary>
        /// <parparam name="includeNamespace">Should the name include namespaces?</parparam>
        public static string CSharpName(this Type type, bool includeNamespace) {
            // we special case some of the common type names
            if (type == typeof(void)) return "void";
            if (type == typeof(int)) return "int";
            if (type == typeof(float)) return "float";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(double)) return "double";
            if (type == typeof(string)) return "string";

            // Generic parameter, ie, T in Okay<T>
            //一般的なパラメータ（Okay<T> のT）
            // We special-case this logic otherwise we will recurse on the T
            //特別なケースではこのロジックを使用しなければ、Tで再帰的に扱います
            if (type.IsGenericParameter) {
                return type.ToString();
            }

            string name = "";

            var genericArguments = (IEnumerable<Type>)type.GetGenericArguments();
            if (type.IsNested) {
                name += type.DeclaringType.CSharpName() + ".";

                // The declaring type generic parameters are considered part of the nested types generic
                // parameters so we need to remove them, otherwise it will get included again.
                //宣言型のジェネリックパラメータは、ネストされたタイプのジェネリックパラメータの一部とみなされるため、それらを削除する必要があります。それ以外の場合は、再度インクルードされます。

                // Say we have type `class Parent<T> { class Child {} }`
                // If we did not do the removal, then we would output Parent<T>.Child<T>, but we really want
                // to output Parent<T>.Child
                //`class Parent <T> {class Child {}}`を削除しなかった場合は、Parent <T> .Child <T>を出力しますが、本当にParent <T>を出力したいとします。 子
                if (type.DeclaringType.GetGenericArguments().Length > 0) {
                    genericArguments = genericArguments.Skip(type.DeclaringType.GetGenericArguments().Length);
                }
            }

            if (genericArguments.Any() == false) {
                name += type.Name;
            }
            else {
                name += type.Name.Substring(0, type.Name.IndexOf('`'));
                name += "<" + String.Join(",", genericArguments.Select(t => CSharpName(t, includeNamespace)).ToArray()) + ">";
            }

            if (includeNamespace && type.Namespace != null) {
                name = type.Namespace + "." + name;
            }

            return name;
        }
    }
}