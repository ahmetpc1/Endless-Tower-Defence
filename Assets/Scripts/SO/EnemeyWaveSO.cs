using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemeyWaveSO", menuName = "SO/EnemeyWaveSO")]
public class EnemeyWaveSO : ScriptableObject
{
    // wave ile ilgili bilgileri saklayacagimiz so  ornegin dusman turu ,kac kisi gelecek vb

    public EnemyDataSO bearData;
    public EnemyDataSO mummyData;

    public int mummyCount;
    public int bearCount;

    public float bearSpawnSpeed;
    public float mummySpawnSpeed;
    public float spawnIntervalTime;

    public float bearIncrementRate;
    public float mummyIncrementRate;



}
