using UnityEngine;

public class WanderAround : SteeringBehaviour
{
    public GameObject attractor;

    public override Vector3 GetLinearAcceleration()
    {
        return WanderAround.GetLinearAcceleration(m_Context, attractor);
    }

    public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject attractor)
    {
        Vector3 seekAcc = Seek.GetLinearAcceleration(me, attractor);
        Vector3 wanderAcc = Wander.GetLinearAcceleration(me);

        return seekAcc * me.m_SeekWeight + wanderAcc * (1 - me.m_SeekWeight);
    }
}

