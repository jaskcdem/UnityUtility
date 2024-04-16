using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    public float AttackRange = 1.5f;
    GameUtility gu;

    private void Start()
    {
        gu = new(transform);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            StartCoroutine(Attacking());
        }
    }

    private void OnDrawGizmos()
    {
        gu ??= new(transform);
        gu.DrawWireSphere(AttackRange, Color.cyan);
    }

    IEnumerator Attacking()
    {
        var eList = GlobalValues.AliveEnemies;
        Debug.Log("Attack---!!!");
        Vector3 selfPos = transform.position;
        BaseEnemy enemy;
        for (int i = 0; i < eList.Count; i++)
        {
            enemy = eList[i];
            float d = Vector3.Distance(selfPos, eList[i].transform.position);
            if (d < AttackRange)
            {
                enemy.Damage();
            }
        }
        yield return 0;
    }
}
