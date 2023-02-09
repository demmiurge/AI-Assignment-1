using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteeringContext))]
public abstract class SteeringBehaviour : MonoBehaviour
{
    public enum RotationalPolicy { LWYG, LWYGI, FT, FTI, NONE};

    public RotationalPolicy m_RotationalPolicy = RotationalPolicy.NONE;

    public SteeringContext m_Context { get => GetComponent<SteeringContext>(); }

    new Rigidbody m_Rigidbody;

    private bool m_HasRigidbody;

    protected static GameObject SURROGATE_TARGET = null;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_HasRigidbody = m_Rigidbody != null;

        if(SURROGATE_TARGET == null)
        {
            SURROGATE_TARGET = new GameObject("Surrogate Target");
            SURROGATE_TARGET.AddComponent<SteeringContext>();
        }
    }

    void FixedUpdate()
    {
        if (m_HasRigidbody)
        {
            ApplyLinearAccelerationWithRigidbody();
            ApplyAngularAccelerationWithRigidBody();
        }
        else
        {
            ApplyLinearAccelerationWithoutRigidBody();
            ApplyAngularAccelerationWithoutRigidbody();
        }
    }


    private void ApplyLinearAccelerationWithoutRigidBody()
    {
        Vector3 l_Acceleration = GetLinearAcceleration();

        if(l_Acceleration.Equals(Vector3.zero))
        {
            m_Context.m_Velocity = Vector3.zero;
            return;
        }

        float dt = Time.fixedDeltaTime;
        Vector3 l_VelIncrement = l_Acceleration * dt;
        m_Context.m_Velocity += l_VelIncrement;

        if(m_Context.m_ClipVelocity)
        {
            if(m_Context.m_Velocity.magnitude > m_Context.m_MaxSpeed)
            {
                m_Context.m_Velocity = m_Context.m_Velocity.normalized * m_Context.m_MaxSpeed;
            }
        }

        transform.position += m_Context.m_Velocity * dt + 0.5f * l_Acceleration * dt * dt;
    }

    private void ApplyAngularAccelerationWithoutRigidbody()
    {
        float l_acceleration = GetAngularAcceleration();
        // zero acceleration implies stop

        if (l_acceleration == 0)
        {
            m_Context.m_AngularSpeed = 0;
            return;
        }

        float dt = Time.fixedDeltaTime;
        m_Context.m_AngularSpeed += l_acceleration * dt;

        if (m_Context.m_ClipAngularSpeed)
            if (Mathf.Abs(m_Context.m_AngularSpeed) > m_Context.m_MaxAngularSpeed)
                m_Context.m_AngularSpeed = m_Context.m_MaxAngularSpeed *
                                             Mathf.Sign(m_Context.m_AngularSpeed);

        float orientation = transform.rotation.eulerAngles.y + m_Context.m_AngularSpeed * dt + 0.5f * l_acceleration * dt * dt;
        transform.rotation = Quaternion.Euler(0, orientation, 0);
    }

    private void ApplyLinearAccelerationWithRigidbody()
    {
        Vector3 acceleration = GetLinearAcceleration();

        // zero acceleration implies stop...
        if (acceleration.Equals(Vector3.zero))
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Context.m_Velocity = Vector3.zero;
            return;
        }

        if (m_Rigidbody.isKinematic)
        {
            // compute velocity increcment
            Vector3 velIncrement = acceleration * Time.fixedDeltaTime;

            // compute new velocity and clip it if necessary
            m_Context.m_Velocity += velIncrement;
            if (m_Context.m_ClipVelocity)
                if (m_Context.m_Velocity.magnitude > m_Context.m_MaxSpeed)
                    m_Context.m_Velocity = m_Context.m_Velocity.normalized * m_Context.m_MaxSpeed;

            m_Rigidbody.velocity = m_Context.m_Velocity;
            // the previous line is equivalent to
            // rigidbody.MovePosition(transform.position + context.velocity*Time.fixedDeltaTime);
            // BUT MovePosition does not seem to update rigidbody.velocity accordingly
        }

        else  // if rigidbody is not kinematic then it is dynamic ("controlled" by forces)
        {
            m_Rigidbody.AddForce(acceleration * m_Rigidbody.mass, ForceMode.Force);

            if (m_Context.m_ClipVelocity)
            {
                float speed = m_Rigidbody.velocity.magnitude;
                if (speed > m_Context.m_MaxSpeed) m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * m_Context.m_MaxSpeed;
            }

            // cache velocity in the context
            m_Context.m_Velocity = m_Rigidbody.velocity; // "caching" of velocity required since behaviours like arrive need to 
                                                   // know it without having to access the rigidbody
                                                   // BUT: this velocity is the one previous to applying the force...
        }
    }

    private void ApplyAngularAccelerationWithRigidBody()
    {
        float acceleration = GetAngularAcceleration();
        if (acceleration == 0)
        {
            m_Rigidbody.angularVelocity = new Vector3(0,0,0);
            m_Context.m_AngularSpeed = 0;
        }

        if (m_Rigidbody.isKinematic)
        {
            float speedIncrement = acceleration * Time.fixedDeltaTime;
            // compute new angular speed and clip if necessary
            m_Context.m_AngularSpeed += speedIncrement;
            if (m_Context.m_ClipAngularSpeed)
                if (Mathf.Abs(m_Context.m_AngularSpeed) > m_Context.m_MaxAngularSpeed)
                    m_Context.m_AngularSpeed = m_Context.m_MaxAngularSpeed * Mathf.Sign(m_Context.m_AngularSpeed);
            // apply to rigidbody
            m_Rigidbody.angularVelocity = new Vector3(0, m_Context.m_AngularSpeed, 0);
        }
        else
        {
            m_Rigidbody.AddTorque(acceleration * m_Rigidbody.inertiaTensor/4 * Mathf.Deg2Rad, ForceMode.Force);

            if (m_Context.m_ClipAngularSpeed)
                if (Mathf.Abs((m_Rigidbody.angularVelocity).magnitude) > m_Context.m_MaxAngularSpeed)
                    m_Rigidbody.angularVelocity = new Vector3(0, m_Context.m_MaxAngularSpeed * Mathf.Sign(m_Rigidbody.angularVelocity.magnitude), 0);

            // cache
            m_Context.m_AngularSpeed = m_Rigidbody.angularVelocity.y;

        }
    }

    public virtual Vector3 GetLinearAcceleration()
    {
        return Vector3.zero;
    }

    public virtual float GetAngularAcceleration()
    {
        return ApplyRotationalPolicy(m_RotationalPolicy);
    }

    public virtual GameObject GetTarget()
    {
        Debug.LogError("Invoking non-redefined version of " +
                       "SteeringBehaviour.GetTarget(). " +
                       "Subclasses (linear) having a target should " +
                       "redefine this method if FT o FTI rotational policies" +
                       " make sense for them");

        return null;
    }

    private float ApplyRotationalPolicy(RotationalPolicy policy)
    {
        float angAcceleration = 0;
        switch (policy)
        {
            case RotationalPolicy.LWYG:
                angAcceleration = PolicyLWYG();
                break;
            case RotationalPolicy.LWYGI:
                PolicyLWYGI();
                break;
            case RotationalPolicy.FT:
                angAcceleration = PolicyFT(GetTarget());
                break;
            case RotationalPolicy.FTI:
                PolicyFTI(GetTarget());
                break;
        }
        return angAcceleration;
    }

    // Rotational (FACING) policies

    private float PolicyLWYG()
    {
        float angAcceleration;
        if (m_Context.m_Velocity.magnitude < 0.001f) angAcceleration = 0;
        // if (context.velocity.Equals(Vector3.zero)) angAcceleration = 0;
        else
        {
            SURROGATE_TARGET.transform.rotation = Quaternion.Euler(0, Utils.VectorToOrientation(m_Context.m_Velocity), 0);
            angAcceleration = Align.GetAngularAcceleration(m_Context, SURROGATE_TARGET);
        }
        return angAcceleration;
    }

    private void PolicyLWYGI()
    {
        if (m_Context.m_Velocity.magnitude < 0.001f) return;
        // this policy does not generate an acceleration. It changes the orientation of the go directly!!!
        if (m_HasRigidbody)
        {
            m_Rigidbody.rotation = Quaternion.Euler(0, Utils.VectorToOrientation(m_Context.m_Velocity), 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, Utils.VectorToOrientation(m_Context.m_Velocity), 0);
        }
    }

    private float PolicyFT(GameObject target)
    {
        float angAcceleration = 0;
        if (target == null)
            Debug.LogError("FT rotational policy applied with null target");
        else
            angAcceleration = Face.GetAngularAcceleration(m_Context, target);

        return angAcceleration;
    }

    private void PolicyFTI(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError("FT rotational policy applied with null target");
            return;
        }

        Vector3 directionToTarget = target.transform.position - transform.position;
        Quaternion orientation = Quaternion.Euler(0, Utils.VectorToOrientation(directionToTarget), 0);

        if (m_HasRigidbody)
        {
            m_Rigidbody.rotation = orientation;
        }
        else
        {
            transform.rotation = orientation;
        }

    }


}

