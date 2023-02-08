using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    public List<GameObject> m_Members = new List<GameObject> ();

    public void Awake()
    {
        
    }

    public virtual void AddInitialBoids()
    {
        foreach (Transform t in transform)
            AddBoid(t.gameObject);
    }

    public void AddBoid(GameObject boid)
    {
        m_Members.Add(boid);
        boid.GetComponent<SteeringContext>().m_GroupManager = this;
        boid.transform.parent = transform;
    }

    public void RemoveBoid(GameObject boid)
    {
        m_Members.Remove(boid);
        boid.transform.parent = null;
        boid.GetComponent<SteeringContext>().m_GroupManager = null;
    }

    public void SetCohesionThreshold(float value)
    {
        foreach(GameObject go in m_Members)
        {
            go.GetComponent<SteeringContext>().m_CohesionThreshold = value;
        }
    }

    public void SetRepulsionThreshold(float value)
    {
        foreach(GameObject go in m_Members)
        {
            go.GetComponent<SteeringContext>().m_RepulsionThreshold = value;
        }
    }

    public void SetConeOfVisionAngle(float value)
    {
        foreach(GameObject go in m_Members)
        {
            go.GetComponent<SteeringContext>().m_ConeOfVisionAngle = value;
        }
    }

    public void SetCohesionWeight(float value)
    {
        foreach(GameObject go in m_Members)
        {
            go.GetComponent<SteeringContext>().m_CohesionWeight = value;
        }
    }

    public void SetRepulsionWeight(float value)
    {
        foreach (GameObject go in m_Members)
        {
            go.GetComponent<SteeringContext>().m_RepulsionWeight = value;
        }
    }

    public void SetAlignmentWeight(float value)
    {
        foreach (GameObject go in m_Members)
        {
            go.GetComponent<SteeringContext>().m_AlignmentWeight = value;
        }
    }

    public void SetMaxSpeed(float value)
    {
        foreach (GameObject go in m_Members)
        {
            go.GetComponent<SteeringContext>().m_MaxSpeed = value;
        }
    }

    public void SetMaxAcceleration(float value)
    {
        foreach (GameObject go in m_Members)
        {
            go.GetComponent<SteeringContext>().m_MaxAcceleration = value;
        }
    }

    public void SetSeekWeight(float value)
    {
        foreach (GameObject go in m_Members)
        {
            go.GetComponent<SteeringContext>().m_SeekWeight = value;
        }
    }

}
