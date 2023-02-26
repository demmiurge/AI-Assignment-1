using UnityEngine;

public class KeepPositionPlusArrive : SteeringBehaviour
{
    public GameObject attractor;
    public GameObject target;
    public float distance;
    public float angle;
    public float ArriveWeight;

    public override Vector3 GetLinearAcceleration()
    {
        return KeepPositionPlusArrive.GetLinearAcceleration(m_Context, attractor, target, distance, angle, ArriveWeight);
    }


    public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject attractor, GameObject target, float distance, float angle, float weight)
    {
        Vector3 arriveAcc = Arrive.GetLinearAcceleration(me, attractor);
        Vector3 keepPos = KeepPosition.GetLinearAcceleration(me, target, distance, angle);

        if (keepPos.Equals(Vector3.zero))
            return arriveAcc;
        else
            return arriveAcc * weight + keepPos * (1 - weight);
    }

}
