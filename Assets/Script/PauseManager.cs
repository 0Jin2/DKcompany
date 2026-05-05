using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("일시정지 옵션 팝업 프리팹 설정")]
    public GameObject lobbyPausePrefab;    // LobbyScene에서 사용할 일시정지 프리팹
    public GameObject tutorialPausePrefab; // Tutorial 씬에서 사용할 일시정지 프리팹
    public GameObject defaultPausePrefab;  // 그 외의 씬에서 사용할 일시정지 프리팹

    private GameObject currentPauseMenu;                                                     // 현재 활성화된 일시정지 프리팹
    private bool isPaused = false;                                                           // 일시정지 상태 추적
    private string[] excludedScenes = { "LoadingScene1", "LoadingScene2", "LoadingScene3" }; // ESC 비활성화 씬 목록
    private TimeAttack timeAttack;                                                           // TimeAttack 참조

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SetupPauseMenu(SceneManager.GetActiveScene().name);
        timeAttack = FindObjectOfType<TimeAttack>();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupPauseMenu(scene.name);
        timeAttack = FindObjectOfType<TimeAttack>();
    }

    void Update()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (Input.GetKeyDown(KeyCode.Escape) && !IsExcludedScene(currentSceneName))
        {
            if (IsPopupActive())
            {
                // Popup이 활성화되어 있을 때 프리팹이 현재 활성화된 경우만 ESC로 비활성화
                if (currentPauseMenu != null && currentPauseMenu.activeInHierarchy)
                {
                    ResumeGame();
                }
            }
            else
            {
                // Popup이 비활성화 상태일 경우 일반적으로 ESC 키로 일시정지 전환
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }

    void PauseGame()
    {
        if (currentPauseMenu != null)
        {
            currentPauseMenu.SetActive(true); // 일시정지 프리팹 활성화
        }
        Time.timeScale = 0f; // 게임 일시정지
        if (timeAttack != null) timeAttack.PauseTimer(); // 타이머 일시정지
        isPaused = true;
    }

    void ResumeGame()
    {
        if (currentPauseMenu != null)
        {
            currentPauseMenu.SetActive(false); // 일시정지 프리팹 비활성화
        }
        Time.timeScale = 1f; // 게임 재개
        if (timeAttack != null) timeAttack.ResumeTimer(); // 타이머 재개
        isPaused = false;
    }

    void SetupPauseMenu(string sceneName)
    {
        if (currentPauseMenu != null)
        {
            Destroy(currentPauseMenu);
        }

        if (sceneName == "LobbyScene")
        {
            currentPauseMenu = Instantiate(lobbyPausePrefab, FindObjectOfType<Canvas>().transform);
        }
        else if (sceneName == "Tutorial")
        {
            currentPauseMenu = Instantiate(tutorialPausePrefab, FindObjectOfType<Canvas>().transform);
        }
        else
        {
            currentPauseMenu = Instantiate(defaultPausePrefab, FindObjectOfType<Canvas>().transform);
        }

        if (currentPauseMenu != null)
        {
            currentPauseMenu.SetActive(false);
        }
    }

    bool IsPopupActive()
    {
        GameObject popupObject = GameObject.FindGameObjectWithTag("Popup");
        return popupObject != null && popupObject.activeInHierarchy;
    }

    bool IsExcludedScene(string sceneName)
    {
        foreach (string excludedScene in excludedScenes)
        {
            if (sceneName == excludedScene)
            {
                return true;
            }
        }
        return false;
    }
}