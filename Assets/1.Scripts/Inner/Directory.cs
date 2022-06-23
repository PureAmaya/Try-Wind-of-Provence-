using UnityEngine;
using System.IO;

/// <summary>
/// 默认设定的文件夹类型
/// </summary>
public static class DefaultDirectory
{
    public  enum SubdirectoryTypes
    {
        /// <summary>
        /// 关卡储存
        /// </summary>
        Stages,
        /// <summary>
        /// 游戏皮肤
        /// </summary>
        Skins,
        /// <summary>
        /// 游戏设置
        /// </summary>
        Settings,
        /// <summary>
        /// 玩家存档
        /// </summary>
        gamesaves,
        Log,
    }
    
    /// <summary>
    /// Assets上一级的目录（绝对路径）
    /// </summary>
    /// <returns></returns>
    public static string UnityButNotAssets
    {
        get
        {
            string[] raw = Application.dataPath.Split("/");

            string done = string.Empty;
            for (int i = 1; i < raw.Length - 1; i++)
            {
                //得到从unity开始的路径
                done = string.Format("{0}/{1}", done, raw[i]);
            }

            DirectoryInfo directoryInfo = new(done);
            return directoryInfo.FullName;
        }
    }

}
