using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private Transform m_Target = null;
    [SerializeField] private float m_FollowSpeed = 6.0f;
    private void Start()
    {
        Debug.Assert(m_Target != null, "Null Target is not allow!");
    }

    private void FixedUpdate()
    {
        if (!m_Target) return;

        float followSpeed = (m_FollowSpeed * Time.deltaTime);
        Vector3 pos = this.transform.position;
        pos = Vector3.Lerp(pos, m_Target.position, followSpeed); //線性內插位置
        this.transform.position = pos;
    }

}
