using System.Collections;
using UnityEngine;
using Cinemachine;

public class FindPB : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    [Header("BossStageSpawner 스크립트 참조 오브젝트")]
    public BossStageSpawner spawner; // BossStageSpawner로 변경

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        StartCoroutine(FindAndAssignBoss());  // 보스 할당 코루틴 시작
    }

    public IEnumerator FindAndAssignBoss()
    {
        GameObject boss = null;

        while (boss == null)
        {
            boss = GameObject.FindGameObjectWithTag("BOSS");  // 보스 오브젝트 찾기
            yield return null;
        }

        if (virtualCamera != null)
        {
            virtualCamera.Follow = boss.transform;  // 버츄얼 카메라가 보스를 따라가도록 설정
        }
        else
        {
            Debug.LogWarning("VirtualCamera를 찾을 수 없습니다.");
        }
    }

    public void SwitchToPlayer()
    {
        StartCoroutine(FindAndAssignPlayer());
    }

    private IEnumerator FindAndAssignPlayer()
    {
        GameObject player = null;

        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");  // 플레이어 오브젝트 찾기
            yield return null;
        }

        if (virtualCamera != null)
        {
            virtualCamera.Follow = player.transform;  // 카메라가 플레이어를 따라가도록 설정

            // 플레이어의 움직임을 활성화
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.EnableMovement();  // 플레이어 움직임 허용
            }

            yield return new WaitForSeconds(1f);

            if (spawner != null)
            {
                spawner.gameObject.SetActive(true);  // 스폰 오브젝트 활성화
                spawner.StartBossStageSpawning();  // 보스 스테이지 몬스터 스폰 시작
            }
            else
            {
                Debug.LogWarning("BossStageSpawner 오브젝트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("CinemachineVirtualCamera를 찾을 수 없습니다.");
        }
    }
}
