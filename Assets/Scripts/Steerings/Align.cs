using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBehaviour
{
	public GameObject target;

	// there's no getter for target since rotational behaviours do not 
	// apply facing policies (facing policies make no sense for rotational
	// behaviours)

	public override float GetAngularAcceleration()
	{
		return Align.GetAngularAcceleration(m_Context, target);
	}

	public static float GetAngularAcceleration(SteeringContext me, GameObject target)
	{

		float result;
		float requiredAngularSpeed;
		float targetOrientation = target.transform.eulerAngles.y; // BEWARE...

		float requiredRotation = targetOrientation - me.transform.eulerAngles.y;  // how many degs do we have to rotate?

		if (requiredRotation < 0)
			requiredRotation = 360 + requiredRotation; // map to positive angles

		if (requiredRotation > 180)
			requiredRotation = (360 - requiredRotation); // don't rotate more than 180 degs. just reverse rotation sense

		// when here, required rotation is in [-180, +180]

		float rotationSize = Mathf.Abs(requiredRotation);

		if (rotationSize <= me.m_CloseEnoughAngle) // if we're "there", no steering needed
			return 0f;


		if (rotationSize > me.m_SlowDownAngle)
			requiredAngularSpeed = me.m_MaxAngularSpeed;
		else
			requiredAngularSpeed = me.m_MaxAngularSpeed * (rotationSize / me.m_SlowDownAngle);

		// restablish sign
		requiredAngularSpeed = requiredAngularSpeed * Mathf.Sign(requiredRotation);

		// compute acceleration
		result = (requiredAngularSpeed - me.m_AngularSpeed) / me.m_TimeToDesiredAngularSpeed;

		// clip acceleration if necessary
		if (Mathf.Abs(result) > me.m_MaxAngularAcceleration)
			result = me.m_MaxAngularAcceleration * Mathf.Sign(result);

		return result;
	}
}

