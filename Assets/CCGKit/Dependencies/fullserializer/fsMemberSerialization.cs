namespace FullSerializer {
    /// <summary>
    /// Controls how the reflected converter handles member serialization.
    /// 反射コンバータがメンバーのシリアル化をどのように処理するかを制御します。
    /// </summary>
    public enum fsMemberSerialization {
        /// <summary>
        /// Only members with [SerializeField] or [fsProperty] attributes are serialized.
        /// [SerializeField]または[fsProperty]属性を持つメンバーだけがシリアル化されます。
        /// </summary>
        OptIn,

        /// <summary>
        /// Only members with [NotSerialized] or [fsIgnore] will not be serialized.
        /// [NotSerialized]または[fsIgnore]を持つメンバーだけがシリアル化されません。
        /// </summary>
        OptOut,

        /// <summary>
        /// The default member serialization behavior is applied.
        /// 既定のメンバシリアライゼーション動作が適用されます。
        /// </summary>
        Default
    }
}