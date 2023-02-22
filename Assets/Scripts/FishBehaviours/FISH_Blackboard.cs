using UnityEngine;


public class FISH_Blackboard : MonoBehaviour
{

    public float hunger = 0f;   // How hungry am I?

    // CONSTANTS
    public float hungerTooHigh = 100; // when this level is reachead I'M HUNGRY
    public float hungerLowEnough = 10; // when this level is reached I'M SATIATED
    public float normalHungerIncrement = 5.5f; // speed of hunger increment
    public float bitesPerSecond = 1f; // one bite each 1/bitesPerSecond seconds
    public float planktonHungerDecrement = 10f; // decrement per bite
    public float planktonDetectableRadius = 20f; // at this distance cheese is detectable
    public float planktonReachedRadius = 5f; // at this distance, cheese is reachable (can be bitten)
    public string planktonLabel = "PLANKTON";
    public string noPlanktonLabel = "NO_PLANKTON";
    public GameObject plankton;
    public float waitTime = 8f;
    public float perilDetectableRadius = 10f; // at this distance enenmy is detectable
    public float perilSafetyRadius = 20f; // at this distance, enemy is no longer a peril
    public string perilLabel = "SHARK";
    public GameObject coral;
    public string coralLabel = "CORAL";
    public float coralReachedRadius = 2f;
    public GameObject defaultAttractor;

    public FISH_GLOBAL_Blackboard globalBlackboard; // the blackboard that all fish share

    // aux. methods
    public bool Hungry() { return hunger >= hungerTooHigh; }
    public bool VeryHungry() { return hunger >= hungerTooHigh * 2f; }
    public bool Satiated() { return hunger <= hungerLowEnough; }

    public void SetNearestCoralHideout()
    {
        GameObject nearestHideout = globalBlackboard.coralHideouts[0];
        for (int i = 1; i < globalBlackboard.coralHideouts.Length; i++)
        {
            if (SensingUtils.DistanceToTarget(gameObject, globalBlackboard.coralHideouts[i]) < SensingUtils.DistanceToTarget(gameObject, nearestHideout))
            {
                nearestHideout = globalBlackboard.coralHideouts[i];
            }
        }
        coral = nearestHideout;
    }

}
