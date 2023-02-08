
using UnityEngine;


public class VelocityMatching : SteeringBehaviour
{

    public GameObject target;

    public override GameObject GetTarget()
    {
        return target;
    }

    public override Vector3 GetLinearAcceleration()
    {
        return VelocityMatching.GetLinearAcceleration(m_Context, target);
    }

    public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject target)
    {
        // velocity of target required. Let's get if from its Steering context
        SteeringContext targetContext = target.GetComponent<SteeringContext>();
        if (targetContext == null)
        {
            Debug.LogWarning("Velocity Matching invoked with a target " +
                              "that has no context attached. Zero acceleration returned");
            return Vector3.zero;
        }

        Vector3 requiredAcceleration = (targetContext.m_Velocity - me.m_Velocity)
                                       / me.m_TimeToDesiredSpeed;

        if (requiredAcceleration.magnitude > me.m_MaxAcceleration)
            requiredAcceleration = requiredAcceleration.normalized * me.m_MaxAcceleration;

        return requiredAcceleration;
    }
}

