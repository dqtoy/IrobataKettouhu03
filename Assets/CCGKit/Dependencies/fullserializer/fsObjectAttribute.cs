using System;

namespace FullSerializer {
    /// <summary>
    /// This attribute controls some serialization behavior for a type. See the comments
    /// on each of the fields for more information.
    /// この属性は、ある型のいくつかの直列化動作を制御します。 詳細については、各フィールドのコメントを参照してください。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class fsObjectAttribute : Attribute {
        /// <summary>
        /// The previous model that should be used if an old version of this
        /// object is encountered. Using this attribute also requires that the
        /// type have a public constructor that takes only one parameter, an object
        /// instance of the given type. Use of this parameter *requires* that
        /// the VersionString parameter is also set.
        /// このオブジェクトの古いバージョンが検出された場合に使用する必要がある以前のモデル。 
        /// この属性を使用するには、型に、指定された型のオブジェクトインスタンスである1つのパラメータだけを取るパブリックコンストラクタも必要です。
        /// このパラメータ*を使用するには、VersionStringパラメータも設定する必要があります。
        /// </summary>
        public Type[] PreviousModels;

        /// <summary>
        /// The version string to use for this model. This should be unique among all
        /// prior versions of this model that is supported for importation. If PreviousModel
        /// is set, then this attribute must also be set. A good valid example for this
        /// is "v1", "v2", "v3", ...
        /// このモデルに使用するバージョン文字列。 これは、インポートのためにサポートされているこのモデルのすべての旧バージョン間で一意でなければなりません。
        /// PreviousModelが設定されている場合、この属性も設定する必要があります。 これに対して有効な例は、 "v1"、 "v2"、 "v3"、...
        /// </summary>
        public string VersionString;

        /// <summary>
        /// This controls the behavior for member serialization.
        /// The default behavior is fsMemberSerialization.Default.
        /// これは、メンバーシリアライゼーションの動作を制御します。 デフォルトの動作はfsMemberSerialization.Defaultです。
        /// </summary>
        public fsMemberSerialization MemberSerialization = fsMemberSerialization.Default;

        /// <summary>
        /// Specify a custom converter to use for serialization. The converter type needs
        /// to derive from fsBaseConverter. This defaults to null.
        /// シリアル化に使用するカスタムコンバータを指定します。 コンバータタイプはfsBaseConverterから派生する必要があります。 デフォルトはnullです。
        /// </summary>
        public Type Converter;

        /// <summary>
        /// Specify a custom processor to use during serialization. The processor type needs
        /// to derive from fsObjectProcessor and the call to CanProcess is not invoked. This
        /// defaults to null.
        /// シリアル化中に使用するカスタムプロセッサを指定します。 プロセッサタイプはfsObjectProcessorから派生する必要があり、CanProcessへの呼び出しは呼び出されません。 デフォルトはnullです。
        /// </summary>
        public Type Processor;

        public fsObjectAttribute() { }
        public fsObjectAttribute(string versionString, params Type[] previousModels) {
            VersionString = versionString;
            PreviousModels = previousModels;
        }
    }
}