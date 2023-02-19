using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraController : MonoBehaviour
{
    [Foldout("Body and Speed", styled = true)]
    [SerializeField]
    private Rigidbody2D m_Rigidbody2D;
    [SerializeField]
    private float m_Speed = 7.5f;
    [SerializeField]
    private float m_MaxSpeed = 5.0f;
    [SerializeField]
    private float m_Acceleration = 7.5f;
    [SerializeField]
    private float m_Deceleration = 5f;

    private Vector3 m_Movement;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_Rigidbody2D) m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Movement.x = Input.GetAxis("Horizontal");
        m_Movement.y = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        Rigidbody2D l_RigidBody2D = GetComponent<Rigidbody2D>();

        if (m_Movement.magnitude > 0f)
            l_RigidBody2D.AddForce(m_Movement.normalized * m_Acceleration);
        else
            l_RigidBody2D.velocity -= l_RigidBody2D.velocity * m_Deceleration * Time.fixedDeltaTime;

        if (l_RigidBody2D.velocity.magnitude > m_MaxSpeed)
            l_RigidBody2D.velocity = l_RigidBody2D.velocity.normalized * m_MaxSpeed;

        if (l_RigidBody2D.velocity.magnitude > m_Speed)
            l_RigidBody2D.velocity = l_RigidBody2D.velocity.normalized * m_Speed;
    }
}
