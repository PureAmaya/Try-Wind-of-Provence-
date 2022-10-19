
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningCtrl : MonoBehaviour
{
    public Slider MusicVolSlider;
    public Slider EffectVolSlider;

    public AudioClip bgm;
   
    public UnityEvent initialization = new();

    public GameObject welcome;
    public GameObject tutorial;
    public GameObject staff;
    
    
    public virtual void Awake()
    {
        Time.timeScale = 1f;
        
        Application.targetFrameRate = -1;
        
        initialization.Invoke();
        
        welcome.SetActive(true); 
        tutorial.SetActive(false);
        staff.SetActive(false);
    }

    // Start is called before the first frame update
    public virtual void  Start()
    {
        //按照文件调整滑块
        MusicVolSlider.value = Settings.SettingsContent.MusicVolume;
        EffectVolSlider.value = Settings.SettingsContent.SoundEffectVolume;
        
        
        //注册事件，滑条更新音量
        MusicVolSlider.onValueChanged.AddListener(delegate(float arg0)
        {
            Settings.SettingsContent.MusicVolume = arg0; 
            if(PublicAudioSource.publicAudioSource != null)  PublicAudioSource.publicAudioSource.UpdateMusicVolume();
        });
        EffectVolSlider.onValueChanged.AddListener(delegate(float arg0)
        {
            Settings.SettingsContent.SoundEffectVolume = arg0;
        });

        
        //播放BGM
        if(PublicAudioSource.publicAudioSource != null)   PublicAudioSource.publicAudioSource.PlayBackgroundMusic(bgm);
    }





 
}
