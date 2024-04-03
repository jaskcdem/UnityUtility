using System;
using UnityEngine;

/// <summary> �@�Τ�k�w </summary>
public class GameUtility
{
    public GameUtility(Transform transform)
    {
        Debug.Assert(transform != null, "UnKnown gameObject Can't use this Class");
        Transform = transform;
    }

    /// <summary> �ާ@���� </summary>
    public Transform Transform { get; set; }
    /// <summary> �ާ@���� </summary>
    public Rigidbody Rigidbody { get; set; }
    /// <summary> �ާ@���� </summary>
    public CharacterController Character { get; set; }

    /// <summary> ���ʳt��<para>�w�]10.0f</para> </summary>
    public float MoveSpeed = 10.0f;
    /// <summary> ��V�t��<para>�w�]80.0f</para> </summary>
    public float RotSpeed = 80.0f;

    /* �����G
     * ����ફ�餤 Rotation �y�� X ���ɭԬ� pitch�A���� Y �b�ɬ� yaw�A���� Z �b�ɬ� roll �C
     * �]������o�ӷƹ���X, Y �b���ʮɪ��ƭȡA�аO�o�ƹ�X�b���ʹ����� yaw�A�� Unity ��Y�b�A�k�����t�C
     * �ƹ� Y�b���ʹ����� pitch�A�� Unity �� X�b�A�W�t�U��
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
    /// <summary> ��V </summary>
    /// <param name="rotX">������V�T��(��Y��)</param>
    /// <param name="rotY">������V�T��(��X��)</param>
    private void TransformRotate(float rotX = 0.0f, float rotY = 0.0f)
    {
        Vector3 vRot = Vector3.zero;
        if (rotX != 0) vRot.y = rotX * Time.deltaTime * RotSpeed;
        if (rotY != 0) vRot.x = -rotY * Time.deltaTime * RotSpeed;
        if (vRot != Vector3.zero) Transform.Rotate(vRot);
    }

    /// <summary> �V�q���ʻP������V </summary>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
    public GameUtility TransformMoveAndRotateY(Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(() =>
        {
            TransformMove();
            TransformRotate(rotX: InputControler.GetMouseXAxis);
        }, beforeAction, afterAction);
        return this;
    }
    /// <summary> �V�q���ʻP������V </summary>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
    public GameUtility TransformMoveAndRotateX(Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(() =>
        {
            TransformMove();
            TransformRotate(rotY: InputControler.GetMouseYAxis);
        }, beforeAction, afterAction);
        return this;
    }
    /// <summary> �V�q���ʻP����/������V </summary>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
    public GameUtility TransformMoveAndRotateXY(Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(() =>
        {
            TransformMove();
            TransformRotate(rotX: InputControler.GetMouseXAxis, rotY: InputControler.GetMouseYAxis);
        }, beforeAction, afterAction);
        return this;
    }

    /// <summary> ���ʻP���w��V </summary>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
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
                float angle = Vector3.Angle(Transform.forward, moveDirt); //���o����
                float rotAngle = RotSpeed * Time.deltaTime;
                if (angle < rotAngle) Transform.forward = moveDirt;
                else
                {
                    /* Dot ���o���n��
                     * (1) 90�� => dot = 0
                     * (2) 0�� => dot = 1
                     * (3) 180�� => dot = -1
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

    /// <summary> Quaternion��V </summary>
    /// <param name="beforeAction">��V�e�ʧ@</param>
    /// <param name="afterAction">��V��ʧ@</param>
    public GameUtility TransformQuaternionRotate(Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(() => Transform.rotation *= GetRotQuaternion(),
            beforeAction, afterAction);
        return this;
    }
    /// <summary> Quaternion��V </summary>
    /// <param name="beforeAction">��V�e�ʧ@</param>
    /// <param name="afterAction">��V��ʧ@</param>
    public GameUtility TransformQuaternionRotateForward(Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(() => Transform.forward = GetRotQuaternion() * Transform.forward,
            beforeAction, afterAction);
        return this;
    }
    #endregion

    #region << Transform Moving >>
    /// <summary> �V�q���� </summary>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
    public GameUtility TransformMoving(Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(TransformMove, beforeAction, afterAction);
        return this;
    }
    /// <summary> �V�q���� </summary>
    /// <param name="moveAction">���ʰʧ@</param>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
    public GameUtility TransformMoving(Action moveAction = default, Action beforeAction = default, Action afterAction = default)
    {
        InvokeAction(moveAction, beforeAction, afterAction);
        return this;
    }
    #endregion

    #region << Rigidbody Moving/Rotating >>
    /// <summary> ���鲾�� </summary>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
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
    /// <summary> ���鲾�� </summary>
    /// <param name="move_x">�������ʶq</param>
    /// <param name="move_z">�������ʶq</param>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
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
    /// <summary> ���鲾�� </summary>
    /// <param name="moveDirt">���ʤ�V</param>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
    public void RigidbodyMoving(Vector3 moveDirt, Action beforeAction = default, Action afterAction = default)
    {
        if (Rigidbody)
        {
            InvokeAction(() => Rigidbody.MovePosition(Rigidbody.position + moveDirt * Time.deltaTime * MoveSpeed), beforeAction, afterAction);
        }
    }
    /// <summary> ���鲾�� </summary>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
    public void RigidbodyVelocityMoving(Action beforeAction = default, Action afterAction = default)
    {
        if (Rigidbody)
        {
            Vector3 inputDir = new(InputControler.GetHorizontalAxis, 0, InputControler.GetVerticalAxis);
            RigidbodyVelocityMoving(inputDir, beforeAction, afterAction);
        }
    }
    /// <summary> ���鲾�� </summary>
    /// <param name="velDirt">���ʤ�V</param>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
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
    /// <summary> ����Quaternion��V </summary>
    /// <param name="beforeAction">��V�e�ʧ@</param>
    /// <param name="afterAction">��V��ʧ@</param>
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

    /// <summary> ���Ⲿ�� </summary>
    /// <param name="move_y">�Y�����ʶq(�w�]���O)</param>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
    public void CharacterSimpleMoving(float move_y = 0.0f, Action beforeAction = default, Action afterAction = default)
    {
        if (Character)
        {
            InvokeAction(() => Character.SimpleMove(MoveDirtUpdate(Vector3.zero, InputControler.GetHorizontalAxis, InputControler.GetVerticalAxis, move_y)),
               beforeAction, afterAction);
        }
    }
    /// <summary> ���Ⲿ�� </summary>
    /// <param name="move_x">�������ʶq</param>
    /// <param name="move_z">�������ʶq</param>
    /// <param name="move_y">�Y�����ʶq(�w�]���O)</param>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
    public void CharacterSimpleMoving(float move_x, float move_z, float move_y = 0.0f, Action beforeAction = default, Action afterAction = default)
    {
        if (Character)
        {
            InvokeAction(() => Character.SimpleMove(MoveDirtUpdate(Vector3.zero, move_x, move_z, move_y)),
              beforeAction, afterAction);
        }
    }
    /// <summary> ���Ⲿ�� </summary>
    /// <param name="moveDirt">���ʤ�V</param>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
    public void CharacterMoving(Vector3 moveDirt, Action beforeAction = default, Action afterAction = default)
    {
        if (Character)
        {
            InvokeAction(() => Character.Move(MoveDirtUpdate(moveDirt, InputControler.GetHorizontalAxis, InputControler.GetVerticalAxis, moveDirt.y)),
                beforeAction, afterAction);
        }
    }
    /// <summary> ���Ⲿ�� </summary>
    /// <param name="move_x">�������ʶq</param>
    /// <param name="move_z">�������ʶq</param>
    /// <param name="move_y">�Y�����ʶq(�w�]���O)</param>
    /// <param name="beforeAction">���ʫe�ʧ@</param>
    /// <param name="afterAction">���ʫ�ʧ@</param>
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
    /// <summary> ø�s�e��b�� </summary>
    /// <param name="radius">�b�|</param>
    /// <param name="lineHight">ø�s����</param>
    /// <param name="drawColor">ø�s�u�C��</param>
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
