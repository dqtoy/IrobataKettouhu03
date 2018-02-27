using System;
using System.Reflection;

namespace FullSerializer.Internal {
    /// <summary>
    /// A property or field on a MetaType. This unifies the FieldInfo and PropertyInfo classes.
    /// MetaTypeのプロパティまたはフィールド。 これにより、FieldInfoおよびPropertyInfoクラスが統一されます。
    /// </summary>
    public class fsMetaProperty {
        internal fsMetaProperty(fsConfig config, FieldInfo field) {
            _memberInfo = field;
            StorageType = field.FieldType;
            MemberName = field.Name;
            IsPublic = field.IsPublic;
            IsReadOnly = field.IsInitOnly;
            CanRead = true;
            CanWrite = true;

            CommonInitialize(config);
        }

        internal fsMetaProperty(fsConfig config, PropertyInfo property) {
            _memberInfo = property;
            StorageType = property.PropertyType;
            MemberName = property.Name;
            IsPublic = (property.GetGetMethod() != null && property.GetGetMethod().IsPublic) &&
                       (property.GetSetMethod() != null && property.GetSetMethod().IsPublic);
            IsReadOnly = false;
            CanRead = property.CanRead;
            CanWrite = property.CanWrite;

            CommonInitialize(config);
        }

        private void CommonInitialize(fsConfig config) {
            var attr = fsPortableReflection.GetAttribute<fsPropertyAttribute>(_memberInfo);
            if (attr != null) {
                JsonName = attr.Name;
                OverrideConverterType = attr.Converter;
            }

            if (string.IsNullOrEmpty(JsonName)) {
                JsonName = config.GetJsonNameFromMemberName(MemberName, _memberInfo);
            }
        }

        /// <summary>
        /// Internal handle to the reflected member.
        /// 反射された部材の内部ハンドル。
        /// </summary>
        private MemberInfo _memberInfo;

        /// <summary>
        /// The type of value that is stored inside of the property. For example, for an int field,
        /// StorageType will be typeof(int).
        /// プロパティの内部に格納される値の型。 たとえば、intフィールドの場合、StorageTypeはtypeof（int）になります。
        /// </summary>
        public Type StorageType {
            get;
            private set;
        }

        /// <summary>
        /// A custom fsBaseConverter instance to use for this field/property, if requested. This will be
        /// null if the default converter selection algorithm should be used. This is specified using the
        /// [fsObject] annotation with the Converter field.
        /// 要求された場合、このフィールド/プロパティに使用するカスタムfsBaseConverterインスタンス。 
        /// デフォルトのコンバータ選択アルゴリズムを使用する必要がある場合は、この値はnullになります。 これは[fsObject]アノテーションと[コンバーター]フィールドを使用して指定します。
        /// </summary>
        public Type OverrideConverterType {
            get;
            private set;
        }

        /// <summary>
        /// Can this property be read?
        /// このプロパティを読み取ることはできますか？
        /// </summary>
        public bool CanRead {
            get;
            private set;
        }

        /// <summary>
        /// Can this property be written to?
        /// このプロパティを書き込めますか？
        /// </summary>
        public bool CanWrite {
            get;
            private set;
        }

        /// <summary>
        /// The serialized name of the property, as it should appear in JSON.
        /// JSONに表示されるプロパティのシリアル化された名前。
        /// </summary>
        public string JsonName {
            get;
            private set;
        }

        /// <summary>
        /// The name of the actual member.
        /// 実際のメンバーの名前。
        /// </summary>
        public string MemberName {
            get;
            private set;
        }

        /// <summary>
        /// Is this member public?
        /// このメンバーは公開されていますか？
        /// </summary>
        public bool IsPublic {
            get;
            private set;
        }

        /// <summary>
        /// Is this type readonly? We can modify readonly properties using reflection, but not
        /// using generated C#.
        /// このタイプは読み込み専用ですか？ リフレクションを使用して読み取り専用プロパティを変更できますが、生成されたC＃を使用することはできません。
        /// </summary>
        public bool IsReadOnly {
            get; private set;
        }

        /// <summary>
        /// Writes a value to the property that this MetaProperty represents, using given object
        /// instance as the context.
        /// 指定されたオブジェクトインスタンスをコンテキストとして使用して、このMetaPropertyが表すプロパティに値を書き込みます。
        /// </summary>
        public void Write(object context, object value) {
            FieldInfo field = _memberInfo as FieldInfo;
            PropertyInfo property = _memberInfo as PropertyInfo;

            if (field != null) {
                field.SetValue(context, value);
            }

            else if (property != null) {
                MethodInfo setMethod = property.GetSetMethod(/*nonPublic:*/ true);
                if (setMethod != null) {
                    setMethod.Invoke(context, new object[] { value });
                }
            }
        }

        /// <summary>
        /// Reads a value from the property that this MetaProperty represents, using the given
        /// object instance as the context.
        /// 指定されたオブジェクトインスタンスをコンテキストとして使用して、このMetaPropertyが表すプロパティから値を読み取ります。
        /// </summary>
        public object Read(object context) {
            if (_memberInfo is PropertyInfo) {
                return ((PropertyInfo)_memberInfo).GetValue(context, new object[] { });
            }

            else {
                return ((FieldInfo)_memberInfo).GetValue(context);
            }
        }
    }
}