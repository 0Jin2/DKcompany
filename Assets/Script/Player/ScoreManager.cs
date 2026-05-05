using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("SCORE 텍스트 프리팹")]
    public TextMeshProUGUI scoreTextPrefab;
    private TextMeshProUGUI scoreText;

    [Header("추가 되는 점수")]
    public int scoreIncreaseAmount = 50;

    [Header("감소 되는 점수")]
    public int scoreDecreaseAmount = 50;
    private int score;

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
        if (scoreTextPrefab != null)
        {
            scoreTextPrefab.gameObject.SetActive(false); // 처음 시작할 때 비활성화
        }

        SetupCanvasAndUI();
        UpdateScoreText();
    }

    void SetupCanvasAndUI()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            if (scoreText == null && scoreTextPrefab != null)
            {
                scoreText = Instantiate(scoreTextPrefab, canvas.transform);
            }
        }
    }

    public void IncreaseScore(int amount)
    {
        if (isIncreaseInProgress) return;

        StartCoroutine(IncreaseScoreCoroutine(amount));
    }

    private IEnumerator IncreaseScoreCoroutine(int amount)
    {
        isIncreaseInProgress = true;

        score += amount;
        UpdateScoreText();

        isIncreaseInProgress = false;
        yield break;
    }

    public void DecreaseScore(int amount)
    {
        score -= amount;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
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
        if (sceneName == "LobbyScene")
        {
            ResetScore(); // 점수 초기화
            if (scoreText != null)
            {
                scoreText.gameObject.SetActive(false); // 로비 씬에서 점수 텍스트 비활성화
            }
        }
        else if (sceneName == "PlayB1" || sceneName == "PlayB2" || sceneName == "PlayBOSS" || sceneName == "PlayB1_HARD" || sceneName == "PlayB2_HARD" || sceneName == "PlayBOSS_HARD" || sceneName == "Tutorial")
        {
            if (scoreText != null)
            {
                scoreText.gameObject.SetActive(true); // 점수 텍스트 활성화
            }
            else
            {
                SetupCanvasAndUI();
                if (scoreText != null)
                {
                    scoreText.gameObject.SetActive(true); // 생성 후 활성화
                }
            }
            UpdateScoreText(); // 점수 갱신
        }
        else if (sceneName == "LoadingScene1" || sceneName == "LoadingScene2" || sceneName == "LoadingScene3")
        {
            // Loading 씬에서는 점수 초기화하지 않고 비활성화만 적용
            if (scoreText != null)
            {
                scoreText.gameObject.SetActive(false); // 점수 텍스트 비활성화
            }
        }
        else
        {
            SetupCanvasAndUI(); // 다른 씬에서도 텍스트를 새로 생성
            if (scoreText != null)
            {
                scoreText.gameObject.SetActive(true); // 다른 씬에서도 활성화
            }
            UpdateScoreText();
        }
    }

    public void ResetScore()
    {
        score = 0;  // 점수 초기화
        UpdateScoreText();  // UI 갱신
    }

    public int GetCurrentScore()
    {
        return score;  // 현재 점수 반환
    }
}
