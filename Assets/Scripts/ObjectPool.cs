using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public Queue<GameObject> pool = new Queue<GameObject>();
    public float time = 1f;
    public GameObject GetObject()
    {
        GameObject prefabObject;
        if (pool.Count>0) 
        {
            prefabObject = pool.Dequeue();
            prefabObject.SetActive(true);
        StartCoroutine(SetObject(prefabObject));
        return prefabObject;
        }
        prefabObject = Instantiate(prefab);
        StartCoroutine(SetObject(prefabObject));
        return prefabObject;
    }
    public IEnumerator SetObject(GameObject prefabObject) 
    {

        yield return new WaitForSeconds(time);
        pool.Enqueue(prefabObject);
        prefabObject.SetActive(false);

    }
}
