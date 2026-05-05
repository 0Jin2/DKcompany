using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FindPlayer : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        // CinemachineVirtualCamera 컴포넌트를 가져옵니다.
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        // 코루틴 시작
        StartCoroutine(FindAndAssignPlayer());
    }

    private IEnumerator FindAndAssignPlayer()
    {
        GameObject player = null;

        // Player 태그가 붙은 오브젝트를 찾을 때까지 반복
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null; // 한 프레임 대기
        }

        if (virtualCamera != null)
        {
            // Follow 속성에 Player 오브젝트를 할당합니다.
            virtualCamera.Follow = player.transform;
        }
        else
        {
            Debug.LogWarning("CinemachineVirtualCamera를 찾을 수 없습니다.");
        }
    }
}