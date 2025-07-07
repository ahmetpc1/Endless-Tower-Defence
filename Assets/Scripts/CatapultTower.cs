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

    public Transform targetEnemy=null;
    Coroutine currentCoroutine=null;
    [SerializeField] float cooldown;
    float lastShootTime;

    public bool canShoot = false;
    public bool CanShoot { get => canShoot; set { canShoot = value; } }

    public IEnumerator fireToEnemy(Transform enemy)
    {
        while (enemy != null&&Time.time>= cooldown +lastShootTime)
        {
            lastShootTime = Time.time;
            GameObject bullet = Instantiate(bulletObject, bulletSpawnPoint.position, Quaternion.identity);
            Bullet bulletScripte = bullet.GetComponent<Bullet>();
            bulletScripte.target = enemy;
            float randomCooldown = UnityEngine.Random.Range(3.0f, 3.6f);
            yield return new WaitForSeconds(randomCooldown);
        }
    }

    public void UpgradeTower()
    {
        if (GameManager.instance.goldCount >= upgradePrice)
        {
            GameManager.instance.ShowUpgradeVfx();

            GameManager.instance.ChangeGoldCount(-upgradePrice);
            BulletAreaDamage.areaDamage++;
        }
        else
        {
            Debug.Log("OLMADI");
        }
    }
    
    private void Update()
    {
        if (targetEnemy!=null) { 
       Vector3 dir = (targetEnemy.position-catapultBody.position).normalized;
       dir.y=0;
       Quaternion lookRot = Quaternion.LookRotation(dir);
       catapultBody.transform.rotation = Quaternion.Slerp(catapultBody.transform.rotation, lookRot, 10f * Time.fixedDeltaTime);
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" &&  targetEnemy == null&& canShoot)
        {
            targetEnemy = other.transform;
            currentCoroutine= StartCoroutine(fireToEnemy(other.transform));
            
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && targetEnemy == null&& canShoot)
        {
            targetEnemy = other.transform;
            currentCoroutine = StartCoroutine(fireToEnemy(other.transform));

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && other.transform == targetEnemy&& canShoot)
        {
            targetEnemy = null;
            if (currentCoroutine!=null) 
            {
            StopCoroutine(currentCoroutine);

            }
        }

    }
}
