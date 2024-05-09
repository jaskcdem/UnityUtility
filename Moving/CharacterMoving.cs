using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoving : MonoBehaviour
{
    CharacterController character;
    [SerializeField] float moveSpeed = 10.0f, rotSpeed = 80.0f;
    float move_x = 0.0f, move_z = 0.0f;
    float radius = 1.3f, lineHight = 2;
    GameUtility gu;

    void Start()
    {
        character = GetComponent<CharacterController>();
        gu = new(transform) { MoveSpeed = moveSpeed, RotSpeed = rotSpeed };
    }

    void Update()
    {
        move_x = InputControler.GetHorizontalAxis;
        move_z = InputControler.GetVerticalAxis;

        gu.TransformQuaternionRotate();
    }

    void FixedUpdate()
    {
        gu.Character = character;
        gu.CharacterSimpleMoving(move_x, move_z);
    }

    private void OnDrawGizmos()
    {
        gu ??= new(transform);
        gu.DrowHalfCircle(radius, lineHight);
    }
}
