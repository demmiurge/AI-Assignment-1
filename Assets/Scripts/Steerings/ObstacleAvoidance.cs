
using UnityEngine;


public class ObstacleAvoidance : SteeringBehaviour
{
    private void Start()
    {
        Physics2D.IgnoreLayerCollision(7, 8);
        Physics2D.IgnoreLayerCollision(7, 6);
    }

    public override Vector3 GetLinearAcceleration()
    {
        return ObstacleAvoidance.GetLinearAcceleration(m_Context);
    }


    // Experimental version (by ESN) with perseverance [INERTIA] 
    // To revert to the old "canonical" version, just return Detection2D(me)
    // use with small perseverance times
    public static Vector3 GetLinearAcceleration(SteeringContext me)
    {
        Vector3 acc = Detection2D(me); // detect and get avoidance acceleration
                                       // do not persevere when an obstacle has been detected
        if (!acc.Equals(Vector3.zero))
        {
            me.m_AvoidanceAcceleration = acc;  // cache avoidance acc. for further use

            // get ready to persevere at next frame (if needed)
            me.m_PerseveranceElapsed = 0f;
            me.m_Persevering = true;

            return acc;
        }

        // if this point is reached, no obstacle detected
        // should we persevere?
        if (me.m_Persevering)
        {
            // if this point is reached recent collision still "remembered" (active)
            me.m_PerseveranceElapsed += Time.fixedDeltaTime;
            if (me.m_PerseveranceElapsed < me.m_PerseveranceTime)
                return me.m_AvoidanceAcceleration;  // return cached acceleration
            else
                me.m_Persevering = false;
        }

        // if this point is reached perseverance was off or just faded (run out of time)
        // obstacle avoidance has nothing to do. Return zero

        // before returning zero (no more obstacle avoidance) reposition 
        // wanderTarget in front of the agent
        // (improves wander+OA as it avoids that the agent tries to reach the same target 
        // that put it in a collision trajectory)
        if (me.m_Velocity.sqrMagnitude > 0)
            me.m_WanderTargetOrientation = Utils.VectorToOrientation(me.m_Velocity);

        return Vector3.zero;
    }

    // this is where the avoidance algorithm is implemented
    private static Vector3 Detection2D(SteeringContext me)
    {
        // only works for 2D environments
        // avoids colliding against a static not necessarily spherical object

        Vector2 mainDirection;
        Vector2 whisker1Direction, whisker2Direction, whisker3Direction;
        RaycastHit2D hit;
        Collider2D collider;
        bool hasActiveCollider;

        // if we are not moving avoid obstacles just in front of us
        // if we are moving avoid obstacles in out trajectory
        if (me.m_Velocity.magnitude < 0.0001f)
            mainDirection = Utils.OrientationToVector(me.transform.eulerAngles.z);
        else
            mainDirection = me.m_Velocity.normalized;

        // disable own collider if any. 
        hasActiveCollider = false;
        collider = me.gameObject.GetComponent<Collider2D>();
        if (collider != null)
        {
            hasActiveCollider = collider.enabled;
            collider.enabled = false;
        }

        // compute whisker directions
        whisker1Direction = mainDirection;
        whisker2Direction = Utils.OrientationToVector(Utils.VectorToOrientation(mainDirection) + me.m_SecondaryWhiskerAngle);
        whisker3Direction = Utils.OrientationToVector(Utils.VectorToOrientation(mainDirection) - me.m_SecondaryWhiskerAngle);

        // show the rays
        if (me.m_ShowAvoidanceGizmos)
        {
            Debug.DrawRay(me.transform.position, whisker1Direction * me.m_LookAheadLength, Color.black);
            Debug.DrawRay(me.transform.position, whisker2Direction * me.m_LookAheadLength * me.m_SecondaryWhiskerRatio, Color.black);
            Debug.DrawRay(me.transform.position, whisker3Direction * me.m_LookAheadLength * me.m_SecondaryWhiskerRatio, Color.black);
        }

        // cast the rays, in order. First the main...
        hit = Physics2D.Raycast(me.transform.position, whisker1Direction, me.m_LookAheadLength);
        if (hit.collider != null && hit.transform.tag != "MainCamera")
        {
            if(hit.collider.tag == "AREATOPUTFOOD")
            {
                Physics2D.IgnoreLayerCollision(7, 8);
            }

            if (hit.collider.tag == "CAMERABLOCKOUT")
            {
                Physics2D.IgnoreLayerCollision(7, 6);
            }
            // obstacle found
            SURROGATE_TARGET.transform.position = hit.point + hit.normal * me.m_AvoidDistance;

            // me.collisions += "1"; Debug.Log(me.collisions);

            if (collider != null)
                collider.enabled = hasActiveCollider;

            if (me.m_ShowAvoidanceGizmos)
                Debug.DrawRay(me.transform.position, whisker1Direction * me.m_LookAheadLength, Color.red);

            return Seek.GetLinearAcceleration(me, SURROGATE_TARGET);
        }

        // when here, "main whisker" found nothing. Let's try with a secondary one...
        hit = Physics2D.Raycast(me.transform.position, whisker2Direction, me.m_LookAheadLength * me.m_SecondaryWhiskerRatio);
        if (hit.collider != null && hit.transform.tag != "MainCamera")
        {
            // obstacle found
            SURROGATE_TARGET.transform.position = hit.point + hit.normal * me.m_AvoidDistance;

            // me.collisions += "2"; Debug.Log(me.collisions);

            if (collider != null)
                collider.enabled = hasActiveCollider;

            if (me.m_ShowAvoidanceGizmos)
                Debug.DrawRay(me.transform.position, whisker2Direction * me.m_LookAheadLength * me.m_SecondaryWhiskerRatio, Color.red);

            return Seek.GetLinearAcceleration(me, SURROGATE_TARGET);
        }
        // and now with the other...
        hit = Physics2D.Raycast(me.transform.position, whisker3Direction, me.m_LookAheadLength * me.m_SecondaryWhiskerRatio);
        if (hit.collider != null && hit.transform.tag != "MainCamera")
        {
            // obstacle found
            SURROGATE_TARGET.transform.position = hit.point + hit.normal * me.m_AvoidDistance;

            // me.collisions += "3"; Debug.Log(me.collisions);

            if (collider != null)
                collider.enabled = hasActiveCollider;

            if (me.m_ShowAvoidanceGizmos)
                Debug.DrawRay(me.transform.position, whisker3Direction * me.m_LookAheadLength * me.m_SecondaryWhiskerRatio, Color.red);

            return Seek.GetLinearAcceleration(me, SURROGATE_TARGET);
        }

        // if this point is reached, no whisker collided, no obstacle detected
        // restore collider and return zero

        if (collider != null) collider.enabled = hasActiveCollider;
        return Vector3.zero;
    }
}


/*
        //  OLD Experimental perseverance [inertia] (by ESN) model included. May need some revamping
        //  in order to avoid some nasty side effects like going through corners during
        //  perseverance time. Use with small times 
        public static Vector3 GetLinearAcceleration(SteeringContext me)
        {
            if (me.persevering)
            {
                // recent collision still active
                me.perseveranceElapsed += Time.fixedDeltaTime;
                if (me.perseveranceElapsed < me.perseveranceTime)
                {
                    // during perseverance time return last acceleration
                    return me.avoidanceAcceleration;
                }
            }
            // if this point is reached, no perseverance

            Vector3 acc = Detection2D(me); // detect and get avoidance acceleration

            me.persevering = !acc.Equals(Vector3.zero);
            if (me.persevering)
            {
                me.perseveranceElapsed = 0f;
                me.avoidanceAcceleration = acc; // cache acceleration for further use
            }

            return acc;
        }
 */