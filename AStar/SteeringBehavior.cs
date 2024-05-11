using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehavior : MonoBehaviour
{
    public static void Seek(ref AIData data)
    {
        Transform t = data.go.transform;
        Vector3 currentPos = t.position, vFor = t.forward, vMove = data.vTarget - currentPos;
        vMove.y = 0.0f;
        float fDist = vMove.magnitude;
        vMove.Normalize();
        float fLimit = data.fSpeed * Time.deltaTime + 0.01f;
        if (fDist < fLimit)
        {
            Vector3 tPos = data.vTarget;
            tPos.y = currentPos.y;
            t.position = tPos;
            return;
        }

        float fDot = Vector3.Dot(vMove, vFor);
        if (fDot > 1.0f)
        {
            fDot = 1.0f;
            t.forward = vMove;
        }
        else
        {
            float fSinLen = Mathf.Sqrt(1 - fDot * fDot), fDot2 = Vector3.Dot(t.right, vMove);
            //float fRad = Mathf.Acos(fDot);
            //fSinLen = Mathf.Sin(fRad);
            float fMag = 0.1f;
            if (fDot < 0) fMag = 1.0f;
            if (fDot2 < 0) fMag = -fMag;
            vFor += t.right * fSinLen * fMag;
            vFor.Normalize();
            t.forward = vFor;
        }

        // Move.
        float fForForce = fDot, fRatio = 1.0f, fAcc = fForForce * fRatio, fAcc2 = fDist / data.fArriveRange;
        if (fAcc2 > 1.0f) fAcc2 = 1.0f;
        else fAcc2 = -(1.0f - fAcc2);

        data.fSpeed = data.fSpeed + fAcc * Time.deltaTime + fAcc2 * Time.deltaTime;
        if (data.fSpeed > data.fMaxSpeed) data.fSpeed = data.fMaxSpeed;
        else if (data.fSpeed < 0.01f) data.fSpeed = 0.01f;
        t.position = currentPos + t.forward * data.fSpeed * Time.deltaTime;
    }
}
