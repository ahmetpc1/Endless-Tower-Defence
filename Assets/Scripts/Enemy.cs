using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening.Plugins.Core.PathCore;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    EnemyDataSO enemyDataSO;

    List<Transform> path;
    int pathCounter = 0;

    int currentHealth;

    public Action<EnemyDataSO> onEnemyEnterGate;
    

    void OnEnable() 
    {
        path=TileManager.instance.path;
    }
    void Start() 
    {
     currentHealth = enemyDataSO.maxHealth;
     onEnemyEnterGate += GameManager.instance.DecreasePlayerHealth;
     StartCoroutine(StartWalk());
     
    }
    IEnumerator StartWalk()
    {
        while (pathCounter < path.Count)
        {
            Vector3 target = path[pathCounter].position;
            target.y = transform.position.y;
            Quaternion lookRot = Quaternion.LookRotation(target-transform.position,Vector3.up);//vector3up yukarý yonu belýrlýyor ký unýty kendýsý y eksenýný secmesýn
            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, enemyDataSO.speed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, enemyDataSO.turnSpeed * Time.deltaTime);

                yield return null;
            }

            pathCounter++;
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Gate")
        {
            onEnemyEnterGate.Invoke(enemyDataSO);
            GameManager.instance.RefreshHealthUI();
            Destroy(gameObject);//object pool olana kadar dursun
        }

        
    }

    public void DecreaseHealth(int damage) 
    {
        currentHealth -= damage;
        if (currentHealth<=0&&gameObject!=null)
        {
            Destroy(gameObject);
            GameManager.instance.ChangeGoldCount(enemyDataSO.goldReward);
        }
    }


}
