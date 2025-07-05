using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    bool isFull = false;
    public float f, g, h;
    public Tile parent = null;
    MPBController mpbController;
    [Header("Tower")]
    [Space(10f)]
    GameObject placeHolder;
    ITower tower;
    [Header("Timer")]
    [Space(10f)]
    float passingTime = 0;
    bool isTick=false;
    float upgradeTickTime;
    Image UpgradeTimerImage;
    
    void Start()
    {
        if (transform.GetChild(0)!=null) 
        {
        mpbController = transform.GetChild(0).GetComponent<MPBController>();
        }
        upgradeTickTime = GameManager.instance.upgradeTickTime;
        UpgradeTimerImage = GameManager.instance.UpgradeTimerImage;
        GameManager.instance.UpgradeTimerParent.gameObject.SetActive(false);
    }

    private void Update()
    {
        TickHandler();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mpbController.SetColor(new Color(0f,255f,221f));

        if (GameManager.instance.showPlaceHolderTowers&&!isFull) 
        {
            placeHolder = GameManager.instance.currentPlaceHolder;
            placeHolder.SetActive(true);
            placeHolder.transform.SetParent(transform);
            GameManager.instance.TowerRangeCircle.GetComponent<SpriteRenderer>().enabled = true;
            GameManager.instance.TowerRangeCircle.transform.SetParent(placeHolder.transform);
            placeHolder.transform.position = new Vector3(transform.position.x, transform.position.y+ placeHolder.transform.localScale.y, transform.position.z) ;
           
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mpbController.SetColor(mpbController.tileColor);
        if (GameManager.instance.showPlaceHolderTowers && !isFull)
        {
            GameManager.instance.TowerRangeCircle.GetComponent<SpriteRenderer>().enabled = false;


            placeHolder = GameManager.instance.currentPlaceHolder;
            placeHolder.SetActive(false);
            placeHolder.transform.SetParent(GameManager.instance.transform);//gereksiz olabilir daha sonra silinebilir
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.instance.showPlaceHolderTowers && !isFull)
        {
            if (GameManager.instance.currentTowerType == GameManager.TowerType.archerTower) {
                if (GameManager.instance.goldCount >= GameManager.instance.archerTowerPrice)
                {
                    CreateTower();
                    GameManager.instance.ChangeGoldCount(-GameManager.instance.archerTowerPrice);
                }
                else
                {
                    TowerNoMoney();
                }
            }
            if (GameManager.instance.currentTowerType == GameManager.TowerType.catapultTower)
            {
                if (GameManager.instance.goldCount >= GameManager.instance.CatapultTowerPrice)
                {
                    CreateTower();
                    GameManager.instance.ChangeGoldCount(-GameManager.instance.CatapultTowerPrice);


                }
                else
                {
                    TowerNoMoney();
                }
            }

        } else if (isFull)
        {
        isTick = true;
            GameManager.instance.UpgradeTimerParent.gameObject.SetActive(true);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isFull)
        {
            isTick = false;
            GameManager.instance.UpgradeTimerParent.gameObject.SetActive(false);
        }
    }
    private void TowerNoMoney()
    {
        placeHolder = GameManager.instance.currentPlaceHolder;
        Sequence towerNoMoneySeq = DOTween.Sequence();
        towerNoMoneySeq.Append(placeHolder.transform.DOScale(1.30f, 0.2f).SetEase(Ease.OutBack))
            .Join(placeHolder.GetComponent<Renderer>().material.DOColor(Color.red, 0.1f))
            .Join(placeHolder.transform.GetChild(0).GetComponent<Renderer>().material.DOColor(Color.red, 0.1f))
           .Append(placeHolder.transform.DOScale(1f, 0.1f).SetEase(Ease.InBack))
           .Join(placeHolder.GetComponent<Renderer>().material.DOColor(Color.white, 0.1f))
           .Join(placeHolder.transform.GetChild(0).GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));
    }

    private void CreateTower()
    {
        GameManager.instance.TowerRangeCircle.GetComponent<SpriteRenderer>().enabled = false;
        
        placeHolder = GameManager.instance.currentPlaceHolder;
        GameObject tower = Instantiate(placeHolder, placeHolder.transform.position, Quaternion.identity);
        this.tower = tower.GetComponent<ITower>();
        tower.transform.SetParent(transform);
        isFull = true;
        placeHolder.SetActive(false);
        Sequence towerSeq = DOTween.Sequence();
        towerSeq.Append(tower.transform.DOScale(1.30f, 0.2f).SetEase(Ease.OutBack))
           .Append(tower.transform.DOScale(1f, 0.1f).SetEase(Ease.InBack));
        towerSeq.OnComplete(() => placeHolder.SetActive(true));
    }
    void TickHandler() 
    {

        if (isTick)
        {
            Vector3 pos = transform.position;
            pos.y += 3.75f;

            pos = Camera.main.WorldToScreenPoint(pos);
            Vector2 newPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameManager.instance.canvas.GetComponent<RectTransform>(), pos, null, out newPos);
            GameManager.instance.UpgradeTimerParent.rectTransform.anchoredPosition = newPos;
            passingTime += Time.deltaTime;
            UpgradeTimerImage.fillAmount = passingTime / upgradeTickTime;
            if (passingTime >= upgradeTickTime)
            {
                tower.UpgradeTower();
                isTick = false;
            }
        }
        else if(passingTime>0)
        {
            passingTime = 0;
            UpgradeTimerImage.fillAmount = passingTime / upgradeTickTime;
        }
    }
    

    
}
