using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupLayerManager : MonoBehaviour
{
    void Start()
    {
        // 처음 씬이 로드될 때 한 번만 'Popup' 태그가 붙은 UI를 최상단으로 이동
        BringPopupsToFront();
    }

    void BringPopupsToFront()
    {
        // 모든 Canvas 오브젝트를 찾음
        Canvas[] canvases = FindObjectsOfType<Canvas>();

        foreach (Canvas canvas in canvases)
        {
            // Canvas의 모든 자식 오브젝트를 순회하며 'Popup' 태그가 있는지 확인
            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                Transform child = canvas.transform.GetChild(i);

                // 자식 오브젝트가 'Popup' 태그를 가지고 있는지 확인
                if (child.CompareTag("Popup"))
                {
                    // 'Popup' 요소를 해당 Canvas의 마지막 자식으로 이동
                    child.SetSiblingIndex(canvas.transform.childCount - 1);
                }
            }
        }
    }
}