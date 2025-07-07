using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform target;
    Rigidbody rb;
    [SerializeField] float speed = 10f;
    float limit;
    float offset = 10f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        limit = transform.position.y+offset;

        Vector3 dir = target.position - transform.position;
        dir.y = 2;
        dir = dir.normalized;
        rb.AddForce(dir, ForceMode.Impulse);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            GameManager.instance.PlayRockHitSFX();
          Vector3 location = transform.position;
          location.y = 1;
          GameManager.instance.ShowAoEVfx(location);
            Destroy(gameObject);
        }
    }
}
