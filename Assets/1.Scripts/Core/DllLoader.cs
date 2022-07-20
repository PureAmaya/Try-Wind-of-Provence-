using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using System.IO;


public class DllLoader
{
    /// <summary>
    /// 异步加载二进制文件（from Addressable）
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task<byte[]> LoadFromAddressables(string path)
    {
        var text = await Addressables.LoadAssetAsync<TextAsset>(path).Task;

        return text.bytes;
    }

    /// <summary>
    /// 将二进制内容转换成的dll(pdb)，并保存在（外部）Stages/{作者名称}/dll/中
    /// </summary>
    /// <param name="authorName">作者名字</param>
    /// <param name="nameOfFile">文件名字。从manifest中获取，不含拓展名</param>
    ///  <param name="version">版本。一般情况下与manifest一致</param>
    /// <param name="bytes"></param>
    public  IEnumerator BytesToDllOrPdb(string authorName, string nameOfFile, string version,
        byte[] bytes)
    {
        string path = string.Format("{0}/Stages/{1}/dll", DefaultDirectory.UnityButNotAssets, authorName);

        if (Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //创建个文件进行版本控制
        var f = new FileStream(string.Format("{0}/version", path), FileMode.Create);
        StreamWriter sw = new(f, System.Text.Encoding.UTF8);
       yield return sw.WriteAsync(version);
        sw.Close();
        f.Close();

        //写正经的dll文件
        path = string.Format("{0}/{1}.dll",path, nameOfFile);
        File.WriteAllBytes(path, bytes);
    }
}