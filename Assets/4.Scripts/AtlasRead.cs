using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;


public class AtlasRead : MonoBehaviour
{
    private Image imageRender;
    [HideInInspector]public SpriteRenderer spriteRenderer;
    public SpriteAtlas spriteAtlas;
    public string spriteName;

    public bool destroyWhenGetSprite = true;
    
    [ContextMenu("获取图片")]
    public virtual void Awake()
    {
        imageRender = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GetSpriteFromAtlas(); 
       if(Application.isPlaying && destroyWhenGetSprite) Destroy(this);
    }

    public void GetSpriteFromAtlas()
    {
        if (imageRender != null)
        {
            imageRender.sprite = spriteAtlas.GetSprite(spriteName);
          
        }
        
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = spriteAtlas.GetSprite(spriteName);
        }
    }


}
