using System;
using System.Collections;
using System.Collections.Generic;

public class FSMSystem
{
    public string WanderTag { get; set; } = "WanderPoint";
    private List<FSMState> StateList;
    private Dictionary<FSMTransition, FSMState> GlobalMap;
    private FSMStateID currentStateID;
    public FSMStateID CurrentStateID => currentStateID;
    private FSMState currentState;
    public FSMState CurrentState => currentState;
    AIData data;

    public FSMSystem(AIData data)
    {
        this.data = data;
        StateList = new List<FSMState>();
        GlobalMap = new Dictionary<FSMTransition, FSMState>();
    }

    public void AddGlobalTransition(FSMTransition t, FSMState s)
    {
        GlobalMap.Add(t, s);
    }

    public void PerformGlobalTransition(FSMTransition t)
    {
        if (GlobalMap.ContainsKey(t))
        {
            currentState.DoBeforeLeave(data);
            currentState = GlobalMap[t];
            currentState.DoBeforeEnter(data);
            currentStateID = currentState.StateID;
        }
    }

    public void AddState(FSMState s)
    {
        if (s == null)
        {
            return;
        }

        if (StateList.Count == 0)
        {
            StateList.Add(s);
            currentState = s;
            currentStateID = s.StateID;
            return;
        }

        foreach (FSMState state in StateList)
        {
            if (state.StateID == s.StateID)
            {
                return;
            }
        }
        StateList.Add(s);
    }
    public void AddState(params FSMState[] arrs)
    {
        foreach (FSMState s in arrs) AddState(s);
    }

    public void DeleteState(FSMStateID id)
    {
        if (id == FSMStateID.NullStateID)
        {
            return;
        }

        foreach (FSMState state in StateList)
        {
            if (state.StateID == id)
            {
                StateList.Remove(state);
                return;
            }
        }
    }
    public void DeleteState(params FSMStateID[] ids)
    {
        foreach (FSMStateID id in ids) DeleteState(id);
    }

    public void PerformTransition(FSMTransition trans)
    {
        if (trans == FSMTransition.NullTransition)
        {
            return;
        }

        FSMState state = currentState.TransitionTo(trans);
        if (state == null)
        {
            return;
        }

        // Update the currentStateID and currentState		
        currentState.DoBeforeLeave(data);
        currentState = state;
        currentStateID = state.StateID;
        currentState.DoBeforeEnter(data);
    }

    public void DoState()
    {
        currentState.CheckCondition(data);
        currentState.Do(data);
    }

}
