using System;
using System.Reflection;

namespace FullSerializer {
    // Global configuration options.
    public static class fsGlobalConfig {
        /// <summary>
        /// Should deserialization be case sensitive? If this is false and the JSON has multiple members with the
        /// same keys only separated by case, then this results in undefined behavior.
        /// </summary>
        /// デシリアライズは大文字と小文字を区別する必要がありますか？ これが偽で、JSONに同じキーが大文字と小文字で区切られた複数のメンバーがある場合、これは未定義の動作になります。
        public static bool IsCaseSensitive = true;

        /// <summary>
        /// If exceptions are allowed internally, then additional date formats can be deserialized.
        /// Note that the Full Serializer public API will *not* throw exceptions with this enabled;
        /// errors will still be returned in a fsResult instance.
        /// 例外が内部的に許可されている場合、追加の日付書式をデシリアライズすることができます。 
        /// フルシリアライザパブリックAPIは、これを有効にして例外をスローしない*ことに注意してください。 エラーは依然としてfsResultインスタンスに返されます。
        /// </summary>
        public static bool AllowInternalExceptions = true;
    }

    /// <summary>
    /// Enables some top-level customization of Full Serializer.
    /// フルシリアライザのトップレベルカスタマイズを有効にします。
    /// </summary>
    public class fsConfig {
        /// <summary>
        /// The attributes that will force a field or property to be serialized.
        /// フィールドまたはプロパティを強制的にシリアル化する属性。
        /// </summary>
        public Type[] SerializeAttributes = {
#if !NO_UNITY
            typeof(UnityEngine.SerializeField),
#endif
            typeof(fsPropertyAttribute)
        };

        /// <summary>
        /// The attributes that will force a field or property to *not* be serialized.
        /// フィールドまたはプロパティを強制的に直列化しない*属性*。
        /// </summary>
        public Type[] IgnoreSerializeAttributes = { typeof(NonSerializedAttribute), typeof(fsIgnoreAttribute) };

        /// <summary>
        /// The default member serialization.
        /// デフォルトのメンバーシリアライゼーション。
        /// </summary>
        public fsMemberSerialization DefaultMemberSerialization = fsMemberSerialization.Default;

        /// <summary>
        /// Convert a C# field/property name into the key used for the JSON object. For example, you could
        /// force all JSON names to lowercase with:
        ///C＃フィールド/プロパティ名をJSONオブジェクトに使用されるキーに変換します。 たとえば、すべてのJSON名を次のように小文字にすることができます。
        ///
        ///    fsConfig.GetJsonNameFromMemberName = (name, info) => name.ToLower();
        ///
        /// This will only be used when the name is not explicitly specified with fsProperty.
        /// これは、名前がfsPropertyで明示的に指定されていない場合にのみ使用されます。
        /// </summary>
        public Func<string, MemberInfo, string> GetJsonNameFromMemberName = (name, info) => name;

        /// <summary>
        /// If false, then *all* property serialization support will be disabled - even properties
        /// explicitly annotated with fsProperty or any other opt-in annotation.
        ///falseの場合、fsPropertyまたはその他のオプトインアノテーションで明示的に注釈が付けられたプロパティさえも、* all *プロパティの直列化サポートは無効になります。
        /// 
        /// Setting this to false means that SerializeNonAutoProperties and
        /// SerializeNonPublicSetProperties will be completely ignored.
        /// これをfalseに設定すると、SerializeNonAutoPropertiesおよびSerializeNonPublicSetPropertiesは完全に無視されます。
        /// </summary>
        public bool EnablePropertySerialization = true;

        /// <summary>
        /// Should the default serialization behaviour include non-auto properties?
        /// デフォルトのシリアル化動作に非自動プロパティが含まれている必要がありますか？
        /// </summary>
        public bool SerializeNonAutoProperties = false;

        /// <summary>
        /// Should the default serialization behaviour include properties with non-public setters?
        /// 既定のシリアル化動作に非公開セッターのプロパティが含まれている必要がありますか？
        /// </summary>
        public bool SerializeNonPublicSetProperties = true;

        /// <summary>
        /// If not null, this string format will be used for DateTime instead of the default one.
        /// nullでない場合、この文字列形式はデフォルトの日付形式ではなくDateTimeに使用されます。
        /// </summary>
        public string CustomDateTimeFormatString = null;

        /// <summary>
        /// Int64 and UInt64 will be serialized and deserialized as string for compatibility
        /// Int64とUInt64はシリアル化され、互換性のために文字列として逆シリアル化されます
        /// </summary>
        public bool Serialize64BitIntegerAsString = false;

        /// <summary>
        /// Enums are serialized using their names by default. Setting this to true will serialize them as integers instead.
        /// Enumは、デフォルトで名前を使用してシリアル化されます。 これをtrueに設定すると、代わりに整数としてシリアル化されます。
        /// </summary>
        public bool SerializeEnumsAsInteger = false;
    }
}