using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAreaDamage : MonoBehaviour
{
    static public int areaDamage=1;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.GetComponent<Enemy>().DecreaseHealth(areaDamage);
        }
    }

    
}
