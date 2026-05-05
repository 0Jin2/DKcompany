using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OutTutorialButton : MonoBehaviour
{
    public Button yourButton;
    public float fadeDuration = 1f;  // 투명도 증가 시간
    private GameObject uiPanel;      // TutorialPage 태그로 찾을 UI 패널
    private Image panelImage;

    void Start()
    {
        // TutorialPage 태그로 UI 패널 찾기
        uiPanel = GameObject.FindWithTag("TutorialPage");

        if (uiPanel != null)
        {
            uiPanel.SetActive(false); // 초기 비활성화
            panelImage = uiPanel.GetComponent<Image>();

            if (panelImage != null)
            {
                Color color = panelImage.color;
                color.a = 0f;         // 초기 투명도 0
                panelImage.color = color;
            }
        }

        if (yourButton != null)
        {
            yourButton.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        if (uiPanel != null && panelImage != null)
        {
            uiPanel.SetActive(true);      // 패널 활성화
            StartCoroutine(FadeInAndLoadScene("LobbyScene")); // 페이드 인 및 씬 이동
        }
    }

    private IEnumerator FadeInAndLoadScene(string sceneName)
    {
        float elapsed = 0f;
        Color color = panelImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeDuration); // 0에서 1까지 알파 증가
            panelImage.color = color;
            yield return null;
        }

        // 페이드 인 완료 후 씬 이동
        SceneManager.LoadScene(sceneName);
    }
}