using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏启动页的UI脚本
/// </summary>
public class Opening : MonoBehaviour
{
    private void Awake()
    {
      
    }


#if UNITY_EDITOR

   

    /// <summary>
    /// 写一个实例数据。学习用
    /// </summary>
    [ContextMenu("Write")]
    public void Write()
    {
       

    }

    [ContextMenu("Read")]
    public void Read()
    {
   
    }

    public void PlayPreBGM()
    {

    }

#endif
}
