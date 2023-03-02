using UnityEngine;


public class FISH_Blackboard : MonoBehaviour
{

    public float hunger = 0f;   
    public float hungerTooHigh = 100; 
    public float hungerLowEnough = 20; 
    public float normalHungerIncrement = 2f; 
    public float bitesPerSecond = 1f;
    public float planktonHungerDecrement = 10f; 
    public float planktonDetectableRadius = 50f; 
    public float planktonReachedRadius = 2f;
    public string planktonLabel = "PLANKTON";
    public string noPlanktonLabel = "NO_PLANKTON";
    public GameObject plankton;
    public float waitTime = 8f;
    public float perilDetectableRadius = 50f; 
    public float perilSafetyRadius = 70f;
    public string perilLabel = "SHARK";
    public GameObject coral;
    public string coralLabel = "CORAL";
    public float coralReachedRadius = 0.8f;
    public GameObject defaultAttractor;

    public string hiddenTag = "HIDDEN_FISH";
    public string defaultTag = "FISH";
    public string trappedTag = "TRAPPED";

    public FISH_GLOBAL_Blackboard globalBlackboard;

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
