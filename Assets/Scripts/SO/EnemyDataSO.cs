using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "SO/EnemyDataSO")]

public class EnemyDataSO : ScriptableObject
{
    public GameObject enemyObject;

    
    [Range(0f, 10f)]
    public float speed = 10f, turnSpeed = 3f;
   
     
     public int damage;
     public int maxHealth;
    public int goldReward;
     


}
