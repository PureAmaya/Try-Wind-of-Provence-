using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SongsInf : MonoBehaviour
{
    //UI中的显示
    [SerializeField]private TMP_Text StagesName;
    [SerializeField]private  TMP_Text Author;
    [SerializeField]private  TMP_Text ShortInstr;
    [SerializeField]private Image Icon;
  
/// <summary>
/// 这个信息卡片使用的manifest的所有信息都在这里面了
/// </summary>
   public YamlAndFormat.Manifest UsedManifestInf;
   
    /// <summary>
    /// 读取manifest信息之后，应用之，并在右侧小框内显示
    /// </summary>
    /// <param name="manifest"></param>
    public void ApplyInf(YamlAndFormat.Manifest manifest,Sprite icon)
    {
        StagesName.text =manifest.StageName;
        Author.text = string.Format("{0} - {1}",manifest.Version,manifest.Author);
        ShortInstr.text = manifest.ShortInstr;
        Icon.sprite = icon;
        UsedManifestInf = manifest;
    }

    /// <summary>
    /// 得到信息卡片的缩略图
    /// </summary>
    /// <returns></returns>
    public Sprite GetImage()
    {
        return Icon.sprite;
    }

    /// <summary>
    /// 右侧小框点击后，把左侧详细信息更新为所选的铺面
    /// </summary>
    public void OnClick()
    {
        MenuCtrl.menuCtrl.OnSelected(this);
    }
}
