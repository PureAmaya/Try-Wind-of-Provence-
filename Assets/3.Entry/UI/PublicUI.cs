using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class PublicUI : MonoBehaviour
{
    public static PublicUI publicUI;





    private void Awake()
    {
        publicUI = this;

        DontDestroyOnLoad(gameObject);
    }
}
