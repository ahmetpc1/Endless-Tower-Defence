using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public enum TowerType {archerTower,catapultTower,none }
    public static GameManager instance;
    [Space(10)]
    [Header(" TOWERS")]
    public TowerType currentTowerType = TowerType.none;
    public bool showPlaceHolderTowers = false; // kule dikmek icin butona tiklandiginda tile uzerinde placeholder obje gozukecek mi
    public GameObject placeHolderArcherTowerObject = null;
    public GameObject placeHolderCatapultTowerObject = null;
    public GameObject currentPlaceHolder = null;
    public int archerTowerPrice, CatapultTowerPrice;
    [HideInInspector]
    public bool isGameStart = false;
    [HideInInspector]
    public bool isWaveEnd;
    public GameObject TowerRangeCircle;
    public float archerTowerRange, catapultTowerRange;
    public float upgradeTickTime;
    [Space(10)]
    [Header(" ENEMY")]
    [SerializeField]
    public Transform enemyPool;
    public Vector3 enemySpawnPoint;
    public Vector3 endPoint;
    int enemyCount = 5;
    public GameObject enemyObject;
    [SerializeField]
    EnemeyWaveSO enemeyWaveSO;

    [Space(10)]
    [Header(" HEALTH")]
    [SerializeField]
    int maxPlayerHealth;
    int currentPlayerHealth;
    [SerializeField] TextMeshProUGUI healthText;

    [Space(10)]
    [Header("UI COMPONENTS")]
    [SerializeField] Sprite[] kingFaces;
    [SerializeField] Image kingFace;
    [SerializeField] Button startWaveBtn, archerTowerBtn, CatapultTowerBtn;
    [SerializeField] Sprite kingDeadFace;
    public Image UpgradeTimerImage;
    public Image UpgradeTimerParent;
    public RectTransform canvas;
    public GameObject GameOverMenu;
    public GameObject InfoMenu;

    Vector2 originalPos;


    [Space(10)]
    [Header("GOLD")]
    [SerializeField] TextMeshProUGUI goldText;
    public int goldCount;
    [SerializeField] int initialGoldCount;





    void Start()
    {

        if (instance!=null&&instance!=this)
        {
        Destroy(this);
        }
        instance = this;

        isWaveEnd=true;

        createTowerObjects();

        currentPlayerHealth= maxPlayerHealth;
        enemySpawnPoint=TileManager.instance.startTile.transform.position;
        enemySpawnPoint.y=0.65f;
        endPoint = TileManager.instance.endTile.transform.position;

        SetEnemyWaveDefault(3);//ilk wave kac dusman saldiracak
        ChangeAllButtonsAlpha(false);
        ChangeGoldCount(initialGoldCount);
        GameOverMenu.SetActive(false);

        originalPos= kingFace.rectTransform.anchoredPosition;
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
        if (isGameStart) { 
        showPlaceHolderTowers = !showPlaceHolderTowers;
        currentPlaceHolder = placeHolderArcherTowerObject;
            currentTowerType = TowerType.archerTower;
            TowerRangeCircleMove.instance?.SetRangeScale(archerTowerRange);
        }
    }
    public void CatapultTowerButton()
    {
        if (isGameStart)
        {
            showPlaceHolderTowers = !showPlaceHolderTowers;
            currentPlaceHolder = placeHolderCatapultTowerObject;
            currentTowerType = TowerType.catapultTower;
            TowerRangeCircleMove.instance?.SetRangeScale(catapultTowerRange);
        }
    }
    #endregion 

   
    IEnumerator StartWave() 
    {
        
        isWaveEnd =false;
        changeButtonAlpha(startWaveBtn,0.25f);
        
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
        isWaveEnd=true;
        changeButtonAlpha(startWaveBtn, 1f);

    }

    void UpdateWaveData() //dusmanlarýn sayýsýnýn artacagý , zorlugun artacagý metot
    {
        enemeyWaveSO.bearCount = Mathf.Max((int)(enemeyWaveSO.bearCount * enemeyWaveSO.bearIncrementRate), enemeyWaveSO.bearCount+1);
        enemeyWaveSO.mummyCount = Mathf.Max((int)(enemeyWaveSO.mummyCount * enemeyWaveSO.mummyIncrementRate), enemeyWaveSO.mummyCount + 1);

    }

    public void DecreasePlayerHealth(EnemyDataSO enemyDataSO) 
    {
        currentPlayerHealth -= enemyDataSO.damage;
        if (currentPlayerHealth<=0) 
        {
            GameOver();
        }
    }
    void GameOver() 
    {
        GameOverMenu.transform.DOScale(0f,0f);
        Time.timeScale = 0f;
        GameOverMenu.SetActive(true);
        ChangeAllButtonsAlpha(false);
        isGameStart = false;
        GameOverMenu.transform.DOScale(1f, 4f).SetUpdate(true);






    }

    #region buttons
    public void RestartButton()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene(0);
    }
    public void ExitButton()
    {
        Application.Quit();
    }
    void changeButtonAlpha(Button btn,float alpha) 
    {
    Color color = Color.white;
    color.a = alpha;
    btn.GetComponent<Image>().color = color;
    }

    public void ChangeAllButtonsAlpha(bool flag) 
    {
        if (flag)
        {
            changeButtonAlpha(startWaveBtn, 1f);
            changeButtonAlpha(archerTowerBtn, 1f);
            changeButtonAlpha(CatapultTowerBtn, 1f);
        }
        else
        {
            changeButtonAlpha(startWaveBtn, 0.25f);
            changeButtonAlpha(archerTowerBtn, 0.25f);
            changeButtonAlpha(CatapultTowerBtn, 0.25f);
        }
    }

    public void StartWaveButton()
    {

        if (isGameStart && isWaveEnd)
        {
            StartCoroutine(StartWave());
        }
    }
    #endregion

    public void RefreshHealthUI()
    {
        healthText.text = $"{currentPlayerHealth} / {maxPlayerHealth}";
        if (currentPlayerHealth <= 0)
        {
            kingFace.sprite = kingDeadFace;
            kingFace.rectTransform.DOAnchorPos(originalPos, 1.25f).SetUpdate(true);
            return;
        }

        DG.Tweening.Sequence kingFaceSeq = DOTween.Sequence().SetUpdate(true);
        Vector2 goingPos = originalPos;
        Vector2 startPos = originalPos;
        goingPos.x -= 200;
        startPos.x += 200;

        kingFaceSeq.Append(kingFace.rectTransform.DOAnchorPos(goingPos, 1.25f))
     .OnComplete(() =>
     {
         UpdateKingFace();
         kingFace.rectTransform.anchoredPosition = startPos;
         kingFace.rectTransform.DOAnchorPos(originalPos, 1.25f);
     });



    }

    private void UpdateKingFace()
    {
        if (currentPlayerHealth <= 0)
        {
            kingFace.sprite = kingDeadFace;
            return;
        }
        float rate = (float)currentPlayerHealth / maxPlayerHealth;
        rate *= 4;
        kingFace.sprite = kingFaces[Mathf.Clamp((int)rate - 1, 0, 4)];
    }

    public void ChangeGoldCount(int amount)//negatýf veya pozýtýf degerler alabýlýr,tek fonk yazmak ýcýn bu sekýlde yaptýk
    {
        goldCount += amount;
        goldText.text = goldCount.ToString();
    }
    

    public void SetEnemyWaveDefault(int initialEnemyCount) 
    {
    enemeyWaveSO.bearCount = initialEnemyCount;
    enemeyWaveSO.mummyCount = initialEnemyCount;
    }
}
