using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitGame : MonoBehaviour
{
    public Button quitButton;

    void Start()
    {
        quitButton.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        Application.Quit();
    }
}