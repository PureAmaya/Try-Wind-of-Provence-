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
    private bool isimageRenderNotNull;
    private bool isspriteRendererNotNull;

    private void Start()
    {
        if(Application.isPlaying && destroyWhenGetSprite) Destroy(this);
    }

    [ContextMenu("获取图片")]
    public virtual void Awake()
    {
       
        imageRender = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        isimageRenderNotNull = imageRender != null;
        isspriteRendererNotNull = spriteRenderer != null;
        GetSpriteFromAtlas(); 
        
       
    }

    /// <summary>
    /// 从图集中得到图片
    /// </summary>
    public void GetSpriteFromAtlas()
    {
        if (isimageRenderNotNull)
        {
            imageRender.sprite = spriteAtlas.GetSprite(spriteName);
        }

        if (isspriteRendererNotNull)
        {
            spriteRenderer.sprite = spriteAtlas.GetSprite(spriteName);
        }
    }


}
