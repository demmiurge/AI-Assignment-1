using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark_Blackboard : MonoBehaviour
{
    [Header("Two point wandering")]
    public GameObject target_A;
    public GameObject target_B;
    public float intervalBetweenTimeOuts = 10f;
    public float initialSeekWeight = 0.2f;
    public float incrementOfSeek = 0.2f;
    public float locationReachedRadius = 10f;
    public float m_SalmonDetectionRadius = 20f;
    public float m_SalmonReachedRadius = 5f;
    public float m_TimeToEatSalmon = 2f;
    public float m_FishDetectionRadius = 20f;
    public float m_FishReachedRadius = 2f;
    public float m_FishEscaped = 100f;
    public float m_RestingTime = 3f;
    public float m_PursueTime = 7f;

    // Start is called before the first frame update
    void Start()
    {
        if (target_A == null)
        {
            target_A = GameObject.Find("LOCATION_A");
            if (target_A == null)
            {
                Debug.LogError("no location A found");
            }
        }

        if (target_B == null)
        {
            target_B = GameObject.Find("LOCATION_B");
            if (target_B == null)
            {
                Debug.LogError("no location A found");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
