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
    

    [Foldout("Sprint", styled = true)]
    [SerializeField]
    private KeyCode m_SprintKey = KeyCode.LeftShift;
    [SerializeField] 
    private float m_SprintMaxSpeed = 10.0f;


    private Vector3 m_Movement;
    private float l_TotalMaxSpeed;

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

        if (Input.GetKey(m_SprintKey))
            l_TotalMaxSpeed = m_SprintMaxSpeed;
        else
            l_TotalMaxSpeed = m_MaxSpeed;
    }

    void FixedUpdate()
    {
        Rigidbody2D l_RigidBody2D = GetComponent<Rigidbody2D>();


        if (m_Movement.magnitude > 0f)
            l_RigidBody2D.AddForce(m_Movement.normalized * m_Acceleration);
        else
            l_RigidBody2D.velocity -= l_RigidBody2D.velocity * m_Deceleration * Time.fixedDeltaTime;

        if (l_RigidBody2D.velocity.magnitude > l_TotalMaxSpeed)
            l_RigidBody2D.velocity = l_RigidBody2D.velocity.normalized * l_TotalMaxSpeed;

        if (l_RigidBody2D.velocity.magnitude > m_Speed)
            l_RigidBody2D.velocity = l_RigidBody2D.velocity.normalized * m_Speed;
    }
}
