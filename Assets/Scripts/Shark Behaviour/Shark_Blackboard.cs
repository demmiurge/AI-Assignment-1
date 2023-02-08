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
