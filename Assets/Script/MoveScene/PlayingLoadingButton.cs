using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayingLoadingButton : MonoBehaviour
{
    public Button yourButton;

    void Start()
    {
        yourButton.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        SceneManager.LoadScene("LoadingScene1");
    }
}