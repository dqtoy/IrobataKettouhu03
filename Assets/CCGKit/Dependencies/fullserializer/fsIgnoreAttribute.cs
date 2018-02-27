using System;

namespace FullSerializer {
    /// <summary>
    /// The given property or field annotated with [JsonIgnore] will not be serialized.
    /// 指定されたプロパティまたは[JsonIgnore]で注釈が付けられたフィールドはシリアル化されません。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class fsIgnoreAttribute : Attribute {
    }
}