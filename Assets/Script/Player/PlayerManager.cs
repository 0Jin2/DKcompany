using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // 씬 로드 이벤트 핸들러 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LobbyScene")
        {
            gameObject.SetActive(false); // "LobbyScene"에서 PlayerManager 오브젝트 비활성화
        }
        else if (scene.name == "PlayB1")
        {
            gameObject.SetActive(true); // "PlayB1"에서는 다시 활성화
        }
    }

    void OnDestroy()
    {
        // 씬 로드 이벤트 핸들러 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
} 