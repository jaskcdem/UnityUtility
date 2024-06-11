using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private Transform Target = null;
    [SerializeField] private float FollowSpeed = 6.0f;
    float checkingTime;

    private void Start()
    {
        Debug.Assert(Target != null, "Null Target is not allow!");
    }

    void Update()
    {
        if (!Target) return;

        float followSpeed = (FollowSpeed * Time.deltaTime);
        Vector3 pos = this.transform.position;
        pos = Vector3.Lerp(pos, Target.position, followSpeed); //線性內插位置
        this.transform.position = pos;

        GameObject player = MonoGameManager.Instance.GetPlayer();
        //camera collide
        if (player.TryGetComponent<SapphiControl>(out SapphiControl playerCtl))
        {
            playerCtl.CameraCollideCheck();
        }
    }

    private void FixedUpdate()
    {
        checkingTime += Time.deltaTime;
        if (checkingTime > 10)
        {
            GameObject player = MonoGameManager.Instance.GetPlayer();
            //camera collide
            if (player.TryGetComponent<SapphiControl>(out SapphiControl playerCtl))
            {
                playerCtl.CamerCheckDistence();
            }
            checkingTime = 0;
        }
    }
}
