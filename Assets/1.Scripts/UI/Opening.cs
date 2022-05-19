using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏启动页的UI脚本
/// </summary>
public class Opening : MonoBehaviour
{
    private void Awake()
    {
        
    }


#if UNITY_EDITOR

    Core.Manifest text = new Core.Manifest
    {
        Name = "text",
        MusicName = "听力",
        Author = "wdnmd",
        Origin = "出题的",
        IsAdvanced = false,
        Version = "玩玩试试的版本",
        PreviewBGM = "Pre.mp3",
    };

    /// <summary>
    /// 写一个实例数据。学习用
    /// </summary>
    [ContextMenu("Write")]
    public void Write()
    {
        Core.YamlWrite(Core.SubdirectoryTypes.SpellCards, text.Name,"Manifest", text);

    }

    [ContextMenu("Read")]
    public void Read()
    {
    var data = Core.YamlRead<Core.Manifest>(Core.SubdirectoryTypes.SpellCards, text.Name,"Manifest");
    }

    public void PlayPreBGM()
    {

    }

#endif
}
