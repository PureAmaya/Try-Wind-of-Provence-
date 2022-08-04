using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Notes : MonoBehaviour
{
    /// <summary>
    /// 这个判定方块，判定玩家按下了哪个键
    /// </summary>
    private KeyCode keyToApply = KeyCode.LeftControl;
    
    /// <summary>
    /// 音符的初始位置（视野之外）
    /// </summary>
   private static readonly  Vector3 InitialNotePoint = new Vector3(-6f, -100f, 0f);
  
   
    public SpriteRenderer[] spriteRenderers;
    
    private Transform tr;

    /// <summary>
    /// 是否被判定块摸着
    /// </summary>
    private bool SquareIsCovering = false;
    
    /// <summary>
    /// 外部获取Transform
    /// </summary>
    /// <returns></returns>
    public Transform GetTransform()
    {
        if (tr == null) tr = transform;
        return tr;
    }

    /// <summary>
    /// 音符回到初始化位置
    /// </summary>
    public void BackToInitialNotePoint()
    {
        tr.position = InitialNotePoint;
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = Color.white; 
        }
    }

    /// <summary>
    /// 初始化音符  
    /// </summary>
    public void Initialization(int character)
    {
        GetTransform();
       BackToInitialNotePoint();

        //按照角色设定按键
        keyToApply = character == 0 ? KeyCode.LeftControl : KeyCode.RightControl;

    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        SquareIsCovering = true;
    }

    /// <summary>
    /// 判定方块与音符的层都是Note，只有他俩之间能碰撞。这个就是碰撞判定用的。判定负责变色
    /// </summary>
    /// <param name="other"></param>
    public void Update()
    {
    
        //如果此时与音符碰上了，并且玩家按下了按键
        if (!Input.GetKeyDown(keyToApply) || !SquareIsCovering) return;
        //变成绿色
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = Color.green; 
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        //如果判定方块都走了，还是白色，说明没按上按键，变红
        SquareIsCovering = false;
        if (spriteRenderers[0].color != Color.white) return;
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = Color.red; 
        }
    }
}
