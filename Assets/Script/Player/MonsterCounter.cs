using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MonsterCounter : MonoBehaviour
{
    public static MonsterCounter instance; 
    public TextMeshProUGUI monsterCountTextPrefab;
    private TextMeshProUGUI monsterCountText;
    private int monsterCount;

    public GameObject endGameUIPrefabB1; 
    private GameObject endGameUIB1Instance; 
    public GameObject endGameUIPrefabB2; 
    private GameObject endGameUIB2Instance; 

    private int stageGoal;

    private bool isIncreaseInProgress = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 초기 설정
        SetupCanvasAndUI();
        UpdateStageGoal();
        UpdateMonsterCountText();
    }

    void SetupCanvasAndUI()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            // Text UI 설정
            if (monsterCountText == null && monsterCountTextPrefab != null)
            {
                monsterCountText = Instantiate(monsterCountTextPrefab, canvas.transform);
            }

            // endGameUIPrefabB1 설정
            if (endGameUIB1Instance == null && endGameUIPrefabB1 != null)
            {
                endGameUIB1Instance = Instantiate(endGameUIPrefabB1, canvas.transform);
                endGameUIB1Instance.SetActive(false); // 초기 비활성화
            }

            // endGameUIPrefabB2 설정
            if (endGameUIB2Instance == null && endGameUIPrefabB2 != null)
            {
                endGameUIB2Instance = Instantiate(endGameUIPrefabB2, canvas.transform);
                endGameUIB2Instance.SetActive(false); // 초기 비활성화
            }
        }
    }

    void UpdateStageGoal()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // 씬 이름에 따라 stageGoal을 설정합니다.
        if (sceneName == "PlayB1")
        {
            stageGoal = 10;
        }
        else if (sceneName == "PlayB2")
        {
            stageGoal = 35;
        }
        else
        {
            stageGoal = 0;
        }
    }

    public void IncreaseMonsterCount()
    {
        // 이미 몬스터 증가 작업 중이라면 더 이상 진행하지 않음
        if (isIncreaseInProgress)
        {
            return;
        }

        StartCoroutine(IncreaseMonsterCountCoroutine());
    }

    private IEnumerator IncreaseMonsterCountCoroutine()
    {
        isIncreaseInProgress = true;

        // 몬스터 수 증가
        monsterCount++;
        UpdateMonsterCountText();

        // 목표 달성 여부 확인
        CheckGoal();

        // 다음 증가 작업까지 대기 시간 설정 (예: 0.1초)
        yield return new WaitForSeconds(0.1f);

        isIncreaseInProgress = false;
    }

    void UpdateMonsterCountText()
    {
        if (monsterCountText != null)
        {
            monsterCountText.text = "SCORE : " + monsterCount.ToString();
        }
    }

    void CheckGoal()
    {
        if (monsterCount >= stageGoal && stageGoal > 0)
        {
            ActivateWinPopup();
        }
    }

    void ActivateWinPopup()
    {
        // 게임 일시정지
        Time.timeScale = 0;

        string sceneName = SceneManager.GetActiveScene().name;

        // 씬 이름에 따라 다른 UI 프리팹을 활성화
        if (sceneName == "PlayB1" && endGameUIB1Instance != null)
        {
            endGameUIB1Instance.SetActive(true);
        }
        else if (sceneName == "PlayB2" && endGameUIB2Instance != null)
        {
            endGameUIB2Instance.SetActive(true);
        }
    }

    public void LoadNextScene(string nextScene)
    {
        // UI 프리팹 비활성화
        if (endGameUIB1Instance != null)
        {
            endGameUIB1Instance.SetActive(false);
        }
        if (endGameUIB2Instance != null)
        {
            endGameUIB2Instance.SetActive(false);
        }

        // 다음 씬의 점수 초기화
        monsterCount = 0;

        Time.timeScale = 1; // 게임 속도 복원
        SceneManager.LoadScene(nextScene); // 다음 씬 로드
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleSceneChange(scene.name);
    }

    void HandleSceneChange(string sceneName)
    {
        if (sceneName == "LobbyScene" || sceneName == "LoadingScene")
        {
            DestroyMonsterCountText(); // "LobbyScene" 및 "LoadingScene"에서 텍스트 오브젝트 파괴
        }
        else
        {
            SetupCanvasAndUI(); // 다른 씬에서 텍스트 오브젝트 생성
            UpdateStageGoal();
            UpdateMonsterCountText();
        }
    }

    void DestroyMonsterCountText()
    {
        if (monsterCountText != null)
        {
            Destroy(monsterCountText.gameObject);
            monsterCountText = null;
        }
    }
}
//MonsterCounter 스크립트를 완전히 수정해서 ScoreManager로 다시 만드려고 하는데, Score를 보여주는 텍스트 매쉬 프로 UI는 씬 전환시에도 파괴되지 않고 "LobbyScene"으로 들어가게 되면 비활성화 되었다가 "PlayB1"씬에 들어가게 되면 Score를 0으로 값을 초기화되게 만들어줘. 그리고 몬스터에게 공격을 해서 충돌이 일어나면 100점씩 올라가게 만들어줘