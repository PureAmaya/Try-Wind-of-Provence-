using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;


public class Opening : MonoBehaviour
{

#if UNITY_EDITOR

    Core.Manifest text = new Core.Manifest
    {
        Name = "text",
        MusicName = "听力",
        Author = "wdnmd",
        Origin = "出题的",
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

        Debug.Log(data.Version);
        Debug.Log(data.MusicName);
    }

    public void PlayPreBGM()
    {

    }

#endif
}
