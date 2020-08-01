using UnityEngine;
using UnityEngine.SceneManagement;

//------------------------------------------------------\\
//  The sole purpose of this script is to allow an      \\
//  animation event to call the CreditsDone function    \\
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

public class CreditsText : MonoBehaviour
{
    //Called by an animation event, loads the main menu scene
    //  once the credits have finished scrolling
    public void CreditsDone()
    {
        SceneManager.LoadScene("mainMenu");
    }
}