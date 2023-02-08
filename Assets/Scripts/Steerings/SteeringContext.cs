using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringContext : MonoBehaviour
{
    [Header("Linear Constraints")]
    public float m_MaxAcceleration = 40f;
    public float m_MaxSpeed = 10f;
    public bool m_ClipVelocity = true;

    [Header("Angular Constraints")]
    public float m_MaxAngularAcceleration = 360f;
    public float m_MaxAngularSpeed = 90f;
    public bool m_ClipAngularSpeed = true;

    [Header("Arrive Related Parameters")]
    public float m_TimeToDesiredSpeed = 0.1f;
    public float m_CloseEnoughRadius = 1f;
    public float m_SlowDownRadius = 5f;

    [Header("Align Related Parameters")]
    public float m_TimeToDesiredAngularSpeed = 0.1f;
    public float m_CloseEnoughAngle = 2f;
    public float m_SlowDownAngle = 10f;

    [Header("Pursue & Evade Related Parameters")]
    public float m_MaxPredictionTime = 3f;
    public bool m_ShowFutureTargetGizmos = false;

    [Header("Group & Flocking Related Parameters")]
    public string m_IdTag;
    public float m_RepulsionThreshold = 15f;
    public float m_CohesionThreshold = 30f;
    public float m_ConeOfVisionAngle = 270f;
    public bool m_ApplyVision = false;
    public float m_CohesionWeight = 0.3f;
    public float m_RepulsionWeight = 0.5f;
    public float m_AlignmentWeight = 0.2f;
    public bool m_AddWanderIfZero = true;

    [Header("Wander Related Parameters")]
    public float m_WanderRate = 30f;
    public float m_WanderRadius = 10f;
    public float m_WanderOffset = 20f;
    public bool m_ShowWanderGizmos = false;
    public float m_WanderTargetOrientation = 0f;

    [Header("Seek weights for steerings")]
    public float m_SeekWeight = 0.2f;

    [Header("Obstacle Avoidance Related Parameters")]
    public float m_LookAheadLength = 10f;
    public float m_AvoidDistance = 12f;
    public float m_SecondaryWhiskerAngle = 30f;
    public float m_SecondaryWhiskerRatio = 0.7f;
    public float m_PerseveranceTime = 0f;
    public bool m_ShowAvoidanceGizmos = false;

    public float m_PerseveranceElapsed = 0f;
    public bool m_Persevering = false;
    public Vector3 m_AvoidanceAcceleration = Vector3.zero;

    [Header("Velocity & Speeds")]
    public Vector3 m_Velocity = Vector3.zero;
    public float m_Speed;
    public float m_AngularSpeed;

    //Group Manager
    [Header("Group Manager Things")]
    public GroupManager m_GroupManager;

    void Awake()
    {
        if(m_GroupManager != null)
            m_GroupManager.m_Members.Add(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_Speed = m_Velocity.magnitude;
    }
}
