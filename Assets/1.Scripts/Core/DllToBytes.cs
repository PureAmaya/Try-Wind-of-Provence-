using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DllToBytes
{
   /// <summary>
   /// dll转为二进制文件。外部读取dll（需要自行检查路径是否存在）
   /// </summary>
   /// <param name="fullPath">dll路径</param>
   /// <param name="savePath">二进制文件保存路径</param>
    public static void DLLToBytesExternal(string fullPath,string savePath)
    {
        //获取文件信息
        FileInfo fileInfo = new FileInfo(fullPath);
      
        
        BytesToFile(savePath, FileToBytes(fileInfo));

    }
   
#region editor编辑器用
#if UNITY_EDITOR
    /// <summary>
    /// dll转为二进制（从菜单中，editor only）
    /// </summary>
    [MenuItem("Tools/ILRuntime/DLL to byte")]
    public static void DllTobytesInMenu()
    {
        DLLToBytes(Selection.assetGUIDs[0]);
    }

   
    
   /// <summary>
   /// dll转为二进制文件，保存在原dll/pdb文件旁边（用于游戏内已经编译好了的dll，editor only）
   /// </summary>
   /// <param name="GUID"></param>
    public static void DLLToBytes(string GUID)
    {
        //得到所选文件的绝对路径
        var path = AssetDatabase.GUIDToAssetPath(GUID);
        //得到所选文件的拓展名与剩余的部分
        string[] ex = path.Split('.');
        //dll pdb才用转换成二进制文件
        if (ex[^1] != "dll" & ex[^1] != "pdb")
        {
            return;
        }
        //二进制保存路径（就是path然后把拓展名给去掉了，之后加上拓展名） 
        var changedPath = string.Empty;
     for (int i = 0; i < ex.Length - 1 ; i++)
        {
         
            if (i== 0)
            {
                changedPath = ex[0];
            }
            //距离拓展名最近的那段，这个把拓展名给省略了，但是有拓展名那个点
            else if(i <= ex.Length - 2)
            {
                changedPath = string.Format("{0}.{1}", changedPath, ex[i]);
            }
          
        }
      Debug.Log(changedPath);

        //获取文件信息
        FileInfo fileInfo = new FileInfo(path);
        FileInfo listDLL = null;
        FileInfo listPDB = null;
        
        //保存对应文件的文件信息
        if (ex[^1] == "dll")
        {
            listDLL = fileInfo;
            BytesToFile(string.Format("{0}dll",changedPath) , FileToBytes(listDLL));
        }
        else
        {
            listPDB = fileInfo;
            BytesToFile(string.Format("{0}pdb",changedPath) , FileToBytes(listPDB));
        }



    
    }
   
   #endif
    #endregion

    /// <summary>
    /// 文件转为二进制
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
    private static byte[] FileToBytes(FileInfo fileInfo)
    {
        return File.ReadAllBytes(fileInfo.FullName);
    }
    
    /// <summary>
    /// 把二进制保存为文件
    /// </summary>
    /// <param name="path">保存路径</param>
    /// <param name="bytes"></param>
    private static void BytesToFile(string path, byte[] bytes)
    {
        File.WriteAllBytes(path, bytes);
    }
    
    
    
    
}
