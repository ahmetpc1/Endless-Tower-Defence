using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPBController : MonoBehaviour
{
    private Renderer tileRenderer;
    private MaterialPropertyBlock materialPropertyBlock;
    public readonly Color tileColor = Color.white;

    void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    public void SetColor(Color color) 
    {
        materialPropertyBlock.SetColor("_Color", color);
        tileRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}
