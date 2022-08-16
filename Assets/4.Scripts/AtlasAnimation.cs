using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlasAnimation : AtlasRead
{
    [Header("系列动画精灵名字")] public string AnimationName;

    [Header("动画精灵范围")] public int[] Range = new int[2];
    [Header("动画精灵名格式")] public string format;

    /// <summary>
    /// 不循环动画分组（数值为每个分组的最后一个的序号）
    /// </summary>
    [Header("不循环动画分组（数值为每个分组的最后一个的序号）")] public int[] groups;

    private GameObject go;

    private int index;
    private int groupIndex;
    private WaitForSeconds animatorInterval = new WaitForSeconds(0.069f);

    public override void Awake()
    {
        
        base.Awake();
        go = gameObject;
        index = Range[0];
        groupIndex = 0;
        spriteRenderer.sprite = null;
    }

    [ContextMenu("显示下一个图片")]
    // Start is called before the first frame update
    public void ShowNextAnimation()
    {
        if (index == Range[1] - Range[0] + 2) index = Range[0];

        spriteName = string.Format(format, AnimationName, index.ToString());
        GetSpriteFromAtlas();
        index++;
    }

    [ContextMenu("显示下一组动画（不循环）")]
    public void ShowNextGroupAnimation()
    {
        //先停止所有可能有的换图片的协程
        StopAllCoroutines();

        if (groupIndex >= groups.Length) groupIndex = 0;
        //获取本组第一个图片的序号
        if (groupIndex != 0)
        {
            index = groups[groupIndex - 1] + 1;
        }
        else
        {
            index = Range[0];
        }

        groupIndex++;
        //防止物体被禁用后仍然要调用协程
      if(go.activeInHierarchy)  StartCoroutine(ShowGroup());
    }


    /// <summary>
    /// 显示一组动画
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowGroup()
    {
        while (true)
        {
            ShowNextAnimation();
            //groupIndex - 1:前文已经加了一个了，这边为了得到这一组所以减去
            //这行用于动画终止（分组进行嘛）
            if (index == groups[groupIndex - 1] + 1) break;
            yield return animatorInterval;
        }
    }
}