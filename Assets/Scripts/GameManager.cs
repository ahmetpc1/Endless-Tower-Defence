using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool showPlaceHolderTowers =false; // kule dikmek icin butona tiklandiginda tile uzerinde placeholder obje gozukecek mi
    public GameObject placeHolderArcherTowerObject=null;
    public GameObject placeHolderCatapultTowerObject = null;
    public GameObject currentPlaceHodler = null;

    [HideInInspector]
    public bool isGameStart=false;
    [HideInInspector]
    public bool isWaveEnd;

    [SerializeField]
    public Transform enemyPool;

    public Vector3 enemySpawnPoint;
    public Vector3 endPoint;

    int enemyCount=5;
    public GameObject enemyObject;

    [SerializeField]
    EnemeyWaveSO enemeyWaveSO;
    void Start()
    {
        if (instance!=null&&instance!=this)
        {
        Destroy(this);
        }
        instance = this;

        isWaveEnd=true;

        createTowerObjects();

        enemySpawnPoint=TileManager.instance.startTile.transform.position;
        enemySpawnPoint.y=0.65f;
        endPoint = TileManager.instance.endTile.transform.position;
        
    }
    #region Towers

    void createTowerObjects()
    {
        GameObject towerObject = Instantiate(placeHolderArcherTowerObject);
        towerObject.transform.SetParent(transform);
        placeHolderArcherTowerObject = towerObject;
        placeHolderArcherTowerObject.SetActive(false);

        towerObject = Instantiate(placeHolderCatapultTowerObject);
        towerObject.transform.SetParent(transform);
        placeHolderCatapultTowerObject = towerObject;
        placeHolderCatapultTowerObject.SetActive(false);

    }
    public void ArcherTowerButton()
    {
        showPlaceHolderTowers = !showPlaceHolderTowers;
        currentPlaceHodler = placeHolderArcherTowerObject;
    }
    public void CatapultTowerButton()
    {
        showPlaceHolderTowers = !showPlaceHolderTowers;
        currentPlaceHodler = placeHolderCatapultTowerObject;
    }
    #endregion 

   
    IEnumerator StartWave() 
    {
       
            for (int i = 0; i < enemeyWaveSO.bearCount; i++) 
            {
            GameObject bear = Instantiate(enemeyWaveSO.bearData.enemyObject,enemySpawnPoint,Quaternion.identity,enemyPool);
            yield return new WaitForSeconds(enemeyWaveSO.bearSpawnSpeed);
            }
            yield return new WaitForSeconds(enemeyWaveSO.spawnIntervalTime);
            for (int i = 0; i < enemeyWaveSO.mummyCount; i++)
            {
                GameObject mummy = Instantiate(enemeyWaveSO.mummyData.enemyObject, enemySpawnPoint, Quaternion.identity, enemyPool);
                yield return new WaitForSeconds(enemeyWaveSO.mummySpawnSpeed);
            }

        UpdateWaveData();
    }

    void UpdateWaveData() //dusmanlarýn sayýsýnýn artacagý , zorlugun artacagý metot
    {
        enemeyWaveSO.bearCount = Mathf.Max((int)(enemeyWaveSO.bearCount * enemeyWaveSO.bearIncrementRate), enemeyWaveSO.bearCount+1);
        enemeyWaveSO.mummyCount = Mathf.Max((int)(enemeyWaveSO.mummyCount * enemeyWaveSO.mummyIncrementRate), enemeyWaveSO.mummyCount + 1);

    }

    public void StartWaveButton()
    {

        if (isGameStart&&isWaveEnd) 
        {
        StartCoroutine(StartWave());
        }
    }
}
