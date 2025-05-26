using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, 0);
    [SerializeField] private Transform ball;
    private bool ballInView = false;
    [SerializeField] private SpriteRenderer arrow;

    void Start()
    {
        arrow.enabled = false;
    }

    void FixedUpdate()
    {
        transform.position = target.position + offset;
    }

    private void Update()
    {
        Vector3 Pos = Camera.main.WorldToViewportPoint(ball.position);
        if (Pos.x >= 0f && Pos.x <= 1f && Pos.y >= 0f && Pos.y <= 1f)
        {
            ballInView = true;
        } else
        {
            ballInView = false;
        }

        if (!ballInView)
        {
            arrow.enabled = true;
        } else
        {
            arrow.enabled = false;
        }

        Vector3 dir = Quaternion.LookRotation(ball.position - target.position).eulerAngles;
        dir.x = 90f;
        dir.z = 0f;
        arrow.transform.rotation = Quaternion.Euler(dir).normalized;
    }
}
