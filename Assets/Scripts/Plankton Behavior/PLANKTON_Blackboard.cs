using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLANKTON_Blackboard : MonoBehaviour
{
    public float Hunger = 0f;
    public float FeedingTime = 0f;
    public GameObject Attractor;

    // CONSTANTS
    [Header("Hunger params")]
    public float HungerTooHigh = 100f;
    public float HungerTooLow = 5f;
    public float DefaultHungerIncrement = 5f;
    public float DefaultHungerDecrement = 10f;

    [Header("Light detection params")]
    public float LightDetectableRadius = 100f;
    public float LightReachedRadius = 10f;

    [Header("Eating params")]
    public float TimeToFeed = 20f;
    public float NumberOfBites = 300f;

    // AUX METHODS
    public bool Hungry() { return Hunger >= HungerTooHigh; }

    public bool AteEnough() { return FeedingTime >= TimeToFeed; }

    public void ResetHunger() { Hunger = 0f; FeedingTime = 0f; }

    public void BeBitten() { NumberOfBites--; }

    public bool NoPlanktonLeft() { return NumberOfBites <= 0f; }
}
