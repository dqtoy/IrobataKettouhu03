using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Object pools are useful for optimization purposes. They allow retrieving game objects from a
/// pre-allocated pool, which avoids the need to instantiate them at runtime (a potentially costly
/// operation). They are particularly useful for handling sets consisting of many small objects
/// like particle systems or audio SFX.
/// オブジェクトプールは、最適化の目的に役立ちます。 事前割り当てプールからゲームオブジェクトを取り出すことができるため、
/// 実行時にインスタンス化する必要がありません（潜在的にコストのかかる操作）。 
/// パーティクルシステムやオーディオSFXのような多くの小さなオブジェクトからなるセットを処理する場合に特に便利です。
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