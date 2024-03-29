using UnityEngine;


public class Pursue : SteeringBehaviour
{
    public GameObject target;

    public override GameObject GetTarget()
    {
        return target;
    }

    public override Vector3 GetLinearAcceleration()
    {
        return Pursue.GetLinearAcceleration(m_Context, target);
    }

    public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject target)
    {
        // velocity of target required. Let's get if from its Steering context
        SteeringContext targetContext = target.GetComponent<SteeringContext>();
        if (targetContext == null)
        {
            Debug.LogWarning("Pursue invoked with a target that has no context attached. Resorting to Seek");
            return Seek.GetLinearAcceleration(me, target);
        }

        Vector3 directionToTarget = target.transform.position - me.transform.position;
        float distanceToTarget = directionToTarget.magnitude;
        float currentSpeed = me.m_Velocity.magnitude;

        // determine the time it will take to reach the target
        float predictedTimeToTarget = distanceToTarget / currentSpeed; // time = distance/speed
        if (predictedTimeToTarget > me.m_MaxPredictionTime)
            predictedTimeToTarget = me.m_MaxPredictionTime;

        // now determine future (at predicted time) location of target
        Vector3 futurePositionOfTarget = target.transform.position + targetContext.m_Velocity * predictedTimeToTarget;

        if (me.m_ShowFutureTargetGizmos)
            DebugExtension.DebugPoint(futurePositionOfTarget, Color.red, 2f);

        // Place surrogate target at future location
        SURROGATE_TARGET.transform.position = futurePositionOfTarget;

        // delegate to seek
        // could also delegate to Arrive if overshooting is an issue...
        return Seek.GetLinearAcceleration(me, SURROGATE_TARGET);
    }
}
