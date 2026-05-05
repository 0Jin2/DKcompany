using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSceneMove : MonoBehaviour
{
    private static PlayerSceneMove instance;
    private GameObject player;

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
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        MovePlayerToTP();
    }

    public void MovePlayerToTP()
    {
        GameObject tpObject = GameObject.FindGameObjectWithTag("TP");  // TP 태그로 오브젝트 찾기
        if (tpObject != null && player != null)
        {
            player.transform.position = tpObject.transform.position;
            StartCoroutine(HoldPositionForSeconds(0.1f));
        }
    }

    IEnumerator HoldPositionForSeconds(float duration)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;  // 플레이어 움직임 비활성화
        }

        yield return new WaitForSeconds(duration);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;  // 플레이어 움직임 다시 활성화
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}