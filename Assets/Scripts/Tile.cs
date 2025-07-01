using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    
    bool isFull=false;
    public float f, g, h;
    public Tile parent = null;
    GameObject placeHolder;

    MPBController mpbController;
    void Start()
    {
        if (transform.GetChild(0)!=null) 
        {
        mpbController = transform.GetChild(0).GetComponent<MPBController>();
        }
    }

    

    public void OnPointerEnter(PointerEventData eventData)
    {
        mpbController.SetColor(new Color(0f,255f,221f));

        if (GameManager.instance.showPlaceHolderTowers&&!isFull) 
        {
            placeHolder = GameManager.instance.currentPlaceHodler;
            placeHolder.SetActive(true);

            placeHolder.transform.SetParent(transform);
            placeHolder.transform.position = new Vector3(transform.position.x, transform.position.y+ placeHolder.transform.localScale.y, transform.position.z) ;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mpbController.SetColor(mpbController.tileColor);
        if (GameManager.instance.showPlaceHolderTowers && !isFull)
        {
            placeHolder = GameManager.instance.currentPlaceHodler;
            placeHolder.SetActive(false);
            placeHolder.transform.SetParent(GameManager.instance.transform);//gereksiz olabilir daha sonra silinebilir

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.instance.showPlaceHolderTowers&&!isFull)
        {
            placeHolder = GameManager.instance.currentPlaceHodler;
            GameObject tower = Instantiate(placeHolder,placeHolder.transform.position,Quaternion.identity);
            tower.transform.SetParent(transform);
            isFull = true;

        }
    }
}
