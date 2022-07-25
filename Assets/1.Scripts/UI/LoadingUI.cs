using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    public Image loadText;
    public Image SOS;

    private Color color = new Color(1f, 1f, 1f, 1f);

    private float i = 0.5f;

    private bool s = false;

    private void Update()
    {
        //每两帧调整一下透明度
        if (s)
        {
            Twink();
        }
        else
        {
            s = true;
        }
    }

    private void Twink()
    {
        if (SOS.color.a >= 1f || SOS.color.a <= 0f)
        {
            i = -i;
        }

        color.a = color.a + (i * Time.deltaTime);

        SOS.color = color;
    }
}