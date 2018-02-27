using System;

namespace FullSerializer {
    /// <summary>
    /// Explicitly mark a property to be serialized. This can also be used to give the name that the
    /// property should use during serialization.
    /// 直列化されるプロパティを明示的にマークします。 これは、プロパティのシリアル化時に使用する名前を指定するためにも使用できます。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class fsPropertyAttribute : Attribute {
        /// <summary>
        /// The name of that the property will use in JSON serialization.
        /// プロパティがJSONのシリアル化で使用する名前。
        /// </summary>
        public string Name;

        /// <summary>
        /// Use a custom converter for the given type. Specify the converter to use using typeof.
        /// 指定した種類のカスタムコンバータを使用します。 typeofを使用して使用するコンバータを指定します。
        /// </summary>
        public Type Converter;

        public fsPropertyAttribute()
            : this(string.Empty) {
        }

        public fsPropertyAttribute(string name) {
            Name = name;
        }
    }
}