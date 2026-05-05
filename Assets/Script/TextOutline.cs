using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextOutline : MonoBehaviour
{
    public Color backgroundColor = Color.black; // 배경 색상
    public Vector4 backgroundPadding = new Vector4(5, 5, 5, 5); // 배경 여백 (좌, 상, 우, 하)
    private TextMeshProUGUI textMeshPro;
    private RectTransform rectTransform;
    private GameObject background;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();

        // 배경 오브젝트를 생성
        background = new GameObject("TextBackground");
        background.transform.SetParent(transform);

        // 배경에 Image 컴포넌트를 추가
        var backgroundImage = background.AddComponent<UnityEngine.UI.Image>();
        backgroundImage.color = backgroundColor;

        // 배경의 RectTransform을 설정합니다.
        var backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0, 0);
        backgroundRect.anchorMax = new Vector2(1, 1);
        backgroundRect.pivot = new Vector2(0.5f, 0.5f);

        UpdateBackground();
    }

    void Update()
    {
        UpdateBackground();
    }

    void UpdateBackground()
    {
        if (textMeshPro != null && background != null)
        {
            Vector2 textSize = textMeshPro.GetPreferredValues();
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.sizeDelta = new Vector2(textSize.x + backgroundPadding.x + backgroundPadding.z, textSize.y + backgroundPadding.y + backgroundPadding.w);
            backgroundRect.anchoredPosition = new Vector2(0, 0);
        }
    }
}