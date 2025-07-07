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
    public ParticleSystem upgradeParticale;


    [Space(10)]
    [Header(" ENEMY")]
    [SerializeField]
    public Transform enemyPool;
    public Vector3 enemySpawnPoint;
    public Vector3 endPoint;
    public GameObject enemyObject;
    [SerializeField]
    EnemeyWaveSO enemeyWaveSO;
    public ParticleSystem EnemyDeathVfx;

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
    [SerializeField] TextMeshProUGUI scoreText;
    Vector2 originalPos;
    int currentFaceID=3;

    [Space(10)]
    [Header("GOLD")]
    [SerializeField] TextMeshProUGUI goldText;
    public int goldCount;
    [SerializeField] int initialGoldCount;

    [Space(10)]
    [Header("AUDIO")]
    public AudioClip ArrowHit; 
    public AudioClip rockHit;
    public AudioClip enemyDeath;

    AudioSource audioSource1, audioSource2,audioSource3;
    
    public int score=0;

    public ObjectPool enemyDeathVfxPool;
    public ObjectPool AoEVfxPool;

    private void Awake()
    {
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource3 = gameObject.AddComponent<AudioSource>();

        audioSource2.volume = 0.65f;
        audioSource1.volume = 1f;
        audioSource3.volume = 1f;


        audioSource1.playOnAwake = false;
        audioSource2.playOnAwake = false;
        audioSource3.playOnAwake = false;


        audioSource1.clip = ArrowHit;
        audioSource2.clip = rockHit;
        audioSource3.clip = enemyDeath;
    }
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

        SetEnemyWaveDefault(3,7,5);//ilk wave kac dusman saldiracak
        ChangeAllButtonsAlpha(false);
        ChangeGoldCount(initialGoldCount);
        GameOverMenu.SetActive(false);
     
        originalPos = kingFace.rectTransform.anchoredPosition;

        CloseUpgradeVfx();
        enemyDeathVfxPool.prefab = EnemyDeathVfx.gameObject;
        healthText.text = $"{currentPlayerHealth} / {maxPlayerHealth}";
       
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
            ShowPressFade(archerTowerBtn,CatapultTowerBtn, showPlaceHolderTowers);
        currentPlaceHolder = placeHolderArcherTowerObject;
            currentTowerType = TowerType.archerTower;
            if (showPlaceHolderTowers)
                TowerRangeCircleMove.instance?.SetRangeScale(archerTowerRange);
        }
    }
    public void CatapultTowerButton()
    {
        if (isGameStart)
        {
            showPlaceHolderTowers = !showPlaceHolderTowers;
            ShowPressFade(CatapultTowerBtn,archerTowerBtn , showPlaceHolderTowers);

            currentPlaceHolder = placeHolderCatapultTowerObject;
            currentTowerType = TowerType.catapultTower;
            if(showPlaceHolderTowers)
            TowerRangeCircleMove.instance?.SetRangeScale(catapultTowerRange);
        }
    }

    void ShowPressFade(Button pressedButton,Button normalButton,bool flag) 
    {
        if (flag)
        {
            changeButtonAlpha(pressedButton, 0.25f);
            changeButtonAlpha(normalButton, 1f);

        }
        else 
        {
            changeButtonAlpha(normalButton, 1f);
            changeButtonAlpha(pressedButton, 1f);

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
        int enemyHealthFactor = enemeyWaveSO.bearCount + enemeyWaveSO.mummyCount;
        enemeyWaveSO.bearCount = Mathf.Max((int)(enemeyWaveSO.bearCount * enemeyWaveSO.bearIncrementRate), enemeyWaveSO.bearCount+1);
        enemeyWaveSO.mummyCount = Mathf.Max((int)(enemeyWaveSO.mummyCount * enemeyWaveSO.mummyIncrementRate), enemeyWaveSO.mummyCount + 1);
        
        enemeyWaveSO.bearData.maxHealth+= enemyHealthFactor*2/3;
        enemeyWaveSO.mummyData.maxHealth+= enemyHealthFactor/3;

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
        scoreText.text ="Score: " +score.ToString();
        GameOverMenu.transform.DOScale(1f, 4f).SetUpdate(true);
    }

    #region buttons
    public void RestartButton()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene(1);
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
        if (!CalculateKingFaceRate()) { return; }

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
    public void ShowUpgradeVfx() 
    {
        upgradeParticale.Play();
        Invoke("CloseUpgradeVfx", 2f);


    }
    private void CloseUpgradeVfx()
    {
        upgradeParticale.Stop();
    }
    public void ShowDeathVfx(Transform parent)
    {
        GameObject obj = enemyDeathVfxPool.GetObject();
        if (obj == null) return;

        obj.transform.position = parent.position;
        
        obj.transform.rotation = Quaternion.identity;
    }
    public void ShowAoEVfx(Vector3 location)
    {
        GameObject obj = AoEVfxPool.GetObject();
        if (obj == null) return;

        obj.transform.position = location;

        obj.transform.rotation = Quaternion.identity;
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
        int faceID = Mathf.Clamp((int)Mathf.Round(rate-1),0,3);
        kingFace.sprite = kingFaces[faceID];
        
    }
    public bool CalculateKingFaceRate() 
    {
        float rate = (float)currentPlayerHealth / maxPlayerHealth;
        rate *= 4;
        int faceID = Mathf.Clamp((int)Mathf.Round(rate - 1), 0, 3);
        if (currentFaceID != faceID)
        {
            currentFaceID = faceID;
            return true;
        }
        else
            return false;
    }
    public void ChangeGoldCount(int amount)//negatýf veya pozýtýf degerler alabýlýr,tek fonk yazmak ýcýn bu sekýlde yaptýk
    {
        goldCount += amount;
        goldText.text ="GOLD: " +goldCount.ToString();
    }
    

    public void SetEnemyWaveDefault(int initialEnemyCount,int bearHp,int mummyHp) 
    {
    enemeyWaveSO.bearCount = initialEnemyCount;
    enemeyWaveSO.mummyCount = initialEnemyCount;
        enemeyWaveSO.bearData.maxHealth = bearHp;
        enemeyWaveSO.mummyData.maxHealth = mummyHp;

    }

    #region audio
    public void PlayArrowHitSFX() 
    {
        if (!audioSource1.isPlaying)
        {
            audioSource1.Play();
        }
        
    }
    public void PlayRockHitSFX()
    {
        if (!audioSource2.isPlaying)
        {
            audioSource2.Play();
        }

    }
    public void PlayEnemyDeathSFX()
    {
        if (!audioSource3.isPlaying)
        {
            audioSource3.Play();
        }

    }
    #endregion
}
