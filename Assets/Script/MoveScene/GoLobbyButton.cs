using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GoLobbyButton : MonoBehaviour
{
    public Button yourButton;

    void Start()
    {
        yourButton.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        SceneManager.LoadScene("LoadingScene2");
    }
}