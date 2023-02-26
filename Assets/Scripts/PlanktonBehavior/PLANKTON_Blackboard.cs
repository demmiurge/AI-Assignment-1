using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLANKTON_Blackboard : MonoBehaviour
{
    public float hunger = 0f;
    public float feedingTime = 0f;
    public GameObject seaweed;

    // CONSTANTS
    public float HUNGER_TOO_HIGH = 100f;
    public float HUNGER_TOO_LOW = 5f;
    public float DEFAULT_HUNGER_INCREMENT = 5f;
    public float DEFAULT_HUNGER_DECREMENT = 10f;
    public float LIGHT_DETECTABLE_RADIUS = 100f;
    public float LIGHT_REACHED_RADIUS = 10f;
    public float TIME_TO_FEED = 20f;
    public float numberOfBites = 300f;

    // AUX METHODS
    public bool Hungry() { return hunger >= HUNGER_TOO_HIGH; }

    public bool AteEnough() { return feedingTime >= TIME_TO_FEED; }

    public void ResetHunger() { hunger = 0f; feedingTime = 0; }

    public void BeBitten()
    {
        numberOfBites--;
        if (numberOfBites <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
}
