using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform target;
    Rigidbody rb;
    [SerializeField]float speed=10f;
    public int damage;
    


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        

    }
    private void FixedUpdate()
    {
        if (target == null || target.Equals(null))
        {
            Destroy(gameObject);
        }
        else
        {
            Vector3 direction = (target.position - transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.fixedDeltaTime);

            transform.position += transform.forward * speed * Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Enemy") 
        {
            other.GetComponent<Enemy>().DecreaseHealth(damage); 
            Destroy(gameObject);
        }
    }
    
}
