using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRangeCircleMove : MonoBehaviour
{
    [SerializeField] float speed;
    public static TowerRangeCircleMove instance;

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward*Time.deltaTime*speed);
        transform.localPosition = new Vector3(0,0.25f,0);
    }
    public void SetRangeScale(float scale) 
    {
    if(gameObject!=null)
    transform.localScale = new Vector3(scale,scale,scale);
    }
}
