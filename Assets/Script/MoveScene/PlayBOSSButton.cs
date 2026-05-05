using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayBOSSButton : MonoBehaviour
{
    public Button Button;

    void Start()
    {
        Button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        SceneManager.LoadScene("PlayBOSS");
    }
}