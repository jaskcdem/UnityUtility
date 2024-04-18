using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>  </summary>
/// <remarks>using unity method : Start, Update(Public), OnDrawGizmos</remarks>
public class BasePlayer : MonoBehaviour
{
    public float AttackRange = 1.5f;
    GameUtility gu;

    private void Start()
    {
        gu = new(transform);
    }

    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Attacking();
        }
    }

    private void OnDrawGizmos()
    {
        gu ??= new(transform);
        gu.DrawWireSphere(AttackRange, Color.cyan);
    }

    public virtual void Attacking()
    {
        var eList = GlobalValues.AliveEnemies;
        Vector3 selfPos = transform.position;
        BaseEnemy enemy;
        for (int i = 0; i < eList.Count; i++)
        {
            enemy = eList[i];
            float d = Vector3.Distance(selfPos, eList[i].transform.position);
            if (d < AttackRange)
            {
                Debug.Log("Attack---!!!");
                enemy.Damage();
            }
        }
    }
}
