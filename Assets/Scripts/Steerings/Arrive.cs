using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Arrive : SteeringBehaviour
{

    public GameObject target;

    public override GameObject GetTarget()
    {
        return target;
    }

    public override Vector3 GetLinearAcceleration()
    {
        return Arrive.GetLinearAcceleration(m_Context, target);
    }

    public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject target)
    {
        Vector3 directionToTarget = target.transform.position - me.transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget < me.m_CloseEnoughRadius) return Vector3.zero;

        if (distanceToTarget > me.m_SlowDownRadius) return Seek.GetLinearAcceleration(me, target);

        float desiredSpeed = me.m_MaxSpeed * (distanceToTarget / me.m_SlowDownRadius);
        Vector3 desiredVelocity = directionToTarget.normalized * desiredSpeed;
        Vector3 requiredAcceleration = (desiredVelocity - me.m_Velocity) / me.m_TimeToDesiredSpeed;

        if (requiredAcceleration.magnitude > me.m_MaxAcceleration)
            requiredAcceleration = requiredAcceleration.normalized * me.m_MaxAcceleration;

        return requiredAcceleration;
    }


    // the following method exists for retrocompatibility with the pathfollowing steering.
    // only PathFollowing should invoke it.
    // It gets the radiuses from its parameters instead of getting them from the context
    public static Vector3 GetLinearAccelerationForPathfinding(SteeringContext me, GameObject target,
                                                              float closeEnoughRadius, float slowdownRadius)
    {
        Vector3 directionToTarget = target.transform.position - me.transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget < closeEnoughRadius) return Vector3.zero;

        if (distanceToTarget > slowdownRadius) return Seek.GetLinearAcceleration(me, target);

        float desiredSpeed = me.m_MaxSpeed * (distanceToTarget / slowdownRadius);
        Vector3 desiredVelocity = directionToTarget.normalized * desiredSpeed;
        Vector3 requiredAcceleration = (desiredVelocity - me.m_Velocity) / me.m_TimeToDesiredSpeed;

        if (requiredAcceleration.magnitude > me.m_MaxAcceleration)
            requiredAcceleration = requiredAcceleration.normalized * me.m_MaxAcceleration;

        return requiredAcceleration;
    }

}

