using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIData
{
    [Tooltip("those who take the data")] public GameObject Go;
    //behavior
    [Tooltip("Collision avoid sphere radius")] public float Radius;
    [Tooltip("detect length")] public float ProbeLength;
    [Tooltip("detect Distance")] public float DecDistance;
    [HideInInspector] public float Speed;
    [HideInInspector] public float moveAmount;
    [Tooltip("max move speed")] public float MaxSpeed;
    [HideInInspector] [Tooltip("rotate y value")] public float Rot;
    [Tooltip("max rotate y value")] public float MaxRot;
    [HideInInspector] [Tooltip("rotate delta")] public float CurrentDeltaRot;
    [Tooltip("see-able range")] public float Sight;
    [Tooltip("attack-able range")] public float AttackRange;
    [HideInInspector] public float AttackTime;
    public float Hp;
    [HideInInspector] public GameObject TargetObject;

    [HideInInspector] public Vector3 target;
    [HideInInspector] public Vector3 TargetDir;
    [HideInInspector] public Vector3 CurrentVector;
    [HideInInspector] [Tooltip("temp saving of turning force")] public float TempTurnForce;
    [HideInInspector] public float MoveForce;
    [HideInInspector] public bool bMoving;

    [HideInInspector] public bool bCol;

    [HideInInspector] public FSMSystem FSMSystem;

    /// <summary> Get directional vectors </summary>
    /// <remarks> <seealso href="https://forum.unity.com/threads/can-someone-explain-target-position-transform-position.196147/"/> </remarks>
    public Vector3 GetDirectionalVector() => target - GetPosition();

    public Vector3 GetPosition() => Go.transform.position;
    public void SetPosition(Vector3 pos) => Go.transform.position = pos;

    public Vector3 GetForward() => Go.transform.forward;
    public void SetForward(Vector3 forward) => Go.transform.forward = forward;

    public Vector3 GetRight() => Go.transform.right;
}


public class AIFunction
{
    /// <summary> look for player in Sight </summary>
    /// <param name="data"/>
    /// <param name="bAttack">Does player in AttackRange? </param>
    public static GameObject CheckEnemyInSight(AIData data, ref bool bAttack)
    {
        GameObject player = MonoGameManager.Instance.GetPlayer();
        Vector3 v = player.transform.position - data.GetPosition();
        float fDist = v.magnitude;
        if (fDist < data.AttackRange)
        {
            bAttack = true;
            return player;
        }
        else if (fDist < data.Sight)
        {
            bAttack = false;
            return player;
        }
        return null;
    }
    /// <summary> look for player in Sight </summary>
    /// <param name="data"/><param name="player"/>
    /// <param name="bAttack">Does player in AttackRange? </param>
    public static GameObject CheckEnemyInSight(AIData data, GameObject player, ref bool bAttack)
    {
        Vector3 v = player.transform.position - data.GetPosition();
        float fDist = v.magnitude;
        if (fDist < data.AttackRange)
        {
            bAttack = true;
            return player;
        }
        else if (fDist < data.Sight)
        {
            bAttack = false;
            return player;
        }
        return null;
    }

    /// <summary> look for target in Sight </summary>
    /// <param name="data"/><param name="target"/>
    /// <param name="bAttack">Does target in AttackRange? </param>
    public static bool CheckTargetEnemyInSight(AIData data, GameObject target, ref bool bAttack)
    {
        GameObject go = target;
        Vector3 v = go.transform.position - data.Go.transform.position;
        float fDist = v.magnitude;
        if (fDist < data.AttackRange)
        {
            bAttack = true;
            return true;
        }
        else if (fDist < data.Sight)
        {
            bAttack = false;
            return true;
        }
        return false;
    }
}