using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenURLInBrowser : MonoBehaviour
{
    public string pathGame = "";
    public string pathGameSubmit = "";
    public void OpenGame()
    {
        Application.OpenURL(pathGame);
    }
    public void OpenGameSubmit()
    {
        Application.OpenURL(pathGameSubmit);
    }
}
