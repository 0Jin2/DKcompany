using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Play2Button_HARD : MonoBehaviour
{
    public Button Button;

    void Start()
    {
        Button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        SceneManager.LoadScene("PlayB2_HARD");
    }
}