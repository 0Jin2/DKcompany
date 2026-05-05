using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageSpawner : MonoBehaviour
{
    [Header("몬스터 프리팹 리스트 (4개 프리팹 등록)")]
    public GameObject[] objectPrefabs;  // 보스 스테이지에서 사용할 4개의 프리팹

    [Header("몬스터 생성 간격 (초 단위)")]
    public float spawnInterval = 3f;  // 몬스터 생성 간격

    private bool isSpawning = false;  // 중복 실행 방지 플래그

    // 보스 스테이지에서 몬스터를 소환하는 메서드
    public void StartBossStageSpawning()
    {
        // 이미 스폰 중이 아니면 코루틴 시작
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnBossMonsters());
        }
    }

    // 보스 스테이지에서 몬스터를 소환하는 코루틴
    private IEnumerator SpawnBossMonsters()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);  // 간격 대기

            if (objectPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(0, objectPrefabs.Length);  // 랜덤 프리팹 선택
                GameObject spawnedObject = Instantiate(objectPrefabs[randomIndex], transform.position, Quaternion.identity);

                // 디버깅용 로그 (필요 시 제거)
                Debug.Log("보스 스테이지에서 소환된 몬스터: " + spawnedObject.name);
            }
        }
    }
}