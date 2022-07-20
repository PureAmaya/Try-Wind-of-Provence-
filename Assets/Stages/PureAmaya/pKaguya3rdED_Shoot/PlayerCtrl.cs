using System;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public Rigidbody2D rb;

    public SpriteRenderer sr;
    public  Color colorForText = Color.red;
    
    
    
    private float moveX = 0f;
    private float moveY = 0f;

    private void Awake()
    {
        sr.color = colorForText;
    }

    // Start is called before the first frame update
    public void Update()
    {
        //根据键盘移动
        moveX = Input.GetAxis("Horizontal") * 7f;
        moveY = Input.GetAxis("Vertical") * 7f;

        rb.velocity = new Vector2(moveX, moveY);
    }
}