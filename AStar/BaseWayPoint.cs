using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWayPoint : MonoBehaviour
{
    public List<GameObject> Neibors = null;
    public bool bLink = false;

    private void OnDrawGizmos()
    {
        if (Neibors != null && Neibors.Count > 0)
        {
            foreach (GameObject g in Neibors)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(this.transform.position, g.transform.position);
            }
        }
    }
}
