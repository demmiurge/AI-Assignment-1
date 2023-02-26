using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishFollowers_Blackboard : MonoBehaviour
{
    public GameObject m_Shark;
    public float m_SalmonDetectionRadius = 25f;
    public float m_SalmonReachedRadius = 2f;
    public float m_TimeToEat = 3f;
    // Start is called before the first frame update
    void Start()
    {
        if (m_Shark == null)
        {
            m_Shark = GameObject.Find("Shark");
            if (m_Shark == null)
            {
                Debug.LogError("no shark found");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
