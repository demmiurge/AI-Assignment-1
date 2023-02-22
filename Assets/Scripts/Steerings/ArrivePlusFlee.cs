using UnityEngine;

public class ArrivePlusFlee : SteeringBehaviour
{

    public GameObject target;
    public GameObject peril;
    private float arriveWeight = 0.5f;

    public override GameObject GetTarget()
    {
        return target;
    }

    public GameObject GetPeril()
    {
        return peril;
    }

    public override Vector3 GetLinearAcceleration()
    {
        return ArrivePlusFlee.GetLinearAcceleration(m_Context, target, peril, arriveWeight);
    }

        
    public static Vector3 GetLinearAcceleration (SteeringContext me, GameObject target, GameObject peril, float arriveWeight)
    {

        Vector3 arrive = ArrivePlusOA.GetLinearAcceleration(me, target);
        Vector3 fleePlusOA = FleePlusOA.GetLinearAcceleration(me, peril);
        if (fleePlusOA == Vector3.zero)
        {
            return arrive;
        }
        else
        {
            return arrive * arriveWeight + fleePlusOA * (1f - arriveWeight);
        }
    }

}