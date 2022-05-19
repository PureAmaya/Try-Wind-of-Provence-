using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 从本地读取的文件，都在这里进行缓存。
/// </summary>

//没啥用
public class ResourceKeeper//统统不静态了，随用随声明吧
{
    /// <summary>
    /// “菜单”界面缓存
    /// </summary>
    public class MenuTemp
    {
        /// <summary>
        /// 菜单中的清单缓存
        /// </summary>
        public class Manifest
        {
            /// <summary>
            /// 音乐名称，在游戏中展示。
            /// </summary>
            public  string MusicName = "默认符卡";
            /// <summary>
            /// 铺面图标
            /// </summary>
            public Sprite Icon;
            /// <summary>
            /// 铺面作者名称。
            /// </summary>
            public string Author = "RSC玩家";

            /// <summary>
            /// BGM出处/所属专辑。
            /// </summary>
            public string Origin = "默认出处";
            /// <summary>
            /// 预览音乐
            /// </summary>
            public AudioClip PreviewBGM;
            /// <summary>
            /// 是否为高级符卡。
            /// </summary>
            public bool IsAdvanced = false;
            /// <summary>
            /// 该文件的版本。（用于对自己做的符卡进行版本控制）
            /// </summary>
            public string Version = "1.0.0";
        }

        /// <summary>
        /// 菜单皮肤缓存
        /// </summary>
        public class Skin
        {

        }
    }
}
