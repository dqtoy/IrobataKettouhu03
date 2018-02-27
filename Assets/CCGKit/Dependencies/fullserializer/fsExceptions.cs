// note: This file contains exceptions used by FullSerializer. Exceptions are never used at runtime
//       in FullSerializer; they are only used when validating annotations and code-based models.
//このファイルには、FullSerializerで使用される例外が含まれています。 FullSerializerでは実行時に例外が使用されることはありません。 注釈とコードベースのモデルを検証する場合にのみ使用されます。

using System;

namespace FullSerializer {
    public sealed class fsMissingVersionConstructorException : Exception {
        public fsMissingVersionConstructorException(Type versionedType, Type constructorType) :
            base(versionedType + " is missing a constructor for previous model type " + constructorType) { }
    }

    public sealed class fsDuplicateVersionNameException : Exception {
        public fsDuplicateVersionNameException(Type typeA, Type typeB, string version) :
            base(typeA + " and " + typeB + " have the same version string (" + version + "); please change one of them.") { }
    }
}