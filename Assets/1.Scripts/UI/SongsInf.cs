using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SongsInf : MonoBehaviour
{
    //缓存数据，以后能用
    public TMP_Text MusicName;
    public TMP_Text Author;
    public TMP_Text Origin;
    public Image Icon;
    [HideInInspector] public bool IsAdvanced;
    [HideInInspector] public  AudioClip PreBGM;
    [HideInInspector] public bool[] difficulty;

    public void ApplyInf(string musicName,string author,string origin,string version,Sprite icon,bool isAdvanced,AudioClip preBGM,bool[] allowedDifficulty)
    {
        MusicName.text = musicName;
        Author.text = string.Format("{0} - {1}",version, author);
        Origin.text = origin;
        Icon.sprite = icon;
        IsAdvanced = isAdvanced;
        PreBGM = preBGM;
        difficulty = allowedDifficulty;

    }

    /// <summary>
    /// 点击后，把左侧详细信息更新为所选的铺面
    /// </summary>
    public void OnClick()
    {
        MenuCtrl.menuCtrl.OnSelected(this);
    }
}
