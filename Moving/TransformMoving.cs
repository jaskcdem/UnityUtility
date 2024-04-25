using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformMoving : MonoBehaviour
{
    [SerializeField] float moveSpeed = 20.0f, rotSpeed = 80.0f;
    float radius = 2.0f, lineHight = 1;
    GameUtility gu;

    void Start()
    {
        gu = new(transform) { MoveSpeed = moveSpeed, RotSpeed = rotSpeed };
    }

    void Update()
    {
        gu.TransformMoveAndRotateX();
    }

    private void OnDrawGizmos()
    {
        gu ??= new(transform);
        gu.DrowHalfCircle(radius);
    }
}
