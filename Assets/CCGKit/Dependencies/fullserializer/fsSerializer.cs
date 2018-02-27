using System;
using System.Collections.Generic;
using FullSerializer.Internal;

#if !UNITY_EDITOR && UNITY_WSA
// For System.Reflection.TypeExtensions
using System.Reflection;
#endif

namespace FullSerializer {
    public class fsSerializer {
        #region Keys
        private static HashSet<string> _reservedKeywords;
        static fsSerializer() {
            _reservedKeywords = new HashSet<string> {
                Key_ObjectReference,
                Key_ObjectDefinition,
                Key_InstanceType,
                Key_Version,
                Key_Content
            };
        }
        /// <summary>
        /// Returns true if the given key is a special keyword that full serializer uses to
        /// add additional metadata on top of the emitted JSON.
        /// </summary>
        public static bool IsReservedKeyword(string key) {
            return _reservedKeywords.Contains(key);
        }

        /// <summary>
        /// This is an object reference in part of a cyclic graph.
        /// </summary>
        private const string Key_ObjectReference = "$ref";

        /// <summary>
        /// This is an object definition, as part of a cyclic graph.
        /// </summary>
        private const string Key_ObjectDefinition = "$id";

        /// <summary>
        /// This specifies the actual type of an object (the instance type was different from
        /// the field type).
        /// </summary>
        private const string Key_InstanceType = "$type";

        /// <summary>
        /// The version string for the serialized data.
        /// </summary>
        private const string Key_Version = "$version";

        /// <summary>
        /// If we have to add metadata but the original serialized state was not a dictionary,
        /// then this will contain the original data.
        /// </summary>
        private const string Key_Content = "$content";

        private static bool IsObjectReference(fsData data) {
            if (data.IsDictionary == false) return false;
            return data.AsDictionary.ContainsKey(Key_ObjectReference);
        }
        private static bool IsObjectDefinition(fsData data) {
            if (data.IsDictionary == false) return false;
            return data.AsDictionary.ContainsKey(Key_ObjectDefinition);
        }
        private static bool IsVersioned(fsData data) {
            if (data.IsDictionary == false) return false;
            return data.AsDictionary.ContainsKey(Key_Version);
        }
        private static bool IsTypeSpecified(fsData data) {
            if (data.IsDictionary == false) return false;
            return data.AsDictionary.ContainsKey(Key_InstanceType);
        }
        private static bool IsWrappedData(fsData data) {
            if (data.IsDictionary == false) return false;
            return data.AsDictionary.ContainsKey(Key_Content);
        }

        /// <summary>
        /// Strips all deserialization metadata from the object, like $type and $content fields.
        /// </summary>
        /// <remarks>After making this call, you will *not* be able to deserialize the same object instance. The metadata is
        /// strictly necessary for deserialization!</remarks>
        public static void StripDeserializationMetadata(ref fsData data) {
            if (data.IsDictionary && data.AsDictionary.ContainsKey(Key_Content)) {
                data = data.AsDictionary[Key_Content];
            }

            if (data.IsDictionary) {
                var dict = data.AsDictionary;
                dict.Remove(Key_ObjectReference);
                dict.Remove(Key_ObjectDefinition);
                dict.Remove(Key_InstanceType);
                dict.Remove(Key_Version);
            }
        }

        /// <summary>
        /// This function converts legacy serialization data into the new format, so that
        /// the import process can be unified and ignore the old format.
        /// </summary>
        private static void ConvertLegacyData(ref fsData data) {
            if (data.IsDictionary == false) return;

            var dict = data.AsDictionary;

            // fast-exit: metadata never had more than two items
            if (dict.Count > 2) return;

            // Key strings used in the legacy system
            string referenceIdString = "ReferenceId";
            string sourceIdString = "SourceId";
            string sourceDataString = "Data";
            string typeString = "Type";
            string typeDataString = "Data";

            // type specifier
            if (dict.Count == 2 && dict.ContainsKey(typeString) && dict.ContainsKey(typeDataString)) {
                data = dict[typeDataString];
                EnsureDictionary(data);
                ConvertLegacyData(ref data);

                data.AsDictionary[Key_InstanceType] = dict[typeString];
            }

            // object definition
            else if (dict.Count == 2 && dict.ContainsKey(sourceIdString) && dict.ContainsKey(sourceDataString)) {
                data = dict[sourceDataString];
                EnsureDictionary(data);
                ConvertLegacyData(ref data);

                data.AsDictionary[Key_ObjectDefinition] = dict[sourceIdString];
            }

            // object reference
            else if (dict.Count == 1 && dict.ContainsKey(referenceIdString)) {
                data = fsData.CreateDictionary();
                data.AsDictionary[Key_ObjectReference] = dict[referenceIdString];
            }
        }
        #endregion

        #region Utility Methods
        private static void Invoke_OnBeforeSerialize(List<fsObjectProcessor> processors, Type storageType, object instance) {
            for (int i = 0; i < processors.Count; ++i) {
                processors[i].OnBeforeSerialize(storageType, instance);
            }
        }
        private static void Invoke_OnAfterSerialize(List<fsObjectProcessor> processors, Type storageType, object instance, ref fsData data) {
            // We run the after calls in reverse order; this significantly reduces the interaction burden between
            // multiple processors - it makes each one much more independent and ignorant of the other ones.
            //アフターコールは逆の順序で実行します。 これにより、複数のプロセッサ間の相互作用の負担が大幅に軽減されます。
            //これにより、各プロセッサは他のプロセッサをはるかに独立して無知にします。

            for (int i = processors.Count - 1; i >= 0; --i) {
                processors[i].OnAfterSerialize(storageType, instance, ref data);
            }
        }
        private static void Invoke_OnBeforeDeserialize(List<fsObjectProcessor> processors, Type storageType, ref fsData data) {
            for (int i = 0; i < processors.Count; ++i) {
                processors[i].OnBeforeDeserialize(storageType, ref data);
            }
        }
        private static void Invoke_OnBeforeDeserializeAfterInstanceCreation(List<fsObjectProcessor> processors, Type storageType, object instance, ref fsData data) {
            for (int i = 0; i < processors.Count; ++i) {
                processors[i].OnBeforeDeserializeAfterInstanceCreation(storageType, instance, ref data);
            }
        }
        private static void Invoke_OnAfterDeserialize(List<fsObjectProcessor> processors, Type storageType, object instance) {
            for (int i = processors.Count - 1; i >= 0; --i) {
                processors[i].OnAfterDeserialize(storageType, instance);
            }
        }
        #endregion

        /// <summary>
        /// Ensures that the data is a dictionary. If it is not, then it is wrapped inside of one.
        /// データが辞書であることを保証します。 そうでなければ、それは1の中に包まれます。
        /// </summary>
        private static void EnsureDictionary(fsData data) {
            if (data.IsDictionary == false) {
                var existingData = data.Clone();
                data.BecomeDictionary();
                data.AsDictionary[Key_Content] = existingData;
            }
        }

        /// <summary>
        /// This manages instance writing so that we do not write unnecessary $id fields. We
        /// only need to write out an $id field when there is a corresponding $ref field. This is able
        /// to write $id references lazily because the fsData instance is not actually written out to text
        /// until we have entirely finished serializing it.
        /// これは、不要な$ idフィールドを書き込まないようにインスタンスの作成を管理します。 
        /// 対応する$ refフィールドがある場合にのみ$ idフィールドを書き出す必要があります。
        /// これは、fsDataインスタンスが実際にテキストに書き出されるのではなく、
        /// 完全にシリアル化が完了するまで、$ id参照を遅らせることができます。
        /// </summary>
        internal class fsLazyCycleDefinitionWriter {
            private Dictionary<int, fsData> _pendingDefinitions = new Dictionary<int, fsData>();
            private HashSet<int> _references = new HashSet<int>();

            public void WriteDefinition(int id, fsData data) {
                if (_references.Contains(id)) {
                    EnsureDictionary(data);
                    data.AsDictionary[Key_ObjectDefinition] = new fsData(id.ToString());
                }

                else {
                    _pendingDefinitions[id] = data;
                }
            }

            public void WriteReference(int id, Dictionary<string, fsData> dict) {
                // Write the actual definition if necessary
                //必要に応じて実際の定義を記述する
                if (_pendingDefinitions.ContainsKey(id)) {
                    var data = _pendingDefinitions[id];
                    EnsureDictionary(data);
                    data.AsDictionary[Key_ObjectDefinition] = new fsData(id.ToString());
                    _pendingDefinitions.Remove(id);
                }
                else {
                    _references.Add(id);
                }

                // Write the reference
                dict[Key_ObjectReference] = new fsData(id.ToString());
            }

            public void Clear() {
                _pendingDefinitions.Clear();
                _references.Clear();
            }
        }

        /// <summary>
        /// Converter type to converter instance lookup table. This could likely be stored inside
        /// of _cachedConverters, but there is a semantic difference because _cachedConverters goes
        /// from serialized type to converter.
        /// コンバータタイプからコンバータインスタンスルックアップテーブル。 
        /// これは_cachedConvertersの内部に格納されている可能性がありますが、
        /// _cachedConvertersはシリアル化された型から変換器に移行するため、意味的な違いがあります。
        /// </summary>
        private Dictionary<Type, fsBaseConverter> _cachedConverterTypeInstances;

        /// <summary>
        /// A cache from type to it's converter.
        /// タイプからそのコンバーターへのキャッシュ。
        /// </summary>
        private Dictionary<Type, fsBaseConverter> _cachedConverters;

        /// <summary>
        /// A cache from type to the set of processors that are interested in it.
        /// タイプから、それに関心のあるプロセッサセットへのキャッシュ。
        /// </summary>
        private Dictionary<Type, List<fsObjectProcessor>> _cachedProcessors;

        /// <summary>
        /// Converters that can be used for type registration.
        /// 型の登録に使用できるコンバーター。
        /// </summary>
        private readonly List<fsConverter> _availableConverters;

        /// <summary>
        /// Direct converters (optimized _converters). We use these so we don't have to
        /// perform a scan through every item in _converters and can instead just do an O(1)
        /// lookup. This is potentially important to perf when there are a ton of direct
        /// converters.
        /// ダイレクトコンバータ（最適化された_コンバータ）。 これらを使用して、
        /// _converters内のすべてのアイテムをスキャンする必要はなく、
        /// 代わりにO（1）ルックアップを行うだけです。 これは、大量のダイレクトコンバータがある場合には、
        /// 潜在的に重要です。
        /// </summary>
        private readonly Dictionary<Type, fsDirectConverter> _availableDirectConverters;

        /// <summary>
        /// Processors that are available.
        /// 利用可能なプロセッサ。
        /// </summary>
        private readonly List<fsObjectProcessor> _processors;

        /// <summary>
        /// Reference manager for cycle detection.
        /// サイクル検出のための参照マネージャ。
        /// </summary>
        private readonly fsCyclicReferenceManager _references;
        private readonly fsLazyCycleDefinitionWriter _lazyReferenceWriter;

        public fsSerializer() {
            _cachedConverterTypeInstances = new Dictionary<Type, fsBaseConverter>();
            _cachedConverters = new Dictionary<Type, fsBaseConverter>();
            _cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();

            _references = new fsCyclicReferenceManager();
            _lazyReferenceWriter = new fsLazyCycleDefinitionWriter();

            // note: The order here is important. Items at the beginning of this
            //       list will be used before converters at the end. Converters
            //       added via AddConverter() are added to the front of the list.
            // 注：ここでの注文は重要です。 このリストの冒頭にある項目は、最後にコンバーターの前に使用されます。
            //     AddConverter（）で追加されたコンバーターがリストの先頭に追加されます。
            _availableConverters = new List<fsConverter> {
                new fsNullableConverter { Serializer = this },
                new fsGuidConverter { Serializer = this },
                new fsTypeConverter { Serializer = this },
                new fsDateConverter { Serializer = this },
                new fsEnumConverter { Serializer = this },
                new fsPrimitiveConverter { Serializer = this },
                new fsArrayConverter { Serializer = this },
                new fsDictionaryConverter { Serializer = this },
                new fsIEnumerableConverter { Serializer = this },
                new fsKeyValuePairConverter { Serializer = this },
                new fsWeakReferenceConverter { Serializer = this },
                new fsReflectedConverter { Serializer = this }
            };
            _availableDirectConverters = new Dictionary<Type, fsDirectConverter>();

            _processors = new List<fsObjectProcessor>() {
                new fsSerializationCallbackProcessor()
            };

#if !NO_UNITY
            _processors.Add(new fsSerializationCallbackReceiverProcessor());
#endif

            Context = new fsContext();
            Config = new fsConfig();

            // Register the converters from the registrar
            // レジストラからコンバータを登録する
            foreach (var converterType in fsConverterRegistrar.Converters) {
                AddConverter((fsBaseConverter)Activator.CreateInstance(converterType));
            }
        }

        /// <summary>
        /// A context object that fsConverters can use to customize how they operate.
        /// fsConvertersが操作方法をカスタマイズするために使用できるコンテキストオブジェクト。
        /// </summary>
        public fsContext Context;

        /// <summary>
        /// Configuration options. Also see fsGlobalConfig.
        /// 設定オプション。 fsGlobalConfigも参照してください。
        /// </summary>
        public fsConfig Config;

        /// <summary>
        /// Add a new processor to the serializer. Multiple processors can run at the same time in the
        /// same order they were added in.
        /// シリアライザに新しいプロセッサを追加します。 複数のプロセッサーは、
        /// 追加されたのと同じ順序で同時に実行できます。
        /// </summary>
        /// <param name="processor">The processor to add.</param>
        public void AddProcessor(fsObjectProcessor processor) {
            _processors.Add(processor);

            // We need to reset our cached processor set, as it could be invalid with the new
            // processor. Ideally, _cachedProcessors should be empty (as the user should fully setup
            // the serializer before actually using it), but there is no guarantee.
            // キャッシュされたプロセッサセットは、新しいプロセッサでは無効になる可能性があるため、
            // リセットする必要があります。 理想的には_cachedProcessorsは空でなければなりません
            // （実際に使用する前にシリアライザを完全に設定する必要があります）が、保証はありません。
            _cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
        }

        /// <summary>
        /// Remove all processors which derive from TProcessor.
        /// TProcessorから派生したすべてのプロセッサを削除します。
        /// </summary>
        public void RemoveProcessor<TProcessor>() {
            int i = 0;
            while (i < _processors.Count) {
                if (_processors[i] is TProcessor) {
                    _processors.RemoveAt(i);
                }
                else {
                    ++i;
                }
            }

            // We need to reset our cached processor set, as it could be invalid with the new
            // processor. Ideally, _cachedProcessors should be empty (as the user should fully setup
            // the serializer before actually using it), but there is no guarantee.
            // キャッシュされたプロセッサセットは、新しいプロセッサでは無効になる可能性があるため、リセットする必要があります。 理想的には_cachedProcessorsは空でなければなりません（実際に使用する前にシリアライザを完全に設定する必要があります）が、保証はありません。
            _cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
        }

        /// <summary>
        /// Fetches all of the processors for the given type.
        /// 指定された型のすべてのプロセッサを取得します。
        /// </summary>
        private List<fsObjectProcessor> GetProcessors(Type type) {
            List<fsObjectProcessor> processors;

            // Check to see if the user has defined a custom processor for the type. If they
            // have, then we don't need to scan through all of the processor to check which
            // one can process the type; instead, we directly use the specified processor.
            // ユーザーがそのタイプのカスタムプロセッサーを定義しているかどうかを確認してください。 もしそれらがあれば、どのプロセッサがそのタイプを処理できるかを調べるために、すべてのプロセッサをスキャンする必要はありません。 代わりに、指定されたプロセッサーを直接使用します。
            var attr = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
            if (attr != null && attr.Processor != null) {
                var processor = (fsObjectProcessor)Activator.CreateInstance(attr.Processor);
                processors = new List<fsObjectProcessor>();
                processors.Add(processor);
                _cachedProcessors[type] = processors;
            }

            else if (_cachedProcessors.TryGetValue(type, out processors) == false) {
                processors = new List<fsObjectProcessor>();

                for (int i = 0; i < _processors.Count; ++i) {
                    var processor = _processors[i];
                    if (processor.CanProcess(type)) {
                        processors.Add(processor);
                    }
                }

                _cachedProcessors[type] = processors;
            }

            return processors;
        }


        /// <summary>
        /// Adds a new converter that can be used to customize how an object is serialized and
        /// deserialized.
        /// オブジェクトの直列化と逆シリアル化の方法をカスタマイズするために使用できる新しいコンバータを追加します。
        /// </summary>
        public void AddConverter(fsBaseConverter converter) {
            if (converter.Serializer != null) {
                throw new InvalidOperationException("Cannot add a single converter instance to " +
                    "multiple fsConverters -- please construct a new instance for " + converter);
            }

            // TODO: wrap inside of a ConverterManager so we can control _converters and _cachedConverters lifetime
            if (converter is fsDirectConverter) {
                var directConverter = (fsDirectConverter)converter;
                _availableDirectConverters[directConverter.ModelType] = directConverter;
            }
            else if (converter is fsConverter) {
                _availableConverters.Insert(0, (fsConverter)converter);
            }
            else {
                throw new InvalidOperationException("Unable to add converter " + converter +
                    "; the type association strategy is unknown. Please use either " +
                    "fsDirectConverter or fsConverter as your base type.");
            }

            converter.Serializer = this;

            // We need to reset our cached converter set, as it could be invalid with the new
            // converter. Ideally, _cachedConverters should be empty (as the user should fully setup
            // the serializer before actually using it), but there is no guarantee.
            // 新しいコンバーターで無効になる可能性があるため、キャッシュされたコンバータセットをリセットする必要があります。 
            // 理想的には_cachedConvertersは空でなければなりません（実際に使用する前にシリアライザを完全に設定する必要があります）が、保証はありません。
            _cachedConverters = new Dictionary<Type, fsBaseConverter>();
        }

        /// <summary>
        /// Fetches a converter that can serialize/deserialize the given type.
        /// 指定された型を直列化/逆直列化できるコンバータを取得します。
        /// </summary>
        private fsBaseConverter GetConverter(Type type, Type overrideConverterType) {
            // Use an override converter type instead if that's what the user has requested.
            // ユーザーが要求したものであれば、代わりにオーバーライドコンバータタイプを使用してください。
            if (overrideConverterType != null) {
                fsBaseConverter overrideConverter;
                if (_cachedConverterTypeInstances.TryGetValue(overrideConverterType, out overrideConverter) == false) {
                    overrideConverter = (fsBaseConverter)Activator.CreateInstance(overrideConverterType);
                    overrideConverter.Serializer = this;
                    _cachedConverterTypeInstances[overrideConverterType] = overrideConverter;
                }

                return overrideConverter;
            }

            // Try to lookup an existing converter.
            // 既存のコンバータを検索してみてください。
            fsBaseConverter converter;
            if (_cachedConverters.TryGetValue(type, out converter)) {
                return converter;
            }

            // Check to see if the user has defined a custom converter for the type. If they
            // have, then we don't need to scan through all of the converters to check which
            // one can process the type; instead, we directly use the specified converter.
            // ユーザーがその種類のカスタムコンバータを定義しているかどうかを確認します。 
            // もしそれらがあれば、どのタイプを処理できるかを調べるためにすべてのコンバータをスキャンする必要はありません。 
            // 代わりに、指定されたコンバータを直接使用します。
            {
                var attr = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
                if (attr != null && attr.Converter != null) {
                    converter = (fsBaseConverter)Activator.CreateInstance(attr.Converter);
                    converter.Serializer = this;
                    return _cachedConverters[type] = converter;
                }
            }

            // Check for a [fsForward] attribute.
            //[fsForward]属性を確認してください。
            {
                var attr = fsPortableReflection.GetAttribute<fsForwardAttribute>(type);
                if (attr != null) {
                    converter = new fsForwardConverter(attr);
                    converter.Serializer = this;
                    return _cachedConverters[type] = converter;
                }
            }


            // There is no specific converter specified; try all of the general ones to see
            // which ones matches.
            // 特定のコンバータは指定されていません。 一般的なものすべてを試して、一致するものを見てください。
            if (_cachedConverters.TryGetValue(type, out converter) == false) {
                if (_availableDirectConverters.ContainsKey(type)) {
                    converter = _availableDirectConverters[type];
                    return _cachedConverters[type] = converter;
                }
                else {
                    for (int i = 0; i < _availableConverters.Count; ++i) {
                        if (_availableConverters[i].CanProcess(type)) {
                            converter = _availableConverters[i];
                            return _cachedConverters[type] = converter;
                        }
                    }
                }
            }

            throw new InvalidOperationException("Internal error -- could not find a converter for " + type);
        }

        /// <summary>
        /// Helper method that simply forwards the call to TrySerialize(typeof(T), instance, out data);
        /// ヘルパーメソッドは、TrySerialize（typeof（T）、インスタンス、アウトデータ）への呼び出しを単純に転送します。
        /// </summary>
        public fsResult TrySerialize<T>(T instance, out fsData data) {
            return TrySerialize(typeof(T), instance, out data);
        }

        /// <summary>
        /// Generic wrapper around TryDeserialize that simply forwards the call.
        /// TryDeserializeの周りの包括的なラッパーで、単純に呼び出しを転送します。
        /// </summary>
        public fsResult TryDeserialize<T>(fsData data, ref T instance) {
            object boxed = instance;
            var fail = TryDeserialize(data, typeof(T), ref boxed);
            if (fail.Succeeded) {
                instance = (T)boxed;
            }
            return fail;
        }

        /// <summary>
        /// Serialize the given value.
        /// 指定された値をシリアル化します。
        /// </summary>
        /// <param name="storageType">The type of field/property that stores the object instance. This is
        /// important particularly for inheritance, as a field storing an IInterface instance
        /// should have type information included.</param>
        /// オブジェクトインスタンスを格納するフィールド/プロパティのタイプ。 IInterfaceインスタンスを格納するフィールドには型情報が含まれている必要があるため、これは特に継承にとって重要です。
        /// <param name="instance">The actual object instance to serialize.</param>
        /// 直列化する実際のオブジェクトインスタンス。
        /// <param name="data">The serialized state of the object.</param>
        /// オブジェクトの直列化された状態。
        /// <returns>If serialization was successful.</returns>
        public fsResult TrySerialize(Type storageType, object instance, out fsData data) {
            return TrySerialize(storageType, null, instance, out data);
        }

        /// <summary>
        /// Serialize the given value.
        /// 指定された値をシリアル化します。
        /// </summary>
        /// <param name="storageType">The type of field/property that stores the object instance. This is
        /// important particularly for inheritance, as a field storing an IInterface instance
        /// should have type information included.</param>
        /// <param name="overrideConverterType">An fsBaseConverter derived type that will be used to serialize
        /// the object instead of the converter found via the normal discovery mechanisms.</param>
        /// <param name="instance">The actual object instance to serialize.</param>
        /// <param name="data">The serialized state of the object.</param>
        /// <returns>If serialization was successful.</returns>
        public fsResult TrySerialize(Type storageType, Type overrideConverterType, object instance, out fsData data) {
            var processors = GetProcessors(instance == null ? storageType : instance.GetType());

            Invoke_OnBeforeSerialize(processors, storageType, instance);

            // We always serialize null directly as null
            //私たちは常にヌルとして直接nullをシリアル化します
            if (ReferenceEquals(instance, null)) {
                data = new fsData();
                Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
                return fsResult.Success;
            }

            var result = InternalSerialize_1_ProcessCycles(storageType, overrideConverterType, instance, out data);
            Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
            return result;
        }

        private fsResult InternalSerialize_1_ProcessCycles(Type storageType, Type overrideConverterType, object instance, out fsData data) {
            // We have an object definition to serialize.
            // シリアル化するオブジェクト定義があります。
            try {
                // Note that we enter the reference group at the beginning of serialization so that we support
                // references that are at equal serialization levels, not just nested serialization levels, within
                // the given subobject. A prime example is serialization a list of references.
                //シリアル化の開始時に参照グループを入力して、指定されたサブオブジェクト内のネストされたシリアル化レベルだけでなく、等しいシリアル化レベルにある参照をサポートするようにします。 主な例は、シリアライゼーションのリファレンスリストです。
                _references.Enter();

                // This type does not need cycle support.
                //このタイプはサイクルサポートを必要としません。
                var converter = GetConverter(instance.GetType(), overrideConverterType);
                if (converter.RequestCycleSupport(instance.GetType()) == false) {
                    return InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
                }

                // We've already serialized this object instance (or it is pending higher up on the call stack).
                // Just serialize a reference to it to escape the cycle.
                //このオブジェクトインスタンスはすでにシリアル化されています（または呼び出しスタック上で上位に保留中です）。
                //サイクルをエスケープするには、その参照をシリアル化してください。
                //
                // note: We serialize the int as a string to so that we don't lose any information
                //       in a conversion to/from double.
                // 注：intを文字列としてシリアル化して、doubleから/への変換で情報が失われないようにします。
                if (_references.IsReference(instance)) {
                    data = fsData.CreateDictionary();
                    _lazyReferenceWriter.WriteReference(_references.GetReferenceId(instance), data.AsDictionary);
                    return fsResult.Success;
                }

                // Mark inside the object graph that we've serialized the instance. We do this *before*
                // serialization so that if we get back into this function recursively, it'll already
                // be marked and we can handle the cycle properly without going into an infinite loop.
                // インスタンスを直列化したオブジェクトグラフの中にマークを付けます。 
                // 再帰的にこの関数に戻っても既にマークされており、無限ループにならずにサイクルを適切に処理できるように、*シリアライゼーションの前にこれを行います。
                _references.MarkSerialized(instance);

                // We've created the cycle metadata, so we can now serialize the actual object.
                // InternalSerialize will handle inheritance correctly for us.
                // サイクルメタデータを作成したので、実際のオブジェクトをシリアル化できます。 InternalSerializeは継承を正しく処理します。
                var result = InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
                if (result.Failed) return result;

                _lazyReferenceWriter.WriteDefinition(_references.GetReferenceId(instance), data);

                return result;
            }
            finally {
                if (_references.Exit()) {
                    _lazyReferenceWriter.Clear();
                }
            }
        }
        private fsResult InternalSerialize_2_Inheritance(Type storageType, Type overrideConverterType, object instance, out fsData data) {
            // Serialize the actual object with the field type being the same as the object
            // type so that we won't go into an infinite loop.
            //実際のオブジェクトをフィールド型をオブジェクト型と同じにしてシリアル化し、無限ループにならないようにします。
            var serializeResult = InternalSerialize_3_ProcessVersioning(overrideConverterType, instance, out data);
            if (serializeResult.Failed) return serializeResult;

            // Do we need to add type information? If the field type and the instance type are different
            // then we will not be able to recover the correct instance type from the field type when
            // we deserialize the object.
            //タイプ情報を追加する必要がありますか？ フィールドタイプとインスタンスタイプが異なる場合、オブジェクトを逆シリアル化するときにフィールドタイプから正しいインスタンスタイプを復元することはできません。
            //
            // Note: We allow converters to request that we do *not* add type information.
            //注：コンバータは、タイプ情報を追加しない*ように要求することができます。
            if (storageType != instance.GetType() &&
                GetConverter(storageType, overrideConverterType).RequestInheritanceSupport(storageType)) {

                // Add the inheritance metadata
                //継承メタデータを追加する
                EnsureDictionary(data);
                data.AsDictionary[Key_InstanceType] = new fsData(instance.GetType().FullName);
            }

            return serializeResult;
        }

        private fsResult InternalSerialize_3_ProcessVersioning(Type overrideConverterType, object instance, out fsData data) {
            // note: We do not have to take a Type parameter here, since at this point in the serialization
            //       algorithm inheritance has *always* been handled. If we took a type parameter, it will
            //       *always* be equal to instance.GetType(), so why bother taking the parameter?
            // 注：ここでは、直列化アルゴリズム継承のこの時点で*常に*処理されているので、ここではTypeパラメータを取る必要はありません。 型パラメータを取った場合、常に* instance * .GetType（）と等しくなります。

            // Check to see if there is versioning information for this type. If so, then we need to serialize it.
            //このタイプのバージョニング情報があるかどうかを確認してください。 もしそうなら、それを直列化する必要があります。
            fsOption<fsVersionedType> optionalVersionedType = fsVersionManager.GetVersionedType(instance.GetType());
            if (optionalVersionedType.HasValue) {
                fsVersionedType versionedType = optionalVersionedType.Value;

                // Serialize the actual object content; we'll just wrap it with versioning metadata here.
                //実際のオブジェクトの内容をシリアル化する。 ここでバージョン管理のメタデータでラップします。
                var result = InternalSerialize_4_Converter(overrideConverterType, instance, out data);
                if (result.Failed) return result;

                // Add the versioning information
                //バージョン情報を追加する
                EnsureDictionary(data);
                data.AsDictionary[Key_Version] = new fsData(versionedType.VersionString);

                return result;
            }

            // This type has no versioning information -- directly serialize it using the selected converter.
            // このタイプにはバージョン管理情報はありません。選択したコンバータを使用して直接シリアル化します。
            return InternalSerialize_4_Converter(overrideConverterType, instance, out data);
        }
        private fsResult InternalSerialize_4_Converter(Type overrideConverterType, object instance, out fsData data) {
            var instanceType = instance.GetType();
            return GetConverter(instanceType, overrideConverterType).TrySerialize(instance, out data, instanceType);
        }

        /// <summary>
        /// Attempts to deserialize a value from a serialized state.
        /// シリアライズされた状態から値を逆シリアライズしようとします。
        /// *シリアライズとは、stringとかbyte[](テキストかバイナリ形式)みたいな形にしてテキスト保存出来る状態にすること。
        /// </summary>
        public fsResult TryDeserialize(fsData data, Type storageType, ref object result) {
            return TryDeserialize(data, storageType, null, ref result);
        }

        /// <summary>
        /// Attempts to deserialize a value from a serialized state.
        /// シリアライズされた状態から値を逆シリアライズしようとします。
        ///  *シリアライズとは、stringとかbyte[](テキストかバイナリ形式)みたいな形にしてテキスト保存出来る状態にすること。
        /// </summary>
        public fsResult TryDeserialize(fsData data, Type storageType, Type overrideConverterType, ref object result) {
            if (data.IsNull) {
                result = null;
                var processors = GetProcessors(storageType);
                Invoke_OnBeforeDeserialize(processors, storageType, ref data);
                Invoke_OnAfterDeserialize(processors, storageType, null);
                return fsResult.Success;
            }

            // Convert legacy data into modern style data
            //従来のデータを最新のスタイルデータに変換する
            ConvertLegacyData(ref data);

            try {
                // We wrap the entire deserialize call in a reference group so that we can properly
                // deserialize a "parallel" set of references, ie, a list of objects that are cyclic
                // w.r.t. the list
                //私たちは参照グループ内のデシリアライズ・コール全体をラップして、参照の「パラレル」セット、つまり周期的なオブジェクトリストを適切に非直列化することができます。 リスト
                _references.Enter();

                List<fsObjectProcessor> processors;
                var r = InternalDeserialize_1_CycleReference(overrideConverterType, data, storageType, ref result, out processors);
                if (r.Succeeded) {
                    Invoke_OnAfterDeserialize(processors, storageType, result);
                }
                return r;
            }
            finally {
                _references.Exit();
            }
        }

        private fsResult InternalDeserialize_1_CycleReference(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors) {
            // We handle object references first because we could be deserializing a cyclic type that is
            // inherited. If that is the case, then if we handle references after inheritances we will try
            // to create an object instance for an abstract/interface type.
            //継承された循環型を逆シリアル化することができるので、最初にオブジェクト参照を処理します。 その場合、継承の後に参照を処理する場合、抽象/インタフェース型のオブジェクトインスタンスを作成しようとします。

            // While object construction should technically be two-pass, we can do it in
            // one pass because of how serialization happens. We traverse the serialization
            // graph in the same order during serialization and deserialization, so the first
            // time we encounter an object it'll always be the definition. Any times after that
            // it will be a reference. Because of this, if we encounter a reference then we
            // will have *always* already encountered the definition for it.
            // オブジェクトの構築は技術的には2パスでなければならないが、シリアライゼーションの仕組みのために1パスで行うことができる。 シリアライゼーションとデシリアライゼーションでは、シリアライゼーショングラフを同じ順序でトラバースします。最初にオブジェクトに遭遇したときには常にその定義になります。 それ以降はいつでも参考になります。 このため、参照に遭遇した場合は、*常に*定義済みです。
            if (IsObjectReference(data)) {
                int refId = int.Parse(data.AsDictionary[Key_ObjectReference].AsString);
                result = _references.GetReferenceObject(refId);
                processors = GetProcessors(result.GetType());
                return fsResult.Success;
            }

            return InternalDeserialize_2_Version(overrideConverterType, data, storageType, ref result, out processors);
        }

        private fsResult InternalDeserialize_2_Version(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors) {
            if (IsVersioned(data)) {
                // data is versioned, but we might not need to do a migration
                //データはバージョン管理されていますが、移行は必要ありません
                string version = data.AsDictionary[Key_Version].AsString;

                fsOption<fsVersionedType> versionedType = fsVersionManager.GetVersionedType(storageType);
                if (versionedType.HasValue &&
                    versionedType.Value.VersionString != version) {

                    // we have to do a migration
                    //私たちはマイグレーションをしなくてはなりません
                    var deserializeResult = fsResult.Success;

                    List<fsVersionedType> path;
                    deserializeResult += fsVersionManager.GetVersionImportPath(version, versionedType.Value, out path);
                    if (deserializeResult.Failed) {
                        processors = GetProcessors(storageType);
                        return deserializeResult;
                    }

                    // deserialize as the original type
                    //元の型として非直列化する
                    deserializeResult += InternalDeserialize_3_Inheritance(overrideConverterType, data, path[0].ModelType, ref result, out processors);
                    if (deserializeResult.Failed) return deserializeResult;

                    // TODO: we probably should be invoking object processors all along this pipeline
                    //TODO：おそらくこのパイプラインに沿ってオブジェクトプロセッサを呼び出すべきです
                    for (int i = 1; i < path.Count; ++i) {
                        result = path[i].Migrate(result);
                    }

                    // Our data contained an object definition ($id) that was added to _references in step 4.
                    // However, in case we are doing versioning, it will contain the old version.
                    // To make sure future references to this object end up referencing the migrated version,
                    // we must update the reference.
                    //私たちのデータには、手順4で_referencesに追加されたオブジェクト定義（$ id）が含まれていました。
                    //ただし、バージョン管理を行っている場合は古いバージョンが含まれます。このオブジェクトへの今後の参照が移行されたバージョンを参照するようにするには、参照を更新する必要があります。
                    if (IsObjectDefinition(data)) {
                        int sourceId = int.Parse(data.AsDictionary[Key_ObjectDefinition].AsString);
                        _references.AddReferenceWithId(sourceId, result);
                    }

                    processors = GetProcessors(deserializeResult.GetType());
                    return deserializeResult;
                }
            }

            return InternalDeserialize_3_Inheritance(overrideConverterType, data, storageType, ref result, out processors);
        }

        private fsResult InternalDeserialize_3_Inheritance(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors) {
            var deserializeResult = fsResult.Success;

            Type objectType = storageType;

            // If the serialized state contains type information, then we need to make sure to update our
            // objectType and data to the proper values so that when we construct an object instance later
            // and run deserialization we run it on the proper type.
            // シリアライズされた状態に型情報が含まれている場合は、objectTypeとデータを適切な値に更新して、後でオブジェクトインスタンスを作成して逆シリアル化を実行するときに適切な型で実行する必要があります。
            if (IsTypeSpecified(data)) {
                fsData typeNameData = data.AsDictionary[Key_InstanceType];

                // we wrap everything in a do while false loop so we can break out it
                //我々はそれを打ち破ることができるので、我々は偽のループ中にすべてをラップする
                do{
                    if (typeNameData.IsString == false) {
                        deserializeResult.AddMessage(Key_InstanceType + " value must be a string (in " + data + ")");
                        break;
                    }

                    string typeName = typeNameData.AsString;
                    Type type = fsTypeCache.GetType(typeName);
                    if (type == null) {
                        deserializeResult.AddMessage("Unable to locate specified type \"" + typeName + "\"");
                        break;
                    }

                    if (storageType.IsAssignableFrom(type) == false) {
                        deserializeResult.AddMessage("Ignoring type specifier; a field/property of type " + storageType + " cannot hold an instance of " + type);
                        break;
                    }

                    objectType = type;
                } while (false);
            }

            // We wait until here to actually Invoke_OnBeforeDeserialize because we do not
            // have the correct set of processors to invoke until *after* we have resolved
            // the proper type to use for deserialization.
            // 実際にInvoke_OnBeforeDeserializeするまでここまで待機するのは、デシリアライズに使用する適切な型を*解決してから*に達するまで呼び出す正しいプロセッサセットがないからです。
            processors = GetProcessors(objectType);
            Invoke_OnBeforeDeserialize(processors, storageType, ref data);

            // Construct an object instance if we don't have one already. We also need to construct
            // an instance if the result type is of the wrong type, which may be the case when we
            // have a versioned import graph.
            // すでにインスタンスインスタンスが存在しない場合、オブジェクトインスタンスを構築します。 また、結果の型が間違った型である場合には、インスタンスを構築する必要があります。これは、バージョン管理されたインポートグラフがある場合があります。
            if (ReferenceEquals(result, null) || result.GetType() != objectType) {
                result = GetConverter(objectType, overrideConverterType).CreateInstance(data, objectType);
            }

            // We call OnBeforeDeserializeAfterInstanceCreation here because we still want to invoke the
            // method even if the user passed in an existing instance.
            //ここではOnBeforeDeserializeAfterInstanceCreationを呼び出します。これは、ユーザーが既存のインスタンスを渡してもメソッドを呼び出すためです。
            Invoke_OnBeforeDeserializeAfterInstanceCreation(processors, storageType, result, ref data);

            // NOTE: It is critically important that we pass the actual objectType down instead of
            //       using result.GetType() because it is not guaranteed that result.GetType()
            //       will equal objectType, especially because some converters are known to
            //       return dummy values for CreateInstance() (for example, the default behavior
            //       for structs is to just return the type of the struct).
            //注：実際には、result.GetType（）を使用する代わりに実際のobjectTypeを渡すことが重要です。なぜなら、Result.GetType（）はobjectTypeと等しくなります。特に、一部のコンバーターはCreateInstance（） （例えば、structのデフォルトの振る舞いは、structの型を返すだけです）。

            return deserializeResult += InternalDeserialize_4_Cycles(overrideConverterType, data, objectType, ref result);
        }

        private fsResult InternalDeserialize_4_Cycles(Type overrideConverterType, fsData data, Type resultType, ref object result) {
            if (IsObjectDefinition(data)) {
                // NOTE: object references are handled at stage 1
                //注：オブジェクト参照はステージ1で処理されます。

                // If this is a definition, then we have a serialization invariant that this is the
                // first time we have encountered the object (TODO: verify in the deserialization logic)
                //これが定義の場合は、オブジェクトに遭遇したのは初めての逐次化不変式です（TODO：非直列化論理で検証する）

                // Since at this stage in the deserialization process we already have access to the
                // object instance, so we just need to sync the object id to the references database
                // so that when we encounter the instance we lookup this same object. We want to do
                // this before actually deserializing the object because when deserializing the object
                // there may be references to itself.
                // デシリアライズ処理のこの段階ではすでにオブジェクトインスタンスにアクセスしているので、オブジェクトIDを参照データベースに同期させるだけで、インスタンスに出会ったときに同じオブジェクトを参照するようにするだけです。 オブジェクトをデシリアライズするときに、オブジェクト自体を参照する可能性があるため、オブジェクトを実際にデシリアライズする前にこれを行う必要があります。

                int sourceId = int.Parse(data.AsDictionary[Key_ObjectDefinition].AsString);
                _references.AddReferenceWithId(sourceId, result);
            }

            // Nothing special, go through the standard deserialization logic.
            //特別なものはなく、標準的なデシリアライズロジックを実行します。
            return InternalDeserialize_5_Converter(overrideConverterType, data, resultType, ref result);
        }

        private fsResult InternalDeserialize_5_Converter(Type overrideConverterType, fsData data, Type resultType, ref object result) {
            if (IsWrappedData(data)) {
                data = data.AsDictionary[Key_Content];
            }

            return GetConverter(resultType, overrideConverterType).TryDeserialize(data, ref result, resultType);
        }
    }
}