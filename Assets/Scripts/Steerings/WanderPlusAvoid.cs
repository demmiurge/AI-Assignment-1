using UnityEngine;


public class WanderPlusAvoid : SteeringBehaviour
{
    public override Vector3 GetLinearAcceleration()
    {
        return WanderPlusAvoid.GetLinearAcceleration(m_Context);
    }

    public static Vector3 GetLinearAcceleration(SteeringContext me)
    {
        Vector3 avoidAcc = ObstacleAvoidance.GetLinearAcceleration(me);

        if (avoidAcc.Equals(Vector3.zero))
            return Wander.GetLinearAcceleration(me);
        else
            return avoidAcc;
    }

}
