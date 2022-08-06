using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;


public class AtlasRead : MonoBehaviour
{
    public Image imageRender;
    public SpriteRenderer spriteRenderer;
    public SpriteAtlas spriteAtlas;
    public string spriteName;

    [ContextMenu("获取图片")]
    public void Awake()
    {
        if (imageRender != null)
        {
            imageRender.sprite = spriteAtlas.GetSprite(spriteName);
        }
        
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = spriteAtlas.GetSprite(spriteName);
        }
        
        Destroy(this);
    }


}
