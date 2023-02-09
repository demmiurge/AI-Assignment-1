
using UnityEngine;



public class Wander : SteeringBehaviour
{
    public override Vector3 GetLinearAcceleration()
    {
        return Wander.GetLinearAcceleration(m_Context);
    }

    public static Vector3 GetLinearAcceleration(SteeringContext me)
    {
        // change target orientation (change location of surrogate target on unit circle)
        me.m_WanderTargetOrientation += me.m_WanderRate * Utils.binomial();

        // place surrogate target on circle of wanderRadius
        SURROGATE_TARGET.transform.position = Utils.OrientationToVector(me.m_WanderTargetOrientation) * me.m_WanderRadius;

        // place circle  "in front"
        // in front of me or in front of my velocity?
        // in fron of my velocity, definitely. Othewise, behaviour with policies different from lwyg is questionable
        if (me.m_Velocity.magnitude > 0.01f)
            SURROGATE_TARGET.transform.position +=
                //me.transform.position + Utils.OrientationToVector(me.transform.eulerAngles.z) * me.wanderOffset;
                me.transform.position + new Vector3(me.m_Velocity.x * me.m_WanderOffset, me.m_Velocity.y * me.m_WanderOffset, me.m_Velocity.z * me.m_WanderOffset);
        else
            SURROGATE_TARGET.transform.position += me.transform.position + Utils.OrientationToVector(me.transform.eulerAngles.y) * me.m_WanderOffset;

        // show some gizmos before returning
        if (me.m_ShowWanderGizmos)
        {
            Debug.DrawLine(me.transform.position,
                       //me.transform.position + Utils.OrientationToVector(me.transform.eulerAngles.z) * me.wanderOffset,
                       me.transform.position + me.m_Velocity.normalized * me.m_WanderOffset,
                       Color.black);

            DebugExtension.DebugCircle(me.transform.position +
                                                             //Utils.OrientationToVector(me.transform.eulerAngles.z) * me.wanderOffset,
                                                             me.m_Velocity.normalized * me.m_WanderOffset,
                                       new Vector3(0, 0, 1),
                                       Color.red,
                                       me.m_WanderRadius);
            DebugExtension.DebugPoint(SURROGATE_TARGET.transform.position,
                                  Color.black,
                                  5f);
        }


        // Seek the surrogate target
        return Seek.GetLinearAcceleration(me, SURROGATE_TARGET);
    }
}

