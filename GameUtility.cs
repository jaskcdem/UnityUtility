using System;
using UnityEngine;

/// <summary> 共用方法庫 </summary>
public class GameUtility
{
    public GameUtility(Transform transform)
    {
        Debug.Assert(transform != null, "UnKnown gameObject Can't use this Class");
        Transform = transform;
    }

    /// <summary> 操作物體 </summary>
    public Transform Transform { get; set; }
    /// <summary> 操作物體 </summary>
    public Rigidbody Rigidbody { get; set; }
    /// <summary> 操作物體 </summary>
    public CharacterController Character { get; set; }

    /// <summary> 移動速度<para>預設10.0f</para> </summary>
    public float MoveSpeed = 10.0f;
    /// <summary> 轉向速度<para>預設80.0f</para> </summary>
    public float RotSpeed = 80.0f;

    /* 說明：
     * 當旋轉物體中 Rotation 座標 X 的時候為 pitch，旋轉 Y 軸時為 yaw，旋轉 Z 軸時為 roll 。
     * 因此當取得該滑鼠的X, Y 軸移動時的數值，請記得滑鼠X軸移動對應到 yaw，為 Unity 的Y軸，右正左負。
     * 滑鼠 Y軸移動對應到 pitch，為 Unity 的 X軸，上負下正
     */

    void InvokeAction(Action mainAction, Action beforeAction = default, Action afterAction = default)
    {
        if (beforeAction != default) beforeAction();
        mainAction.Invoke();
        if (afterAction != default) afterAction();
    }

    #region << Transform Moving and Rotating >>
    private void TransformMove()
    {
        float h = InputControler.GetHorizontalAxis, v = InputControler.GetVerticalAxis;
        Vector3 moveDirt = new(h, 0, v);
        Transform.position += moveDirt * Time.deltaTime * MoveSpeed;
    }
    /// <summary> 轉向 </summary>
    /// <param name="rotX">水平轉向幅度(對Y轉)</param>
    /// <param name="rotY">垂直轉向幅度(對X轉)</param>
    private void TransformRotate(float rotX = 0.0f, float rotY = 0.0f)
    {
        Vector3 vRot = Vector3.zero;
        if (rotX != 0) vRot.y = rotX * Time.deltaTime * RotSpeed;
        if (rotY != 0) vRot.x = -rotY * Time.deltaTime * RotSpeed;
        if (vRot != Vector3.zero) Transform.Rotate(vRot);
    }

    /// <summary> 向量移動與水平轉向 </summary>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public GameUtility TransformMoveAndRotateY(Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(() =>
        {
            TransformMove();
            TransformRotate(rotX: InputControler.GetMouseXAxis);
        }, beforeAction, afterAction);
        return this;
    }
    /// <summary> 向量移動與垂直轉向 </summary>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public GameUtility TransformMoveAndRotateX(Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(() =>
        {
            TransformMove();
            TransformRotate(rotY: InputControler.GetMouseYAxis);
        }, beforeAction, afterAction);
        return this;
    }
    /// <summary> 向量移動與垂直/水平轉向 </summary>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public GameUtility TransformMoveAndRotateXY(Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(() =>
        {
            TransformMove();
            TransformRotate(rotX: InputControler.GetMouseXAxis, rotY: InputControler.GetMouseYAxis);
        }, beforeAction, afterAction);
        return this;
    }

    /// <summary> 移動與平緩轉向 </summary>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public GameUtility TransformMoveAndRotate(Action beforeAction = default, Action afterAction = default)
    {
        void _main()
        {
            float h = InputControler.GetHorizontalAxis, v = InputControler.GetVerticalAxis;
            Vector3 moveDirt = Transform.forward * v + Transform.right * h;
            float moveStg = moveDirt.magnitude;
            if (moveStg > 0)
            {
                moveDirt.Normalize();
                float angle = Vector3.Angle(Transform.forward, moveDirt); //取得夾角
                float rotAngle = RotSpeed * Time.deltaTime;
                if (angle < rotAngle) Transform.forward = moveDirt;
                else
                {
                    /* Dot 取得內積值
                     * (1) 90度 => dot = 0
                     * (2) 0度 => dot = 1
                     * (3) 180度 => dot = -1
                     */
                    float _sign = Vector3.Dot(Transform.right, moveDirt) > 0 ? 1.0f : -1.0f;
                    Transform.forward = Quaternion.Euler(0, angle * _sign, 0) * Transform.forward;
                }
                Transform.position += Transform.forward * moveStg * Time.deltaTime * MoveSpeed;
            }
        }
        InvokeAction(_main, beforeAction, afterAction);
        return this;
    }
    #endregion

    #region << Transform Quaternion Rotate >>
    Quaternion GetRotQuaternion() => Quaternion.Euler(-InputControler.GetMouseYAxis * Time.deltaTime * RotSpeed, InputControler.GetMouseXAxis * Time.deltaTime * RotSpeed, 0);

    /// <summary> Quaternion轉向 </summary>
    /// <param name="beforeAction">轉向前動作</param>
    /// <param name="afterAction">轉向後動作</param>
    public GameUtility TransformQuaternionRotate(Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(() => Transform.rotation *= GetRotQuaternion(),
            beforeAction, afterAction);
        return this;
    }
    /// <summary> Quaternion轉向 </summary>
    /// <param name="beforeAction">轉向前動作</param>
    /// <param name="afterAction">轉向後動作</param>
    public GameUtility TransformQuaternionRotateForward(Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(() => Transform.forward = GetRotQuaternion() * Transform.forward,
            beforeAction, afterAction);
        return this;
    }
    #endregion

    #region << Transform Moving >>
    /// <summary> 向量移動 </summary>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public GameUtility TransformMoving(Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(TransformMove, beforeAction, afterAction);
        return this;
    }
    /// <summary> 向量移動 </summary>
    /// <param name="moveAction">移動動作</param>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public GameUtility TransformMoving(Action moveAction = default, Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(moveAction, beforeAction, afterAction);
        return this;
    }
    #endregion

    #region << Rigidbody Moving/Rotating >>
    /// <summary> 物體移動 </summary>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public void RigidbodyMoving(Action beforeAction = default, Action afterAction = default)
    {
        if (Rigidbody)
        {
            InvokeAction(() =>
            {
                Vector3 moveDirt = new(InputControler.GetHorizontalAxis, 0, InputControler.GetVerticalAxis);
                Rigidbody.MovePosition(Rigidbody.position + moveDirt * Time.deltaTime * MoveSpeed);
            }, beforeAction, afterAction);
        }
    }
    /// <summary> 物體移動 </summary>
    /// <param name="move_x">水平移動量</param>
    /// <param name="move_z">垂直移動量</param>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public void RigidbodyMoving(float move_x, float move_z, Action beforeAction = default, Action afterAction = default)
    {
        if (Rigidbody)
        {
            InvokeAction(() =>
            {
                Vector3 moveDirt = new(move_x, 0, move_z);
                Rigidbody.MovePosition(Rigidbody.position + moveDirt * Time.deltaTime * MoveSpeed);
            }, beforeAction, afterAction);
        }
    }
    /// <summary> 物體移動 </summary>
    /// <param name="moveDirt">移動方向</param>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public void RigidbodyMoving(Vector3 moveDirt, Action beforeAction = default, Action afterAction = default)
    {
        if (Rigidbody)
        {
            InvokeAction(() => Rigidbody.MovePosition(Rigidbody.position + moveDirt * Time.deltaTime * MoveSpeed), beforeAction, afterAction);
        }
    }
    /// <summary> 物體移動 </summary>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public void RigidbodyVelocityMoving(Action beforeAction = default, Action afterAction = default)
    {
        if (Rigidbody)
        {
            Vector3 inputDir = new(InputControler.GetHorizontalAxis, 0, InputControler.GetVerticalAxis);
            RigidbodyVelocityMoving(inputDir, beforeAction, afterAction);
        }
    }
    /// <summary> 物體移動 </summary>
    /// <param name="velDirt">移動方向</param>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public void RigidbodyVelocityMoving(Vector3 velDirt, Action beforeAction = default, Action afterAction = default)
    {
        void _main()
        {
            Vector3 vel = Rigidbody.velocity;
            if (velDirt.magnitude > 1.0f) velDirt.Normalize();
            if (velDirt.x != 0) vel.x = velDirt.x * MoveSpeed;
            if (velDirt.z != 0) vel.z = velDirt.z * MoveSpeed;
            Rigidbody.velocity = vel;
        }
        if (Rigidbody)
        {
            InvokeAction(_main, beforeAction, afterAction);
        }
    }
    /// <summary> 物體Quaternion轉向 </summary>
    /// <param name="beforeAction">轉向前動作</param>
    /// <param name="afterAction">轉向後動作</param>
    public void RigidbodyRotating(Action beforeAction = default, Action afterAction = default)
    {
        void _main()
        {
            float x = InputControler.GetMouseXAxis, y = InputControler.GetMouseYAxis;
            Quaternion qRot = Quaternion.Euler(-y * Time.deltaTime * RotSpeed, x * Time.deltaTime * RotSpeed, 0);
            Rigidbody.MoveRotation(qRot);
        }
        if (Rigidbody)
        {
            InvokeAction(_main, beforeAction, afterAction);
        }
    }
    #endregion

    #region << Character Moving >>
    float GetGravityVectorY(float move_y = 0.0f) => move_y != 0.0f ? move_y : Transform.forward.y * Physics.gravity.y * Time.deltaTime;
    Vector3 MoveDirtUpdate(Vector3 moveDirt, float move_x, float move_z, float move_y)
    {
        moveDirt.x = MoveSpeed * move_x;
        moveDirt.z = MoveSpeed * move_z;
        moveDirt.y = GetGravityVectorY(move_y);
        return moveDirt;
    }

    /// <summary> 角色移動 </summary>
    /// <param name="move_y">墜落移動量(預設重力)</param>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public void CharacterSimpleMoving(float move_y = 0.0f, Action beforeAction = default, Action afterAction = default)
    {
        if (Character)
        {
            InvokeAction(() => Character.SimpleMove(MoveDirtUpdate(Vector3.zero, InputControler.GetHorizontalAxis, InputControler.GetVerticalAxis, move_y)),
               beforeAction, afterAction);
        }
    }
    /// <summary> 角色移動 </summary>
    /// <param name="move_x">水平移動量</param>
    /// <param name="move_z">垂直移動量</param>
    /// <param name="move_y">墜落移動量(預設重力)</param>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public void CharacterSimpleMoving(float move_x, float move_z, float move_y = 0.0f, Action beforeAction = default, Action afterAction = default)
    {
        if (Character)
        {
            InvokeAction(() => Character.SimpleMove(MoveDirtUpdate(Vector3.zero, move_x, move_z, move_y)),
              beforeAction, afterAction);
        }
    }
    /// <summary> 角色移動 </summary>
    /// <param name="moveDirt">移動方向</param>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public void CharacterMoving(Vector3 moveDirt, Action beforeAction = default, Action afterAction = default)
    {
        if (Character)
        {
            InvokeAction(() => Character.Move(MoveDirtUpdate(moveDirt, InputControler.GetHorizontalAxis, InputControler.GetVerticalAxis, moveDirt.y)),
                beforeAction, afterAction);
        }
    }
    /// <summary> 角色移動 </summary>
    /// <param name="move_x">水平移動量</param>
    /// <param name="move_z">垂直移動量</param>
    /// <param name="move_y">墜落移動量(預設重力)</param>
    /// <param name="beforeAction">移動前動作</param>
    /// <param name="afterAction">移動後動作</param>
    public void CharacterMoving(float move_x, float move_z, float move_y = 0.0f, Action beforeAction = default, Action afterAction = default)
    {
        if (Character)
        {
            InvokeAction(() => Character.Move(MoveDirtUpdate(Transform.forward, move_x, move_z, move_y)),
                beforeAction, afterAction);
        }
    }
    #endregion

    #region << Drow in OnDrawGizmos() >>
    /// <summary> 繪製前方半圓 </summary>
    /// <param name="radius">半徑</param>
    /// <param name="lineHight">繪製高度</param>
    /// <param name="drawColor">繪製線顏色</param>
    public void DrowHalfCircle(float radius, float lineHight = 0.0f, Color drawColor = default)
    {
        if (radius <= 0.0f) return;
        Gizmos.color = drawColor == default ? Color.green : drawColor;
        Vector3 vStart = Transform.position + Transform.right * radius;
        if (lineHight != 0.0f) vStart.y = lineHight;
        for (int i = 1; i <= 180; i++)
        {
            Vector3 vRot = Quaternion.Euler(0, i * -1.0f, 0) * Transform.right,
                vEnd = Transform.position + vRot * radius;
            if (lineHight != 0.0f) vEnd.y = lineHight;
            Gizmos.DrawLine(vStart, vEnd);
            vStart = vEnd;
        }
    }
    #endregion
}
