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
            transform.position = Vector3.MoveTowards(transform.position,target.position,Time.fixedDeltaTime* speed);

            Vector3 enemyPos = target.position - transform.position;
            float angle = Vector3.SignedAngle(transform.position, enemyPos, transform.forward);
            transform.Rotate(-90f, 0f, angle);


            //Vector3 direction = target.position - transform.position;
            //var rot = Quaternion.LookRotation(direction);
            //rb.MoveRotation(rot);
            //rb.velocity = direction * Time.fixedDeltaTime * arrowSpeed;
            //transform.rotation = Quaternion.LookRotation(direction);
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
