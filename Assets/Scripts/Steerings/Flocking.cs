using System.Collections.Generic;
using UnityEngine;


public class Flocking : SteeringBehaviour
{
    public override Vector3 GetLinearAcceleration()
    {
        return Flocking.GetLinearAcceleration(m_Context);
    }

    public static Vector3 GetLinearAcceleration(SteeringContext me)
    {
        // get all potential "mates" (the other boids) 
        //ICollection<GameObject> boids = me.groupManager.members;
        GameObject[] boids = GameObject.FindGameObjectsWithTag(me.m_IdTag);


        Vector3 averageVelocity = Vector3.zero;
        int count = 0;

        // iterate to find average velocity
        foreach (GameObject boid in boids)
        {
            // do not take yourself into account
            if (boid == me.gameObject) continue;

            // velocity of mate required. Let's get if from its Steering context
            SteeringContext boidContext = boid.GetComponent<SteeringContext>();

            // disregard distant boids (what is the meaning of distant?)
            // (I've decided to define distant as "not contributing to cohesion")
            if ((boid.transform.position - me.transform.position).magnitude > me.m_CohesionThreshold) continue;

            // also disregard boids outside the cone of vision, if vision applies
            if (me.m_ApplyVision)
                if (!Utils.InCone(me.gameObject, boid, me.m_ConeOfVisionAngle)) continue;

            averageVelocity += boidContext.m_Velocity;
            count++;
        }


        if (count > 0)
            averageVelocity /= count;
        else
        {
            // if no boid is close, there's nowhere to steer for
            // (hence wander, if allowed...)
            if (me.m_AddWanderIfZero) return Wander.GetLinearAcceleration(me);
            else return Vector3.zero;
        }

        // now let's compute all the ingredients
        SURROGATE_TARGET.GetComponent<SteeringContext>().m_Velocity = averageVelocity;
        Vector3 velocityMatching = VelocityMatching.GetLinearAcceleration(me, SURROGATE_TARGET);
        Vector3 separation = LinearRepulsion.GetLinearAcceleration(me);
        Vector3 cohesion = Cohesion.GetLinearAcceleration(me);

        separation = separation.normalized * me.m_MaxAcceleration;
        velocityMatching = velocityMatching.normalized * me.m_MaxAcceleration;
        // cohesion is based on seek and, when it returns an acceleration that acceleration is of max. magnitude
        // but when no mates are found, cohesion returns Vector3.zero

        // and the actual weights to use
        float vmW = me.m_AlignmentWeight;
        float coW = me.m_CohesionWeight;
        float seW = me.m_RepulsionWeight;


        // notice that if this point is reached count>0. This means that there's at least one mate boid close enough
        // hence cohesion applies and alignment applies. Only separation may not apply
        if (separation.Equals(Vector3.zero))
        { // give the separation weight to the other two... 
            vmW += seW * (vmW / (vmW + coW));
            coW += seW * (coW / (vmW + coW));
            seW = 0;
        }

        Vector3 flockingAcceleration = velocityMatching * vmW + cohesion * coW + separation * seW;

        if (me.m_AddWanderIfZero && flockingAcceleration.Equals(Vector3.zero))
        {
            return Wander.GetLinearAcceleration(me);
        }
        else return flockingAcceleration;

    }
}

