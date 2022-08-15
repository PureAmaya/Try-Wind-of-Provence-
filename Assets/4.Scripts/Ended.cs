using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ended : MonoBehaviour
{

    public AudioClip ClickEffect;
    
    // Start is called before the first frame update
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ReturnToTitle()
    {
        PublicAudioSource.PlaySoundEffect(ClickEffect);
        SceneManager.LoadScene("Opening");
    }

    public void PlayAgain()
    {
        PublicAudioSource.PlaySoundEffect(ClickEffect);
        SceneManager.LoadScene("LOADING");
    }
    
    
}
