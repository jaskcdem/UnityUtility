using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SteeringBehavior
{
    const float arrivalRange = 0.001f, minSpeed = 0.01f, fMaxCost = 10000.0f, selfNomolize = 1.0f, selfMinesNomolize = -1.0f;
    const float forceMin = -1.0f, forceMax = 1.0f, forceMaxDouble = forceMax * 2, forceCheckGapDown = -0.96f, forceCheckGapUp = 0.96f;
    const float randomMin = -10.0f, randomMax = 10.0f, faceDirt = 0.0001f, offsetForce = -10.0f, distThresholdDefault = 3.0f;
    const float moveForceTimes = 10.0f, moveBigForceTimes = 100.0f, moveGrateForceTimes = 1000.0f;

    /// <summary>
    /// if <paramref name="value"/> out of <paramref name="min"/> and <paramref name="max"/>
    /// <para>reset <paramref name="value"/> to <paramref name="min"/> or <paramref name="max"/></para>
    /// </summary>
    static float Clamp(this float value, float min, float max) => Mathf.Clamp(value, min, max);
    /// <summary>
    /// if <paramref name="value"/> out of negative<paramref name="border"/> and <paramref name="border"/>
    /// <para>reset <paramref name="value"/> to negative <paramref name="border"/> or <paramref name="border"/></para>
    /// </summary>
    static float Clamp(this float value, float border) => value.Clamp(-border, border);

    static bool Arrival(AIData data, Vector3 vec)
    {
        bool _R = false;
        float fDist = vec.magnitude;
        if (fDist < data.moveAmount + arrivalRange)
        {
            Vector3 posGoal = data.target;
            posGoal.y = data.GetPosition().y;
            data.SetPosition(posGoal);
            data.MoveForce = data.TempTurnForce = data.Speed = 0.0f;
            data.bMoving = false;
            _R = true;
        }
        return _R;
    }

    #region << Move >>
    public static void MoveAngle(AIData data)
    {
        if (!data.bMoving) return;
        float fRotAmount = data.TempTurnForce * Time.deltaTime;
        data.Rot += fRotAmount;
        //rotate should range in max or now-rot-delta
        data.Rot = data.Rot.Clamp(data.MaxRot);
        data.Rot = data.Rot.Clamp(data.CurrentDeltaRot);
        Transform t = data.Go.transform;
        t.Rotate(0, data.Rot, 0);
    }
    public static void Move(AIData data)
    {
        List<MapObstacle> _AvoidTargets = MonoGameManager.Instance.GetObstacles();
        Move(data, _AvoidTargets);
    }
    public static void Move(AIData data, List<MapObstacle> _AvoidTargets)
    {
        if (!data.bMoving) return;
        Transform t = data.Go.transform;
        Vector3 cPos = data.GetPosition(), vR = t.right, vOriF = t.forward, vF = data.CurrentVector;
        data.TempTurnForce = data.TempTurnForce.Clamp(data.MaxRot);
        vF += vR * data.TempTurnForce;
        //vF.Normalize();
        t.forward = vF;
        data.Speed += data.MoveForce * Time.deltaTime;
        data.Speed = data.Speed.Clamp(minSpeed, data.MaxSpeed);

        if (!data.bCol && CheckCollision(data, _AvoidTargets))
            t.forward = vOriF;
        else if (data.Speed < minSpeed * 2)
            t.forward = data.TempTurnForce > 0 ? vR : -vR;

        data.moveAmount = data.Speed * Time.deltaTime;
        cPos += t.forward * data.moveAmount;
        t.position = cPos;
    }
    #endregion

    #region << Obstacles check and avoid >>
    public static bool CheckCollision(AIData data)
    {
        List<MapObstacle> _AvoidTargets = MonoGameManager.Instance.GetObstacles();
        return CheckCollision(data, _AvoidTargets);
    }
    public static bool CheckCollision(AIData data, List<MapObstacle> _AvoidTargets)
    {
        if (_AvoidTargets == null) return false;
        Transform ct = data.Go.transform;
        Vector3 cPos = ct.position, cForward = ct.forward, vec;

        float fDist, fDot;
        int iCount = _AvoidTargets.Count;
        for (int i = 0; i < iCount; i++)
        {
            vec = _AvoidTargets[i].transform.position - cPos;
            vec.y = 0.0f;
            fDist = vec.magnitude;
            if (fDist > data.ProbeLength + _AvoidTargets[i].draw_radius)
            {
                _AvoidTargets[i].ObtState = MapObstacle.State.OUTSIDE;
                continue;
            }

            vec.Normalize();
            fDot = Vector3.Dot(vec, cForward);
            if (fDot < 0) //target at the back
            {
                _AvoidTargets[i].ObtState = MapObstacle.State.OUTSIDE;
                continue;
            }
            _AvoidTargets[i].ObtState = MapObstacle.State.INSIDE;
            float fProjDist = fDist * fDot, fDotDist = Mathf.Sqrt(fDist * fDist - fProjDist * fProjDist);
            if (fDotDist > _AvoidTargets[i].draw_radius + data.Radius)
            {
                continue;
            }
            return true;
        }
        return false;
    }

    public static bool CollisionAvoid(AIData data)
    {
        List<MapObstacle> _AvoidTargets = MonoGameManager.Instance.GetObstacles();
        return CollisionAvoid(data, _AvoidTargets);
    }
    public static bool CollisionAvoid(AIData data, List<MapObstacle> _AvoidTargets)
    {
        if (_AvoidTargets == null || _AvoidTargets.Any(a => a == null)) return false;
        Transform ct = data.Go.transform;
        Vector3 cPos = ct.position, cForward = ct.forward;
        data.CurrentVector = cForward;
        Vector3 vec;
        float fFinalDotDist, fFinalProjDist;
        Vector3 vFinalVec = Vector3.forward;
        MapObstacle oFinal = null;
        float fDist, fDot, fFinalDot = 0.0f;
        int iCount = _AvoidTargets.Count;
        float fMinDist = fMaxCost;
        for (int i = 0; i < iCount; i++)
        {
            vec = _AvoidTargets[i].transform.position - cPos;
            vec.y = 0.0f;
            fDist = vec.magnitude;
            if (fDist > data.ProbeLength + _AvoidTargets[i].draw_radius)
            {
                _AvoidTargets[i].ObtState = MapObstacle.State.OUTSIDE;
                continue;
            }

            vec.Normalize();
            fDot = Vector3.Dot(vec, cForward);
            if (fDot < 0)
            {
                _AvoidTargets[i].ObtState = MapObstacle.State.OUTSIDE;
                continue;
            }
            else if (fDot > 1.0f)
            {
                fDot = 1.0f;
            }
            _AvoidTargets[i].ObtState = MapObstacle.State.INSIDE;
            float fProjDist = fDist * fDot;
            float fDotDist = Mathf.Sqrt(fDist * fDist - fProjDist * fProjDist);
            if (fDotDist > _AvoidTargets[i].draw_radius + data.Radius)
            {
                continue;
            }

            if (fDist < fMinDist)
            {
                fMinDist = fDist;
                fFinalDotDist = fDotDist;
                fFinalProjDist = fProjDist;
                vFinalVec = vec;
                oFinal = _AvoidTargets[i];
                fFinalDot = fDot;
            }
        }

        if (oFinal != null)
        {
            Vector3 vCross = Vector3.Cross(cForward, vFinalVec);
            float fTurnMag = Mathf.Sqrt(1.0f - fFinalDot * fFinalDot);
            if (vCross.y > 0.0f)
                fTurnMag = -fTurnMag;
            data.TempTurnForce = fTurnMag;

            float fTotalLen = data.ProbeLength + oFinal.draw_radius;
            float fRatio = fMinDist / fTotalLen;
            if (fRatio > 1.0f) fRatio = 1.0f;
            fRatio = 1.0f - fRatio;
            data.MoveForce = -fRatio;
            oFinal.ObtState = MapObstacle.State.COL;
            data.bCol = data.bMoving = true;
            return true;
        }
        data.bCol = false;
        return false;
    }
    #endregion << Obstacles check and avoid >>

    #region << Flee >>
    public static bool Flee(AIData data)
    {
        Vector3 vec = data.GetDirectionalVector();
        vec.y = 0.0f;
        float fDist = vec.magnitude;
        data.TempTurnForce = 0.0f;
        if (data.ProbeLength < fDist)
        {
            if (data.Speed > minSpeed) data.MoveForce = forceMin;
            data.bMoving = true;
            return false;
        }

        Vector3 vf = data.GetForward(), vr = data.GetRight();
        data.CurrentVector = vf;
        vec.Normalize();
        float fDotF = Vector3.Dot(vf, vec);
        if (fDotF < forceCheckGapDown)
        {
            fDotF = forceMin;
            data.CurrentVector = -vec;
            //data.SetForward(-vec);
            data.TempTurnForce = data.Rot = 0.0f;
        }
        else
        {
            if (fDotF > forceMax) fDotF = forceMax;
            float fDotR = Vector3.Dot(vr, vec);
            if (fDotF > 0.0f) fDotR = fDotR > 0.0f ? forceMax : forceMin;
            //Debug.Log($"Flee : {fDotR}");
            data.TempTurnForce = -fDotR;
        }

        data.MoveForce = -fDotF;
        data.bMoving = true;
        return true;
    }

    public static bool FleeAnyDir(AIData data)
    {
        Vector3 vFleeDir = data.GetDirectionalVector();
        float fDist = vFleeDir.magnitude;
        if (fDist < faceDirt)
        {
            vFleeDir = new Vector3(Random.Range(randomMin, randomMax), 0.0f, Random.Range(randomMin, randomMax));
        }
        else if (fDist > data.ProbeLength) //out of detect range
        {
            data.TempTurnForce = 0.0f;
            data.MoveForce = offsetForce;
            data.bMoving = true;
            return false;
        }
        data.TargetDir = -vFleeDir;
        return SeekDirection(data);
    }
    #endregion << Flee >>

    #region << Seek >>
    public static bool SeekAngle(AIData data)
    {
        Vector3 vec = data.GetDirectionalVector();
        vec.y = 0.0f;
        if (Arrival(data, vec)) return false;

        Vector3 vf = data.GetForward(), vr = data.GetRight();
        data.CurrentVector = vf;
        vec.Normalize();
        float fDotF = Vector3.Dot(vf, vec).Clamp(selfNomolize), fDotR = Vector3.Dot(vr, vec);
        data.CurrentDeltaRot = Mathf.Acos(fDotF) * Mathf.Rad2Deg;
        data.TempTurnForce = fDotR * moveForceTimes;
        return true;
    }

    public static bool SeekDirection(AIData data)
    {
        Vector3 cPos = data.GetPosition(), vec = data.TargetDir;
        Debug.DrawLine(cPos, cPos + vec, Color.yellow);
        vec.y = 0.0f;
        Vector3 vf = data.GetForward(), vr = data.GetRight();
        data.CurrentVector = vf;
        vec.Normalize();
        float fDotF = Vector3.Dot(vf, vec);
        // float distThreshold = data.DecDistance;
        // float distRatio = (selfNomolize - Mathf.Clamp01(vec.magnitude / distThreshold));

        if (fDotF > forceCheckGapUp)
        {
            fDotF = forceMax;
            data.CurrentVector = vec;
            data.TempTurnForce = 0.0f;
            data.Rot = 0.0f;
        }
        else
        {
            float fDotR = Vector3.Dot(vr, vec), fSign;
            //dot positive => same side, negative => opposite side
            if (fDotR > 0.0f) fSign = selfNomolize;
            else
            {
                fSign = selfMinesNomolize;
                fDotR = -fDotR;
            }
            if (fDotF < 0.0f) fDotR = selfNomolize;

            data.TempTurnForce = fDotR * fSign;
        }
        data.MoveForce = fDotF * moveBigForceTimes;
        data.bMoving = true;
        return true;
    }

    public static bool Seek(AIData data)
    {
        Vector3 vec = data.GetDirectionalVector();
        vec.y = 0.0f;
        if (Arrival(data, vec)) return false;

        float fDist = vec.magnitude;
        Vector3 vf = data.GetForward(), vr = data.GetRight();
        data.CurrentVector = vf;
        vec.Normalize();
        float fDotF = Vector3.Dot(vf, vec), distThreshold = data.DecDistance, distRatio = (selfNomolize - Mathf.Clamp01(fDist / distThreshold));

        if (fDotF > forceCheckGapUp)
        {
            fDotF = selfNomolize;
            data.CurrentVector = vec;
            data.TempTurnForce = 0.0f;
            data.Rot = 0.0f;
        }
        else
        {
            float fDotR = Vector3.Dot(vr, vec), fSign;
            //dot positive => same side, negative => opposite side
            if (fDotR > 0.0f) fSign = selfNomolize;
            else
            {
                fSign = selfMinesNomolize;
                fDotR = -fDotR;
            }
            if (fDotF < 0.0f) fDotR = selfNomolize;
            if (fDist < distThreshold)
            {
                // fDotR *= (forceMaxDouble - Mathf.Clamp01(fDist / distThresholdDefault));
                fDotR += distRatio * moveForceTimes;
                Vector3 vSlop = vec - vf;
                float vSDist = vSlop.magnitude;
                if (fDotR > vSDist) fDotR = vSDist;
            }
            data.TempTurnForce = fDotR * fSign;
        }

        data.MoveForce = fDist < distThreshold
            ? data.Speed > forceMaxDouble ? -fDotF * distRatio * moveGrateForceTimes : fDotF * moveBigForceTimes
            : fDotF * moveBigForceTimes;
        //Debug.Log(data.MoveForce);
        data.bMoving = true;
        return true;
    }
    #endregion << Seek >>
}
