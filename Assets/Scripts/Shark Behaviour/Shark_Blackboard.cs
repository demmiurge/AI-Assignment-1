using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark_Blackboard : MonoBehaviour
{
    public float m_Hunger = 0f;
    public float m_Tiredness = 0f;

    [Header("Wandering")]
    public GameObject target_A;
    public GameObject target_B;
    public float initialSeekWeight = 0.2f;
    public float incrementOfSeek = 0.2f;
    public float locationReachedRadius = 10f;
    [Header("Tiredness")]
    public float m_MaxTiredLevel = 100f;
    public float m_NormalTiredIncrement = 0.8f;
    public float m_RestingTime = 3f;
    [Header("Salmon Parameters")]
    public float m_SalmonDetectionRadius = 35f;
    public float m_SalmonReachedRadius = 4f;
    public float m_SalmonHungerDecrement = 3f;
    public float m_TimeToEat = 2f;
    [Header("Fish Parameters")]
    public float m_FishDetectionRadius = 30f;
    public float m_FishReachedRadius = 4f;
    public float m_FishEscaped = 50f;
    public float m_PursueTime = 7f;
    public float m_FishHungerDecrement = 5f;
    public float m_TimeToEatFish = 1f;
    [Header("Hunger Parameters")]
    public float m_HungerTooHigh = 30; 
    public float m_HungerLowEnough = 10; 
    public float m_NormalHungerIncrement = 1f;
    [Header("Points Agents Eated")]
    public int m_FishPoints = 5;
    public int m_SalmonPoints = 1;

    [Header("HUD")]
    public HUDManager m_HUDManager;

    // Start is called before the first frame update
    void Start()
    {
        if (target_A == null)
        {
            target_A = GameObject.Find("TargetA");
            if (target_A == null)
            {
                Debug.LogError("no location A found");
            }
        }

        if (target_B == null)
        {
            target_B = GameObject.Find("TargetB");
            if (target_B == null)
            {
                Debug.LogError("no location A found");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_Hunger += m_NormalHungerIncrement * Time.deltaTime;
        m_Tiredness += m_NormalTiredIncrement * Time.deltaTime;
    }
}
