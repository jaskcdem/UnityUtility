using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FSMTransition
{
    NullTransition = 0,
    Go_Idle,
    Go_MoveTo,
    Go_Chase,
    Go_Attack,
    Go_Dead,
}

public enum FSMStateID
{
    NullStateID = 0,
    IdleStateID,
    MoveToStateID,
    ChaseStateID,
    AttackStateID,
    DeadStateID,
}

public class FSMState
{
    public FSMStateID StateID;
    public Dictionary<FSMTransition, FSMState> Map;
    public float CurrentTime;
    public AnimatorHelper Animator;

    public FSMState()
    {
        StateID = FSMStateID.NullStateID;
        CurrentTime = 0.0f;
        Map = new Dictionary<FSMTransition, FSMState>();
    }

    public void AddTransition(FSMTransition trans, FSMState toState)
    {
        if (Map.ContainsKey(trans)) return;
        Map.Add(trans, toState);
    }
    public void AddTransition(params (FSMTransition tran, FSMState toState)[] tranArr)
    {
        foreach (var (tran, toState) in tranArr) AddTransition(tran, toState);
    }
    public void DelTransition(FSMTransition trans)
    {
        if (Map.ContainsKey(trans)) Map.Remove(trans);
    }
    public void DelTransition(params FSMTransition[] tranArr)
    {
        foreach (var tran in tranArr) DelTransition(tran);
    }

    public FSMState TransitionTo(FSMTransition trans) => !Map.ContainsKey(trans) ? null : Map[trans];

    public virtual void DoBeforeEnter(AIData data) { }

    public virtual void DoBeforeLeave(AIData data) { }

    public virtual void Do(AIData data) { }

    public virtual void CheckCondition(AIData data) { }
}


public class FSMIdleState : FSMState
{
    private float IdleTime, defaultMin = 1.0f, defaultMax = 3.0f;

    public FSMIdleState()
    {
        StateID = FSMStateID.IdleStateID;
        IdleTime = Random.Range(defaultMin, defaultMax);
    }

    public override void DoBeforeEnter(AIData data)
    {
        CurrentTime = 0.0f;
        IdleTime = Random.Range(defaultMin, defaultMax);
    }

    public override void Do(AIData data)
    {
        CurrentTime += Time.deltaTime;
    }

    public override void CheckCondition(AIData data)
    {
        bool bAttack = false;
        GameObject go = AIFunction.CheckEnemyInSight(data, ref bAttack);
        if (go != null)
        {
            data.TargetObject = go;
            data.FSMSystem.PerformTransition(bAttack ? FSMTransition.Go_Attack : FSMTransition.Go_Chase);
            return;
        }
        if (CurrentTime > IdleTime)
        {
            data.FSMSystem.PerformTransition(FSMTransition.Go_MoveTo);
        }
    }

    public void ResetIdleTime(float time) => IdleTime = time;
}

public class FSMMoveToState : FSMState
{
    private int CurrentWanderPt;
    private GameObject[] wanderPoints;
    string wanderPointTag { get; set; }

    public FSMMoveToState(string wpTag)
    {
        wanderPointTag = wpTag;
        StateID = FSMStateID.MoveToStateID;
        CurrentWanderPt = -1;
        wanderPoints = GameObject.FindGameObjectsWithTag(wanderPointTag);
    }

    public override void DoBeforeEnter(AIData data)
    {
        if (wanderPoints.Length == 0) return;
        int iNewPt = Random.Range(0, wanderPoints.Length);
        if (CurrentWanderPt == iNewPt) return;
        CurrentWanderPt = iNewPt;
        data.target = wanderPoints[CurrentWanderPt].transform.position;
        data.bMoving = true;
    }

    public override void Do(AIData data)
    {
        if (!SteeringBehavior.CollisionAvoid(data))
            SteeringBehavior.Seek(data);
        SteeringBehavior.Move(data);
    }

    public override void CheckCondition(AIData data)
    {
        bool bAttack = false;
        GameObject go = AIFunction.CheckEnemyInSight(data, ref bAttack);
        if (go != null)
        {
            data.TargetObject = go;
            data.FSMSystem.PerformTransition(bAttack ? FSMTransition.Go_Attack : FSMTransition.Go_Chase);
            return;
        }

        if (!data.bMoving)
        {
            data.FSMSystem.PerformTransition(FSMTransition.Go_Idle);
        }
    }
}

public class FSMChaseState : FSMState
{
    public FSMChaseState()
    {
        StateID = FSMStateID.ChaseStateID;
    }

    public override void Do(AIData data)
    {
        data.target = data.TargetObject.transform.position;
        if (!SteeringBehavior.CollisionAvoid(data))
            SteeringBehavior.Seek(data);
        SteeringBehavior.Move(data);
    }

    public override void CheckCondition(AIData data)
    {
        bool bAttack = false, bCheck = AIFunction.CheckTargetEnemyInSight(data, data.TargetObject, ref bAttack);
        if (!bCheck)
        {
            data.FSMSystem.PerformTransition(FSMTransition.Go_Idle);
            return;
        }
        if (bAttack)
        {
            data.FSMSystem.PerformTransition(FSMTransition.Go_Attack);
        }
    }
}

public class FSMAttackState : FSMState
{
    private float fAttackTime = 0.0f;
    public FSMAttackState()
    {
        StateID = FSMStateID.AttackStateID;
    }

    public override void DoBeforeEnter(AIData data)
    {
        fAttackTime = Random.Range(1.0f, 3.0f);
        CurrentTime = 0.0f;
    }

    public override void Do(AIData data)
    {
        // Check Animation complete.
        //...

        if (CurrentTime > fAttackTime)
        {
            CurrentTime = 0.0f;
            // Do attack.
        }
        CurrentTime += Time.deltaTime;
    }

    public override void CheckCondition(AIData data)
    {
        bool bAttack = false, bCheck = AIFunction.CheckTargetEnemyInSight(data, data.TargetObject, ref bAttack);
        if (!bCheck)
        {
            data.FSMSystem.PerformTransition(FSMTransition.Go_Idle);
            return;
        }
        if (!bAttack)
        {
            data.FSMSystem.PerformTransition(FSMTransition.Go_Chase);
            return;
        }
    }
}

public class FSMDeadState : FSMState
{
    private float DeadTime, defaultMin = 1.0f, defaultMax = 3.0f;
    public FSMDeadState()
    {
        StateID = FSMStateID.DeadStateID;
    }

    public override void DoBeforeEnter(AIData data)
    {
        CurrentTime = 0.0f;
        DeadTime = Random.Range(defaultMin, defaultMax);
    }

    public override void Do(AIData data)
    {
        CurrentTime += Time.deltaTime;
    }

    public override void CheckCondition(AIData data)
    {
        GameObject go = data.Go;
        if (CurrentTime > DeadTime && go != null) go.SetActive(false);
    }

    public void ResetDeadTime(float time) => DeadTime = time;
}