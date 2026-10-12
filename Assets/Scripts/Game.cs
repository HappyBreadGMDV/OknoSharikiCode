using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    public string Name;
    public string Author;
    public string Url;

    public TextMeshProUGUI NameTmp;
    public TextMeshProUGUI AuthorTmp;

    void Start()
    {
        if (Name != null)
        {
            NameTmp.text = Name;
        }
        if (Author != null)
        {
            AuthorTmp.text = $"By:{Author}";
        }
    }
}
