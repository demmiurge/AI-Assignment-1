using UnityEngine;

public class KeepPosition : SteeringBehaviour
{
    public GameObject target;
    public float requiredDistance;
    public float requiredAngle;

    public override Vector3 GetLinearAcceleration()
    {
        return KeepPosition.GetLinearAcceleration(m_Context, target, requiredDistance, requiredAngle); 
    }


    public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject target, float distance, float angle)
    {
        float desiredAngle = target.transform.eulerAngles.magnitude + angle;
        Vector3 desiredDirectionFromTarget = Utils.OrientationToVector(desiredAngle).normalized;
        Vector3 desiredPosition = target.transform.position + (desiredDirectionFromTarget * distance);

        SURROGATE_TARGET.transform.position = desiredPosition;

        return Arrive.GetLinearAcceleration(me, SURROGATE_TARGET);
    }

}
