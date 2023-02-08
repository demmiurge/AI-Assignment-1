using UnityEngine;


public class FlockingAround : SteeringBehaviour
{

    public GameObject attractor;

    public override Vector3 GetLinearAcceleration()
    {
        return FlockingAround.GetLinearAcceleration(m_Context, attractor);
    }

    public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject attractor)
    {
        Vector3 seekAcc = Seek.GetLinearAcceleration(me, attractor);
        Vector3 flockingAcc = Flocking.GetLinearAcceleration(me);

        return seekAcc * me.m_SeekWeight + flockingAcc * (1 - me.m_SeekWeight);
    }
}

