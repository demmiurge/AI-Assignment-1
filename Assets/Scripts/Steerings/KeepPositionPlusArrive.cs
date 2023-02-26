using UnityEngine;

public class KeepPositionPlusArrive : SteeringBehaviour
{
    public GameObject attractor;
    public GameObject target;
    public float distance;
    public float angle;

    public override Vector3 GetLinearAcceleration()
    {
        return KeepPositionPlusArrive.GetLinearAcceleration(m_Context, attractor, target, distance, angle);
    }


    public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject attractor, GameObject target, float distance, float angle)
    {
        Vector3 arriveAcc = Arrive.GetLinearAcceleration(me, attractor);

        if (arriveAcc.Equals(Vector3.zero))
            return KeepPosition.GetLinearAcceleration(me, target, distance, angle);
        else
            return arriveAcc;
    }

}
