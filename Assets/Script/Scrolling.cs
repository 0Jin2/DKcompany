using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrolling : MonoBehaviour
{
    [SerializeField] [Range(1f, 20f)] float speed = 1f; //스크롤링 속도
    [SerializeField] float posValus;                    //스크롤링 오브젝트 이미지 크기

    Vector2 startPos;
    float newPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        newPos = Mathf.Repeat(Time.time * speed, posValus);
        transform.position = startPos + Vector2.right * newPos;
    }
}
