using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GoTutorialButton : MonoBehaviour
{
    public Button yourButton;
    public GameObject uiPanel;       // 투명도 조절할 UI 패널 오브젝트
    public float fadeDuration = 1f;  // 투명도 증가 시간

    private Image panelImage;

    void Start()
    {
        yourButton.onClick.AddListener(OnClick);

        if (uiPanel != null)
        {
            uiPanel.SetActive(false); // 초기에는 비활성화 상태
            panelImage = uiPanel.GetComponent<Image>();

            if (panelImage != null)
            {
                Color color = panelImage.color;
                color.a = 0f;         // 초기 투명도 0으로 설정
                panelImage.color = color;
            }
        }
    }

    void OnClick()
    {
        if (uiPanel != null && panelImage != null)
        {
            uiPanel.SetActive(true);      // 패널 활성화
            StartCoroutine(FadeInAndLoadScene("Tutorial")); // 페이드 인 및 씬 이동 코루틴 실행
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