using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{


    // Start is called before the first frame update
    private IEnumerator Start()
    {
        Settings.SaveSettings();
        yield return Resources.UnloadUnusedAssets();
        yield return SceneManager.LoadSceneAsync("Wind of Provence - Copy");
    }



}
