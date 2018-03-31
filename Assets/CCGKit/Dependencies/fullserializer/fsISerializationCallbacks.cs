using System;
#if !NO_UNITY
using UnityEngine;
#endif

#if !UNITY_EDITOR && UNITY_WSA
// For System.Reflection.TypeExtensions
using System.Reflection;
#endif

namespace FullSerializer {
    /// <summary>
    /// Extend this interface on your type to receive notifications about serialization/deserialization events. If you don't
    /// have access to the type itself, then you can write an fsObjectProcessor instead.
    /// �����̃^�C�v�ł��̃C���^�t�F�[�X���g�����āA�V���A���C�[�[�V����/�f�V���A���C�Y�C�x���g�Ɋւ���ʒm���󂯎��܂��B �^���̂ɃA�N�Z�X�ł��Ȃ��ꍇ�́A�����fsObjectProcessor���L�q���邱�Ƃ��ł��܂��B
    /// </summary>
    public interface fsISerializationCallbacks {
        /// <summary>
        /// Called before serialization.
        /// �V���A���C�[�[�V�����̑O�ɌĂяo����܂��B
        /// </summary>
        void OnBeforeSerialize(Type storageType);

        /// <summary>
        /// Called after serialization.
        /// �V���A���C�[�[�V������ɌĂяo����܂��B
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="data">The data that was serialized.</param>
        void OnAfterSerialize(Type storageType, ref fsData data);

        /// <summary>
        /// Called before deserialization.
        /// �t�V���A�����̑O�ɌĂяo����܂��B
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="data">The data that will be used for deserialization.</param>
        void OnBeforeDeserialize(Type storageType, ref fsData data);

        /// <summary>
        /// Called after deserialization.
        /// �f�V���A���C�Y��ɌĂяo����܂��B
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="instance">The type of the instance.</param>
        void OnAfterDeserialize(Type storageType);
    }
}

namespace FullSerializer.Internal {
    public class fsSerializationCallbackProcessor : fsObjectProcessor {
        public override bool CanProcess(Type type) {
            return typeof(fsISerializationCallbacks).IsAssignableFrom(type);
        }

        public override void OnBeforeSerialize(Type storageType, object instance) {
            // Don't call the callback on null instances.
            //�k���C���X�^���X�ŃR�[���o�b�N���Ăяo���Ȃ��ł��������B
            if (instance == null) return;
            ((fsISerializationCallbacks)instance).OnBeforeSerialize(storageType);
        }

        public override void OnAfterSerialize(Type storageType, object instance, ref fsData data) {
            // Don't call the callback on null instances.
            //�k���C���X�^���X�ŃR�[���o�b�N���Ăяo���Ȃ��ł��������B
            if (instance == null) return;
            ((fsISerializationCallbacks)instance).OnAfterSerialize(storageType, ref data);
        }

        public override void OnBeforeDeserializeAfterInstanceCreation(Type storageType, object instance, ref fsData data) {
            if (instance is fsISerializationCallbacks == false) {
                throw new InvalidCastException("Please ensure the converter for " + storageType + " actually returns an instance of it, not an instance of " + instance.GetType());
            }

            ((fsISerializationCallbacks)instance).OnBeforeDeserialize(storageType, ref data);
        }

        public override void OnAfterDeserialize(Type storageType, object instance) {
            // Don't call the callback on null instances.
            if(instance == null) return;
            ((fsISerializationCallbacks)instance).OnAfterDeserialize(storageType);
        }
    }

#if !NO_UNITY
    public class fsSerializationCallbackReceiverProcessor : fsObjectProcessor {
        public override bool CanProcess(Type type) {
            return typeof(ISerializationCallbackReceiver).IsAssignableFrom(type);
        }

        public override void OnBeforeSerialize(Type storageType, object instance) {
            // Don't call the callback on null instances.
            //�k���C���X�^���X�ŃR�[���o�b�N���Ăяo���Ȃ��ł��������B
            if (instance == null) return;
            ((ISerializationCallbackReceiver)instance).OnBeforeSerialize();
        }

        public override void OnAfterDeserialize(Type storageType, object instance) {
            // Don't call the callback on null instances.
            //�k���C���X�^���X�ŃR�[���o�b�N���Ăяo���Ȃ��ł��������B
            if (instance == null) return;
            ((ISerializationCallbackReceiver)instance).OnAfterDeserialize();
        }
    }
#endif
}
