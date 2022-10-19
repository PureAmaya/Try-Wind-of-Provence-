using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ended : MonoBehaviour
{

    /// <summary>
    /// 全国大赛得奖用的bgm
    /// </summary>
    /// <returns></returns>
    public AudioClip Music;
    
    /// <summary>
    /// 展示成果
    /// </summary>
    public TMP_Text achievement;

    private const string Gold = "#e2d87c";
    private const string Sliver = "#e1ddc5";
    private const string Bronze = "#a75b10";
    
    // Start is called before the first frame update
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

  

    /// <summary>
    /// 结果结算
    /// </summary>
    public void Settlement()
    {
        //根据TextUI进行评价
        switch (TextUI.textUI.achievement.RankLevel)
      {
          //府赛没有得到名次
            case 0:
                achievement.text = $"本次演奏评价为{TextUI.textUI.achievement.score}\n很遗憾，京都府赛没有取得名次";
                break;
            
          //府赛金奖（废金）
            case 1:
                achievement.text = $"本次演奏评价为{TextUI.textUI.achievement.score}\n祝贺，京都府赛取得<color={Gold}><size=150%>金奖</size></color>！\n但是很可惜，无缘关西赛";
                break;
            
          //府赛金奖（晋级）
            case 2:
                achievement.text = $"本次演奏评价为{TextUI.textUI.achievement.score}\n祝贺，京都府赛取得<color={Gold}><size=150%>金奖</size></color>，并顺利进军关西赛！\n但是很可惜，关西赛尚未获奖";
                break;
            
          //关西废金
            case 3:
                achievement.text = $"本次演奏评价为{TextUI.textUI.achievement.score}\n祝贺，关西赛取得<color={Gold}><size=150%>金奖</size></color>！\n但是很可惜，无缘全国大赛";
                break;
         
          //关西金奖（晋级）
            case 4:
                achievement.text = $"本次演奏评价为{TextUI.textUI.achievement.score}\n祝贺，关西赛取得<color={Gold}><size=150%>金奖</size></color>，并顺利进军全国大赛！\n但是很可惜，全国大赛尚未获奖";
                break;
            
          //全国赛 铜奖
            case 5:
                achievement.text = $"本次演奏评价为{TextUI.textUI.achievement.score}\n祝贺，全国大赛取得<color={Bronze}><size=150%>铜奖</size></color>！";
                PublicAudioSource.publicAudioSource.PlayBackgroundMusic(Music);
                break;
            
          //全国赛 银奖
          case 6:
              achievement.text = $"本次演奏评价为{TextUI.textUI.achievement.score}\n祝贺，全国大赛取得<color={Sliver}><size=150%>银奖</size></color>！";
              PublicAudioSource.publicAudioSource.PlayBackgroundMusic(Music);
              break;
          
          //全国赛 金奖
          case 7:
              achievement.text = $"本次演奏评价为{TextUI.textUI.achievement.score}\n祝贺，全国大赛取得<color={Gold}><size=150%>金奖</size></color>！";
              PublicAudioSource.publicAudioSource.PlayBackgroundMusic(Music);
              break;
              
        } 
        
        //然后存个档
        YamlReadWrite.Write(TextUI.textUI.achievement,YamlReadWrite.FileName.Achievement,"#上一次演奏的成绩");
    }
    
}
