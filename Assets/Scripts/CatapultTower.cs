using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CatapultTower : MonoBehaviour,ITower
{

    [SerializeField] GameObject bulletObject;
    [SerializeField] int upgradePrice;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] Transform catapultBody;
    int bulletDamage = 1;

    [SerializeField] Transform targetEnemy;
    
    public bool isLocked = false;

    public IEnumerator fireToEnemy(Transform enemy)
    {
        while (enemy != null&& isLocked)
        {
            GameObject bullet = Instantiate(bulletObject, bulletSpawnPoint.position, Quaternion.identity);
            Bullet bulletScripte = bullet.GetComponent<Bullet>();
            bulletScripte.damage = bulletDamage;
            bulletScripte.target = enemy;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void UpgradeTower()
    {
        if (GameManager.instance.goldCount >= upgradePrice)
        {
            GameManager.instance.ChangeGoldCount(-upgradePrice);
            bulletDamage++;
        }
        else
        {
            Debug.Log("OLMADI");
        }
    }
    Vector3 gizmoVecDirTest;
    private void Update()
    {
        if (isLocked&& targetEnemy != null) 
        {
        Vector3 dir= targetEnemy.transform.position-catapultBody.position;
            //dir.y = catapultBody.position.y;
            gizmoVecDirTest = dir;
        catapultBody.LookAt(dir);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(catapultBody.position, gizmoVecDirTest);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && !isLocked && targetEnemy == null)
        {
            targetEnemy = other.transform;
            StartCoroutine(fireToEnemy(other.transform));
            isLocked = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && isLocked && other.transform == targetEnemy)
        {
            targetEnemy = null;
            isLocked = false;
            StopCoroutine(fireToEnemy(other.transform));
        }

    }
}
