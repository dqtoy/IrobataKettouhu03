using System;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal {
    /// <summary>
    /// Caches type name to type lookups. Type lookups occur in all loaded assemblies.
    /// ルックアップをタイプするためにキャッシュをタイプします。 タイプルックアップは、ロードされたすべてのアセンブリで発生します。
    /// </summary>
    public static class fsTypeCache {
        /// <summary>
        /// Cache from fully qualified type name to type instances.
        /// 完全修飾型名から型インスタンスにキャッシュします。
        /// </summary>
        // TODO: verify that type names will be unique
        //TODO：型名が一意であることを確認する
        private static Dictionary<string, Type> _cachedTypes = new Dictionary<string, Type>();

        /// <summary>
        /// Assemblies indexed by their name.
        /// アセンブリーはその名前で索引付けされます。
        /// </summary>
        private static Dictionary<string, Assembly> _assembliesByName;

        /// <summary>
        /// A list of assemblies, by index.
        /// インデックス別のアセンブリーのリスト。
        /// </summary>
        private static List<Assembly> _assembliesByIndex;

        static fsTypeCache() {
            // Setup assembly references so searching and the like resolves correctly.
            //検索などが正しく行われるようにアセンブリ参照を設定します。
            _assembliesByName = new Dictionary<string, Assembly>();
            _assembliesByIndex = new List<Assembly>();

#if (!UNITY_EDITOR && UNITY_METRO && !ENABLE_IL2CPP) // no AppDomain on WinRT //WinRTにAppDomainがありません
            var assembly = typeof(object).GetTypeInfo().Assembly;
            _assembliesByName[assembly.FullName] = assembly;
            _assembliesByIndex.Add(assembly);
#else
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                _assembliesByName[assembly.FullName] = assembly;
                _assembliesByIndex.Add(assembly);
            }
#endif

            _cachedTypes = new Dictionary<string, Type>();

#if !(UNITY_WP8  || UNITY_METRO) // AssemblyLoad events are not supported on these platforms //AssemblyLoadイベントはこれらのプラットフォームではサポートされていません
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;
#endif
        }

#if !(UNITY_WP8 || UNITY_METRO) // AssemblyLoad events are not supported on these platforms //AssemblyLoadイベントはこれらのプラットフォームではサポートされていません
        private static void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args) {
            _assembliesByName[args.LoadedAssembly.FullName] = args.LoadedAssembly;
            _assembliesByIndex.Add(args.LoadedAssembly);

            _cachedTypes = new Dictionary<string, Type>();
        }
#endif

        /// <summary>
        /// Does a direct lookup for the given type, ie, goes directly to the assembly identified by
        /// assembly name and finds it there.
        /// 指定された型を直接参照するかどうか、つまりアセンブリ名で識別されたアセンブリに直接移動し、そこにあるかどうかを調べます。
        /// </summary>
        /// <param name="assemblyName">The assembly to find the type in.</param>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="type">The found type.</param>
        /// <returns>True if the type was found, false otherwise.</returns>
        private static bool TryDirectTypeLookup(string assemblyName, string typeName, out Type type) {
            if (assemblyName != null) {
                Assembly assembly;
                if (_assembliesByName.TryGetValue(assemblyName, out assembly)) {
                    type = assembly.GetType(typeName, /*throwOnError:*/ false);
                    return type != null;
                }
            }

            type = null;
            return false;
        }

        /// <summary>
        /// Tries to do an indirect type lookup by scanning through every loaded assembly until the
        /// type is found in one of them.
        /// ロードされたすべてのアセンブリをスキャンして、型がその中に見つかるまで間接型の参照を試みます。
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="type">The found type.</param>
        /// <returns>True if the type was found, false otherwise.</returns>
        private static bool TryIndirectTypeLookup(string typeName, out Type type) {
            // There used to be a foreach loop through the value keys of the _assembliesByName
            // dictionary. However, during that loop assembly loads could occur, causing an
            // OutOfSync exception. To resolve that, we just iterate through the assemblies by
            // index.
            //以前は、_assembliesByNameディクショナリの値キーを使ってforeachループを実行していました。 
            //ただし、ループ中にアセンブリの負荷が発生し、OutOfSync例外が発生する可能性があります。 これを解決するために、アセンブリをインデックスごとに反復するだけです。

            int i = 0;
            while (i < _assembliesByIndex.Count) {
                Assembly assembly = _assembliesByIndex[i];

                // try GetType; should be fast
                //GetTypeを試してください。 速くなければならない
                type = assembly.GetType(typeName);
                if (type != null) {
                    return true;
                }

                // private type or similar; go through the slow path and check every type's full
                // name
                //私的なタイプまたは同様のもの; 遅いパスを通り、すべてのタイプのフルネームをチェックする
                foreach (var foundType in assembly.GetTypes()) {
                    if (foundType.FullName == typeName) {
                        type = foundType;
                        return true;
                    }
                }

                ++i;
            }

            type = null;
            return false;
        }

        /// <summary>
        /// Removes any cached type lookup results.
        /// キャッシュされた型参照結果を削除します。
        /// </summary>
        public static void Reset() {
            _cachedTypes = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Find a type with the given name. An exception is thrown if no type with the given name
        /// can be found. This method searches all currently loaded assemblies for the given type. If the type cannot
        /// be found, then null will be returned.
        /// 指定された名前の型を検索します。 指定された名前の型が見つからない場合は、例外がスローされます。 
        /// このメソッドは、現在ロードされているすべてのアセンブリを特定の型で検索します。 型が見つからない場合はnullが返されます。
        /// </summary>
        /// <param name="name">The fully qualified name of the type.</param>
        public static Type GetType(string name) {
            return GetType(name, null);
        }

        /// <summary>
        /// Find a type with the given name. An exception is thrown if no type with the given name
        /// can be found. This method searches all currently loaded assemblies for the given type. If the type cannot
        /// be found, then null will be returned.
        /// 指定された名前の型を検索します。 指定された名前の型が見つからない場合は、
        /// 例外がスローされます。 このメソッドは、現在ロードされているすべてのアセンブリを特定の型で検索します。 型が見つからない場合はnullが返されます。
        /// </summary>
        /// <param name="name">The fully qualified name of the type.</param>
        /// <param name="assemblyHint">A hint for the assembly to start the search with. Use null if unknown.</param>
        public static Type GetType(string name, string assemblyHint) {
            if (string.IsNullOrEmpty(name)) {
                return null;
            }

            Type type;
            if (_cachedTypes.TryGetValue(name, out type) == false) {
                // if both the direct and indirect type lookups fail, then throw an exception
                //直接型と間接型の両方の検索が失敗した場合は、例外をスローします
                if (TryDirectTypeLookup(assemblyHint, name, out type) == false &&
                    TryIndirectTypeLookup(name, out type) == false) {
                }

                _cachedTypes[name] = type;
            }

            return type;
        }
    }
}

