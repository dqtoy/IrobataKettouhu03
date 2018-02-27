using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer.Internal;

namespace FullSerializer {
    /// <summary>
    /// The serialization converter allows for customization of the serialization process.
    /// シリアライゼーション・コンバーターは、シリアライズ・プロセスのカスタマイズを可能にします。
    /// </summary>
    /// <remarks>You do not want to derive from this class - there is no way to actually use it within
    /// the serializer.. Instead, derive from either fsConverter or fsDirectConverter</remarks>
    /// <注意>このクラスから派生したくないのですが、シリアライザ内で実際に使用する方法はありません。代わりに、fsConverterまたはfsDirectConverterのいずれかから派生します。</注意>
    public abstract class fsBaseConverter {
        /// <summary>
        /// The serializer that was owns this converter.
        /// このコンバータを所有していたシリアライザ。
        /// </summary>
        public fsSerializer Serializer;

        /// <summary>
        /// Construct an object instance that will be passed to TryDeserialize. This should **not**
        /// deserialize the object.
        /// TryDeserializeに渡すオブジェクトインスタンスを作成します。 これはオブジェクトを非直列化しない**です。
        /// </summary>
        /// <param name="data">The data the object was serialized with.</param>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <returns>An object instance</returns>
        public virtual object CreateInstance(fsData data, Type storageType) {
            if (RequestCycleSupport(storageType)) {
                throw new InvalidOperationException("Please override CreateInstance for " +
                    GetType().FullName + "; the object graph for " + storageType +
                    " can contain potentially contain cycles, so separated instance creation " +
                    "is needed");
            }

            return storageType;
        }

        /// <summary>
        /// If true, then the serializer will support cyclic references with the given converted
        /// type.
        /// trueの場合、シリアライザは指定された変換タイプの循環参照をサポートします。
        /// </summary>
        /// <param name="storageType">The field/property type that is currently storing the object that is being serialized.</param>
        /// 現在直列化されているオブジェクトを格納しているフィールド/プロパティ型。
        public virtual bool RequestCycleSupport(Type storageType) {
            if (storageType == typeof(string)) return false;

            return storageType.Resolve().IsClass || storageType.Resolve().IsInterface;
        }

        /// <summary>
        /// If true, then the serializer will include inheritance data for the given converter.
        /// trueの場合、シリアライザには指定されたコンバーターの継承データが含まれます。
        /// </summary>
        /// <param name="storageType">The field/property type that is currently storing the object
        /// that is being serialized.</param>
        public virtual bool RequestInheritanceSupport(Type storageType) {
            return storageType.Resolve().IsSealed == false;
        }

        /// <summary>
        /// Serialize the actual object into the given data storage.
        /// 実際のオブジェクトを指定されたデータストレージにシリアル化します。
        /// </summary>
        /// <param name="instance">The object instance to serialize. This will never be null.</param>
        /// <param name="serialized">The serialized state.</param>
        /// <param name="storageType">The field/property type that is storing this instance.</param>
        /// <returns>If serialization was successful.</returns>
        public abstract fsResult TrySerialize(object instance, out fsData serialized, Type storageType);

        /// <summary>
        /// Deserialize data into the object instance.
        /// データをオブジェクトインスタンスにデシリアライズします。
        /// </summary>
        /// <param name="data">Serialization data to deserialize from.</param>
        /// <param name="instance">The object instance to deserialize into.</param>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <returns>True if serialization was successful, false otherwise.</returns>
        public abstract fsResult TryDeserialize(fsData data, ref object instance, Type storageType);

        protected fsResult FailExpectedType(fsData data, params fsDataType[] types) {
            return fsResult.Fail(GetType().Name + " expected one of " +
                string.Join(", ", types.Select(t => t.ToString()).ToArray()) +
                " but got " + data.Type + " in " + data);
        }

        protected fsResult CheckType(fsData data, fsDataType type) {
            if (data.Type != type) {
                return fsResult.Fail(GetType().Name + " expected " + type + " but got " + data.Type + " in " + data);
            }
            return fsResult.Success;
        }

        protected fsResult CheckKey(fsData data, string key, out fsData subitem) {
            return CheckKey(data.AsDictionary, key, out subitem);
        }

        protected fsResult CheckKey(Dictionary<string, fsData> data, string key, out fsData subitem) {
            if (data.TryGetValue(key, out subitem) == false) {
                return fsResult.Fail(GetType().Name + " requires a <" + key + "> key in the data " + data);
            }
            return fsResult.Success;
        }

        protected fsResult SerializeMember<T>(Dictionary<string, fsData> data, Type overrideConverterType, string name, T value) {
            fsData memberData;
            var result = Serializer.TrySerialize(typeof(T), overrideConverterType, value, out memberData);
            if (result.Succeeded) data[name] = memberData;
            return result;
        }

        protected fsResult DeserializeMember<T>(Dictionary<string, fsData> data, Type overrideConverterType, string name, out T value) {
            fsData memberData;
            if (data.TryGetValue(name, out memberData) == false) {
                value = default(T);
                return fsResult.Fail("Unable to find member \"" + name + "\"");
            }

            object storage = null;
            var result = Serializer.TryDeserialize(memberData, typeof(T), overrideConverterType, ref storage);
            value = (T)storage;
            return result;
        }
    }
}