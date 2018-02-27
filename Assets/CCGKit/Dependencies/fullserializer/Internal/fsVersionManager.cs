using System;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal {
    public static class fsVersionManager {
        private static readonly Dictionary<Type, fsOption<fsVersionedType>> _cache = new Dictionary<Type, fsOption<fsVersionedType>>();

        public static fsResult GetVersionImportPath(string currentVersion, fsVersionedType targetVersion, out List<fsVersionedType> path) {
            path = new List<fsVersionedType>();

            if (GetVersionImportPathRecursive(path, currentVersion, targetVersion) == false) {
                return fsResult.Fail("There is no migration path from \"" + currentVersion + "\" to \"" + targetVersion.VersionString + "\"");
            }

            path.Add(targetVersion);
            return fsResult.Success;
        }

        private static bool GetVersionImportPathRecursive(List<fsVersionedType> path, string currentVersion, fsVersionedType current) {
            for (int i = 0; i < current.Ancestors.Length; ++i) {
                fsVersionedType ancestor = current.Ancestors[i];

                if (ancestor.VersionString == currentVersion ||
                    GetVersionImportPathRecursive(path, currentVersion, ancestor)) {

                    path.Add(ancestor);
                    return true;
                }
            }

            return false;
        }

        public static fsOption<fsVersionedType> GetVersionedType(Type type) {
            fsOption<fsVersionedType> optionalVersionedType;

            if (_cache.TryGetValue(type, out optionalVersionedType) == false) {
                var attr = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);

                if (attr != null) {
                    if (string.IsNullOrEmpty(attr.VersionString) == false || attr.PreviousModels != null) {
                        // Version string must be provided
                        //バージョン文字列を指定する必要があります
                        if (attr.PreviousModels != null && string.IsNullOrEmpty(attr.VersionString)) {
                            throw new Exception("fsObject attribute on " + type + " contains a PreviousModels specifier - it must also include a VersionString modifier");
                        }

                        // Map the ancestor types into versioned types
                        //祖先型をバージョン管理された型にマップする
                        fsVersionedType[] ancestors = new fsVersionedType[attr.PreviousModels != null ? attr.PreviousModels.Length : 0];
                        for (int i = 0; i < ancestors.Length; ++i) {
                            fsOption<fsVersionedType> ancestorType = GetVersionedType(attr.PreviousModels[i]);
                            if (ancestorType.IsEmpty) {
                                throw new Exception("Unable to create versioned type for ancestor " + ancestorType + "; please add an [fsObject(VersionString=\"...\")] attribute");
                            }
                            ancestors[i] = ancestorType.Value;
                        }

                        // construct the actual versioned type instance
                        //実際のバージョン管理された型インスタンスを構築する
                        fsVersionedType versionedType = new fsVersionedType {
                            Ancestors = ancestors,
                            VersionString = attr.VersionString,
                            ModelType = type
                        };

                        // finally, verify that the versioned type passes some sanity checks
                        //最後に、バージョン管理された型がいくつかの健全性チェックをパスすることを確認します
                        VerifyUniqueVersionStrings(versionedType);
                        VerifyConstructors(versionedType);

                        optionalVersionedType = fsOption.Just(versionedType);
                    }
                }

                _cache[type] = optionalVersionedType;
            }

            return optionalVersionedType;
        }

        /// <summary>
        /// Verifies that the given type has constructors to migrate from all ancestor types.
        /// 指定された型にすべての祖先型から移行するコンストラクタがあることを検証します。
        /// </summary>
        private static void VerifyConstructors(fsVersionedType type) {
            ConstructorInfo[] publicConstructors = type.ModelType.GetDeclaredConstructors();

            for (int i = 0; i < type.Ancestors.Length; ++i) {
                Type requiredConstructorType = type.Ancestors[i].ModelType;

                bool found = false;
                for (int j = 0; j < publicConstructors.Length; ++j) {
                    var parameters = publicConstructors[j].GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == requiredConstructorType) {
                        found = true;
                        break;
                    }
                }

                if (found == false) {
                    throw new fsMissingVersionConstructorException(type.ModelType, requiredConstructorType);
                }
            }
        }

        /// <summary>
        /// Verifies that the given version graph contains only unique versions.
        /// 指定されたバージョングラフに固有のバージョンのみが含まれていることを検証します。
        /// </summary>
        private static void VerifyUniqueVersionStrings(fsVersionedType type) {
            // simple tree traversal

            var found = new Dictionary<string, Type>();

            var remaining = new Queue<fsVersionedType>();
            remaining.Enqueue(type);

            while (remaining.Count > 0) {
                fsVersionedType item = remaining.Dequeue();

                // Verify we do not already have the version string. Take into account that we're not just
                // comparing the same model twice, since we can have a valid import graph that has the same
                // model multiple times.
                //バージョン文字列がまだないことを確認します。 同じモデルを複数回持つ有効なインポートグラフを持つことができるので、同じモデルを2回比較するだけではないことを考慮してください。
                if (found.ContainsKey(item.VersionString) && found[item.VersionString] != item.ModelType) {
                    throw new fsDuplicateVersionNameException(found[item.VersionString], item.ModelType, item.VersionString);
                }
                found[item.VersionString] = item.ModelType;

                // scan the ancestors as well
                foreach (var ancestor in item.Ancestors) {
                    remaining.Enqueue(ancestor);
                }
            }
        }
    }
}