using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Object pools are useful for optimization purposes. They allow retrieving game objects from a
/// pre-allocated pool, which avoids the need to instantiate them at runtime (a potentially costly
/// operation). They are particularly useful for handling sets consisting of many small objects
/// like particle systems or audio SFX.
/// �I�u�W�F�N�g�v�[���́A�œK���̖ړI�ɖ𗧂��܂��B ���O���蓖�ăv�[������Q�[���I�u�W�F�N�g�����o�����Ƃ��ł��邽�߁A
/// ���s���ɃC���X�^���X������K�v������܂���i���ݓI�ɃR�X�g�̂����鑀��j�B 
/// �p�[�e�B�N���V�X�e����I�[�f�B�ISFX�̂悤�ȑ����̏����ȃI�u�W�F�N�g����Ȃ�Z�b�g����������ꍇ�ɓ��ɕ֗��ł��B
/// </summary>
public class ObjectPool : MonoBehaviour
{
    public GameObject Prefab;
    public int InitialSize = 16;

    private List<GameObject> instances = new List<GameObject>();

    private void Start()
    {
        for (var i = 0; i < InitialSize; i++)
        {
            var clone = CreateInstance();
            clone.transform.parent = transform;
            clone.SetActive(false);
        }
    }

    private GameObject CreateInstance()
    {
        var clone = Instantiate(Prefab, Vector3.zero, Quaternion.identity) as GameObject;
        clone.transform.parent = transform;
        instances.Add(clone);
        return clone;
    }

    public GameObject GetObject()
    {
        foreach (var instance in instances)
        {
            if (instance.activeSelf != true)
            {
                instance.SetActive(true);
                return instance;
            }
        }
        return CreateInstance();
    }
}