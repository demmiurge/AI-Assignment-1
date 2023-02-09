
using UnityEngine;


public class GoWhereYouLook : SteeringBehaviour
{
    public override Vector3 GetLinearAcceleration()
    {
        return GoWhereYouLook.GetLinearAcceleration(m_Context);
    }

    public static Vector3 GetLinearAcceleration(SteeringContext me)
    {
        Vector3 myDirection = Utils.OrientationToVector(me.transform.eulerAngles.y);
        Vector3 inFrontOfme = new Vector3(me.transform.position.x + myDirection.x, me.transform.position.y + myDirection.y, me.transform.position.z);
        //Vector3 inFrontOfme = me.transform.position + myDirection;

         SURROGATE_TARGET.transform.position = inFrontOfme;
        return Seek.GetLinearAcceleration(me, SURROGATE_TARGET);
    }
}

