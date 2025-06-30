using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
public class TileManager : MonoBehaviour
{
    public static TileManager instance { get; private set; }

    [SerializeField]
    GameObject tileParent;

    [SerializeField]
    Tile grassTile, pathTile;

    [SerializeField]
    int width,height,tileScale;
   
    Tile[,] tileMap;

    Tile startTile, endTile;
    List<Tile> path;
    private void Awake()
    {
        if (instance!=null&& instance!=this) 
        {
        Destroy(this);
        }
        instance = this;
    }
    void Start()
    {
        tileMap = new Tile[width, height];

        CreateMap();
        startTile = GetStartTile();
        endTile = GetEndTile();
        path=FindPath();
        StartCoroutine(ShowPathEffect());
    }

    void Update()
    {
        
    }

    void CreateMap()
    {
        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = Instantiate(grassTile ,new Vector3(x+tileScale*x,0f,y + tileScale*y),Quaternion.identity,tileParent.transform);
                tileMap[x, y] = tile;
                tile.gameObject.name = $"{tile.transform.position.x/tileScale},{tile.transform.position.z / tileScale}";
            }
        }
    }

    Tile GetStartTile() 
    {
    return tileMap[UnityEngine.Random.Range(0, width),0];
    
    }
    Tile GetEndTile()
    {
        return tileMap[UnityEngine.Random.Range(0, width),height-1];

    }

    List<Tile> FindPath()
    {
        List<Tile> openList= new List<Tile>();
        HashSet<Tile> closedList = new HashSet<Tile>();
        List<Tile> path = new List<Tile>();

        openList.Add(startTile);
        

        startTile.g = 0;
        startTile.h = Vector3.Distance(startTile.transform.position, endTile.transform.position);
        startTile.f = startTile.g + startTile.h;

        Tile current;



        while (openList.Count != 0)
            {
           current = openList[0];
            foreach (var tile in openList)
            {
                if (tile.f < current.f)
                {
                    current = tile;
                }
            }
            closedList.Add(current);
            openList.Remove(current);
                if (current == endTile)
                {
                    break;
                }
                foreach (Tile neighbour in getNeighbors(current))
                {
                    if (closedList.Contains(neighbour))
                    {
                        continue;
                    }
                    float tempG = current.g + 2;
                    if (!openList.Contains(neighbour) || tempG < neighbour.g)
                    {
                        

                        neighbour.g = tempG;
                        neighbour.h = Vector3.Distance(neighbour.transform.position, endTile.transform.position);
                        neighbour.f = neighbour.g + neighbour.h;
                        neighbour.parent = current;
                    }
                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }

            }
        current = endTile;
        while (current!=null)
        {
            path.Add(current);
            //Destroy(current.transform.GetChild(2).gameObject);
            current = current.parent;
        }
         path.Reverse();
        return path;




    }
    HashSet<Tile> getNeighbors(Tile tile)
    {
        HashSet<Tile> set = new HashSet<Tile>();
        Vector3 pos = tile.transform.position;
        int x = (int)(tile.transform.position.x/ (tileScale*2));
        int y = (int)(tile.transform.position.z / (tileScale * 2));

        if (x+1< tileMap.GetLength(0))
        {
        set.Add(tileMap[x+1,y]);
        }
        if (y + 1 < tileMap.GetLength(1))
        {
            set.Add(tileMap[x, y+1]);
        }
        if (x - 1 >= 0)
        {
            set.Add(tileMap[x - 1, y]);
        }
        if (y - 1 >= 0)
        {
            set.Add(tileMap[x, y - 1]);
        }
        return set;
    }

    IEnumerator ShowPathEffect() 
    {

        yield return new WaitForSeconds(0.1f);

        foreach (var tile in path) 
        {
            GameObject top = tile.transform.GetChild(2).gameObject;
            Sequence seq = DOTween.Sequence();
            seq.Append(top.transform.DOScale(1.15f, 0.1f).SetEase(Ease.OutBack))
               .Join(top.GetComponent<Renderer>().material.DOColor(Color.red, 0.1f))
               .Append(top.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack))
               .OnComplete(() => Destroy(top));

            yield return new WaitForSeconds(0.2f);
        }
    }

}
