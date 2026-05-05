using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialLoadingPage : MonoBehaviour
{
    public float fadeOutDuration = 1f;

    private Image panelImage;

    void Start()
    {
        panelImage = GetComponent<Image>();

        if (panelImage != null)
        {
            StartCoroutine(FadeOutAndDeactivate());
        }
    }

    private IEnumerator FadeOutAndDeactivate()
    {
        float elapsed = 0f;
        Color color = panelImage.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - (elapsed / fadeOutDuration));
            panelImage.color = color;
            yield return null;
        }

        // 투명도가 0이 되면 오브젝트 비활성화
        gameObject.SetActive(false);
    }
}