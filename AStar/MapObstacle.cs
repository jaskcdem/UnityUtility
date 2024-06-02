using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObstacle : MonoBehaviour
{
    public const string TagStr = "Obstacle";
    public enum State
    {
        NONE = -1,
        OUTSIDE,
        INSIDE,
        COL
    }

    public float draw_radius;
    public float Speed;
    [HideInInspector] public State ObtState = State.NONE;


    private void OnDrawGizmos()
    {
        switch (ObtState)
        {
            case State.INSIDE: Gizmos.color = Color.yellow; break;
            case State.COL: Gizmos.color = Color.red; break;
            default: Gizmos.color = Color.white; break;
        }
        Gizmos.DrawWireSphere(this.transform.position, draw_radius);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * Speed);
        //Gizmos.color = Color.yellow;
        //Vector3 vLeftStart = this.transform.position - this.transform.right * draw_radius;
        //Vector3 vLeftEnd = vLeftStart + this.transform.forward * Speed;
        //Gizmos.DrawLine(vLeftStart, vLeftEnd);
        //Vector3 vRightStart = this.transform.position + this.transform.right * draw_radius;
        //Vector3 vRightEnd = vRightStart + this.transform.forward * Speed;
        //Gizmos.DrawLine(vRightStart, vRightEnd);
        //Gizmos.DrawLine(vLeftEnd, vRightEnd);
    }
}
