using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower : MonoBehaviour, ITower
{
    [SerializeField] GameObject arrowObject;
    [SerializeField] int upgradePrice;
    Transform arrowSpawnPoint;
    Transform targetEnemy;
    [SerializeField]int arrowDamage = 1;

    Coroutine currentCoroutine=null;
    void Start()
    {
        arrowSpawnPoint = transform.GetChild(0);
    }
    void Update()
    {

    }

    public IEnumerator fireToEnemy(Transform enemy)
    {
        while (enemy != null)
        {
            GameObject arrow = Instantiate(arrowObject, arrowSpawnPoint.position, Quaternion.identity);
            Arrow arrowScripte = arrow.GetComponent<Arrow>();
            arrowScripte.damage = arrowDamage;
            arrowScripte.target = enemy;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && targetEnemy == null)
        {
            targetEnemy = other.transform;
            currentCoroutine = StartCoroutine(fireToEnemy(other.transform));
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && targetEnemy == null)
        {
            targetEnemy = other.transform;
            currentCoroutine = StartCoroutine(fireToEnemy(other.transform));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy"&& targetEnemy == other.transform  )
        {
            targetEnemy = null;
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);

            }
        }

    }

    public void UpgradeTower()
    {
        if (GameManager.instance.goldCount >= upgradePrice)
        {
            GameManager.instance.ChangeGoldCount(-upgradePrice);
            arrowDamage++;
        }
        else
        {
            Debug.Log("OLMADI");
        }

    }


}
