using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class countdown : MonoBehaviour
{
    private int countdownNumber = 3;

    public Text text;
    public GameObject countdownParent;
    
    public void Initialization()
    {
        countdownNumber = 5;
    }
    
    
    public void UpdateNumber()
    {
        countdownNumber--;

        switch (countdownNumber)
        {
            case > 1:
                text.text = (countdownNumber - 1).ToString();
                break;
            
            case 1:
                text.text = "继续！";
                break;
            
            default:
               
                //解除暂停
                countdownParent.SetActive(false);
                basicEvents.Pause();
              
                break;
        }
    }
    
}
