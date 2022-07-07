using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SongsInf : MonoBehaviour
{
    //缓存数据，以后能用
    public TMP_Text StagesName;
    public TMP_Text Author;
    public TMP_Text ShortInstr;
    public Image Icon;
   [HideInInspector]public string Instruction;
    
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
        Instruction = manifest.Instruction;

    }

    /// <summary>
    /// 右侧小框点击后，把左侧详细信息更新为所选的铺面
    /// </summary>
    public void OnClick()
    {
        MenuCtrl.menuCtrl.OnSelected(this);
    }
}
