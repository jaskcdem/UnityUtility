using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>  </summary>
/// <remarks>using unity method : FixedUpdate(Public)</remarks>
public class BaseEnemy : MonoBehaviour
{
    public GlobalValues.EnemyType Type;
    public int MaxHp = 10;
    public int Hp;
    protected bool bDead = false;

    public BaseEnemy()
    {
        Hp = MaxHp;
    }

    public void FixedUpdate()
    {
        if (bDead)
        {
            Deaded();
        }
    }

    public virtual void Init()
    {
        bDead = false;
        Hp = MaxHp;
    }

    public virtual void Damage()
    {
        if (bDead) return;
        Hp--;
        if (Hp <= 0) { Hp = 0; bDead = true; }
        Debug.Log($"{gameObject.name} Leaf Hp: {Hp}");
    }

    public virtual void Deaded()
    {
        var eList = GlobalValues.AliveEnemies;
        eList.RemoveAll(e => e.name.Equals(gameObject.name));
        BattleManager.UnLoadDataToPool(Type, gameObject);
        int _aliveCnt = eList.Count;
        Debug.Log(_aliveCnt switch
        {
            0 => $"No enemies are alive.",
            1 => $"Left only {_aliveCnt} enemy is alive.",
            _ => $"Still {_aliveCnt} enemies are alive.",
        });
        //GlobalValues.AliveEnemies = eList;
    }
}
