//using FSMs;
using UnityEngine;
using System;
using UnityEngine.UIElements.Experimental;
//using Steerings;

[CreateAssetMenu(fileName = "FSM_FishFeed", menuName = "Finite State Machines/FSM_FishFeed", order = 1)]
public class FSM_FishFeed : FiniteStateMachine
{

    private FISH_Blackboard blackboard;
    private FlockingAroundPlusAvoidance flockingPlusAvoid;
    private ArrivePlusOA arrive;
    private float timeSinceLastBite;
    private SteeringContext context;

    public override void OnEnter()
    {
        blackboard = GetComponent<FISH_Blackboard>();
        context = GetComponent<SteeringContext>();
        //wanderPlusAvoid = GetComponent<WanderPlusAvoid>();
        flockingPlusAvoid = GetComponent<FlockingAroundPlusAvoidance>();
        arrive = GetComponent<ArrivePlusOA>();
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {

        DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {

        State WANDERING = new State("WANDERING",
            () => { flockingPlusAvoid.enabled = true; },
            () => { blackboard.hunger += blackboard.normalHungerIncrement * Time.deltaTime; },
            () => { flockingPlusAvoid.enabled = false; }
        );

        State SPREAD = new State("SPREAD",
            () => { flockingPlusAvoid.enabled = true; context.m_CohesionThreshold = 4f; context.m_RepulsionThreshold = 2f; },
            () => { blackboard.hunger += blackboard.normalHungerIncrement * Time.deltaTime; },
            () => { context.m_CohesionThreshold = 2f; context.m_RepulsionThreshold = 1f; flockingPlusAvoid.enabled = false; }
        );

        State WIDE_SPREAD = new State("WIDE_SPREAD",
            () => { flockingPlusAvoid.enabled = true; context.m_CohesionThreshold = 8f; context.m_RepulsionThreshold = 4f; },
            () => { blackboard.hunger += blackboard.normalHungerIncrement * Time.deltaTime; },
            () => { context.m_CohesionThreshold = 2f; context.m_RepulsionThreshold = 1f; flockingPlusAvoid.enabled = false; }
        );

        Transition hungryAndNoPlankton = new Transition("Hungry NoPlankton",
           () => {
               if (blackboard.Hungry()) return true;
               else return false;
           },
           () => { }
        );

        Transition veryHungryAndNoPlankton = new Transition("Very Hungry NoPlankton",
           () => {
               if (blackboard.VeryHungry()) return true;
               else return false;
           },
           () => { }
        );

        AddStates(WANDERING, SPREAD, WIDE_SPREAD);

        AddTransition(WANDERING, hungryAndNoPlankton, SPREAD);
        AddTransition(SPREAD, veryHungryAndNoPlankton, WIDE_SPREAD);

        initialState = WANDERING;
    }
}
