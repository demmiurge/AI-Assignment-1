
using UnityEngine;

public class FishGroupManager : GroupManager
{
    public int numInstances = 20;
    public float delay = 0.5f;
    public GameObject prefab;
    public bool around = false;
    public GameObject attractor;
    private FISH_GLOBAL_Blackboard globalBlackboard;

    private int created = 0;
    private float elapsedTime = 0f;

    // the following attributes are specifically created to help listeners of UI
    // components get the initial values for the UI elements they're attached to
    [HideInInspector]
    public float maxSpeed, maxAcceleration, cohesionThreshold, repulsionThreshold, coneOfVisionAngle,
    cohesionWeight, repulsionWeight, alignmentWeight, seekWeight;

    void Start()
    {
        GameObject dummy = Instantiate(prefab);
        SteeringContext context = dummy.GetComponent<SteeringContext>();
        maxSpeed = context.m_MaxSpeed;
        maxAcceleration = context.m_MaxAcceleration;
        cohesionThreshold = context.m_CohesionThreshold;
        repulsionThreshold = context.m_RepulsionThreshold;
        coneOfVisionAngle = context.m_ConeOfVisionAngle;
        cohesionWeight = context.m_CohesionWeight;
        repulsionWeight = context.m_RepulsionWeight;
        alignmentWeight = context.m_AlignmentWeight;
        seekWeight = context.m_SeekWeight;
        Destroy(dummy);

        globalBlackboard = GetComponent<FISH_GLOBAL_Blackboard>();
        if (globalBlackboard == null)
            globalBlackboard = gameObject.AddComponent<FISH_GLOBAL_Blackboard>();
    }

    // Update is called once per frame
    void Update()
    {
        Spawn();
    }


    private void Spawn()
    {
        if (created == numInstances) return;

        if (elapsedTime < delay)
        {
            elapsedTime += Time.deltaTime;
            return;
        }

        // if this point is reached, it's time to spawn a new instance
        GameObject clone = Instantiate(prefab);
        clone.transform.position = transform.position;
        clone.GetComponent<FISH_Blackboard>().globalBlackboard = globalBlackboard;
        if (around)
        {
            clone.AddComponent<FlockingAroundPlusAvoidance>();
            clone.GetComponent<FlockingAroundPlusAvoidance>().attractor = attractor;
            clone.GetComponent<FlockingAroundPlusAvoidance>().m_RotationalPolicy = SteeringBehaviour.RotationalPolicy.LWYGI;
        }
        else
        {
            clone.AddComponent<FlockingPlusAvoidance>();
            clone.GetComponent<FlockingPlusAvoidance>().m_RotationalPolicy = SteeringBehaviour.RotationalPolicy.LWYGI;
        }



        //if (created == 0)
        //{
        //    // first one and only it
        //    ShowRadiiPro shr = clone.GetComponent<ShowRadiiPro>();
        //    shr.componentTypeName = "Steerings.SteeringContext";
        //    shr.innerFieldName = "repulsionThreshold";
        //    shr.outerFieldName = "cohesionThreshold";
        //    shr.enabled = true;

        //    if (around)
        //    {
        //        if (clone.GetComponent<TrailRenderer>() != null)
        //        {
        //            clone.AddComponent<ToggleTrail>();
        //            clone.GetComponent<TrailRenderer>().enabled = true;
        //        }
        //    }
        //}

        AddBoid(clone);
        created++;
        elapsedTime = 0f;
    }
}
