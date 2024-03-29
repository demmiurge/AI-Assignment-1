using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteeringContext))]
public abstract class SteeringBehaviour : MonoBehaviour
{
    public enum RotationalPolicy { LWYG, LWYGI, FT, FTI, NONE};

    public RotationalPolicy m_RotationalPolicy = RotationalPolicy.NONE;

    public SteeringContext m_Context { get => GetComponent<SteeringContext>(); }

    new Rigidbody2D m_Rigidbody;

    private bool m_HasRigidbody;

    protected static GameObject SURROGATE_TARGET = null;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
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
        Vector3 acceleration = GetLinearAcceleration();
        // zero acceleration implies stop...

        if (acceleration.Equals(Vector3.zero))
        {
            m_Context.m_Velocity = Vector3.zero;
            return;
        }

        float dt = Time.fixedDeltaTime;
        Vector3 velIncrement = acceleration * dt;
        m_Context.m_Velocity += velIncrement;

        if (m_Context.m_ClipVelocity)
            if (m_Context.m_Velocity.magnitude > m_Context.m_MaxSpeed)
                m_Context.m_Velocity = m_Context.m_Velocity.normalized * m_Context.m_MaxSpeed;

        transform.position += m_Context.m_Velocity * dt + 0.5f * acceleration * dt * dt;
    }

    private void ApplyAngularAccelerationWithoutRigidbody()
    {
        float acceleration = GetAngularAcceleration();
        // zero acceleration implies stop

        if (acceleration == 0)
        {
            m_Context.m_AngularSpeed = 0;
            return;
        }

        float dt = Time.fixedDeltaTime;
        m_Context.m_AngularSpeed += acceleration * dt;

        if (m_Context.m_ClipAngularSpeed)
            if (Mathf.Abs(m_Context.m_AngularSpeed) > m_Context.m_MaxAngularSpeed)
                m_Context.m_AngularSpeed = m_Context.m_MaxAngularSpeed *
                                           Mathf.Sign(m_Context.m_AngularSpeed);

        float orientation = transform.rotation.eulerAngles.z +
                            m_Context.m_AngularSpeed * dt + 0.5f * acceleration * dt * dt;
        transform.rotation = Quaternion.Euler(0, 0, orientation);
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
            m_Rigidbody.AddForce(acceleration * m_Rigidbody.mass, ForceMode2D.Force);

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
            m_Rigidbody.angularVelocity = 0;
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
            m_Rigidbody.angularVelocity = m_Context.m_AngularSpeed;
        }
        else
        {
            m_Rigidbody.AddTorque(acceleration * m_Rigidbody.inertia * Mathf.Deg2Rad, ForceMode2D.Force);

            if (m_Context.m_ClipAngularSpeed)
                if (Mathf.Abs(m_Rigidbody.angularVelocity) > m_Context.m_MaxAngularSpeed)
                    m_Rigidbody.angularVelocity = m_Context.m_MaxAngularSpeed * Mathf.Sign(m_Rigidbody.angularVelocity);

            // cache
            m_Context.m_AngularSpeed = m_Rigidbody.angularVelocity;

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
            SURROGATE_TARGET.transform.rotation = Quaternion.Euler(0, 0, Utils.VectorToOrientation(m_Context.m_Velocity));
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
            m_Rigidbody.rotation = Utils.VectorToOrientation(m_Context.m_Velocity);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, Utils.VectorToOrientation(m_Context.m_Velocity));
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
        float orientation = Utils.VectorToOrientation(directionToTarget);

        if (m_HasRigidbody)
        {
            m_Rigidbody.rotation = orientation;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, orientation);
        }

    }


}

