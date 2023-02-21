//using FSMs;
using UnityEngine;
using System;
//using Steerings;

[CreateAssetMenu(fileName = "FSM_FishFeed", menuName = "Finite State Machines/FSM_FishFeed", order = 1)]
public class FSM_FishFeed : FiniteStateMachine
{

    private FISH_Blackboard blackboard;
    //private WanderPlusAvoid wanderPlusAvoid;
    private FlockingAroundPlusAvoidance flockingPlusAvoid;
    private ArrivePlusOA arrive;
    private float timeSinceLastBite;
    private GameObject plankton;
    private SteeringContext context;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<FISH_Blackboard>();
        context = GetComponent<SteeringContext>();
        //wanderPlusAvoid = GetComponent<WanderPlusAvoid>();
        flockingPlusAvoid = GetComponent<FlockingAroundPlusAvoidance>();
        arrive = GetComponent<ArrivePlusOA>();
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {


        /* STAGE 1: create the states with their logic(s) */

        State WANDERING = new State("WANDERING",
            () => { flockingPlusAvoid.enabled = true; },
            () => { blackboard.hunger += blackboard.normalHungerIncrement * Time.deltaTime; },
            () => { flockingPlusAvoid.enabled = false; }
        );

        State SPREAD = new State("SPREAD",
            () => { flockingPlusAvoid.enabled = true; context.m_CohesionThreshold *= 2f; context.m_RepulsionThreshold *= 2f; },
            () => { blackboard.hunger += blackboard.normalHungerIncrement * Time.deltaTime; },
            () => { context.m_CohesionThreshold *= 2f; context.m_RepulsionThreshold *= 2f; flockingPlusAvoid.enabled = false;  }
        );

        State REACHING = new State("REACHING PLANKTON",
            () => { arrive.target = plankton; arrive.enabled = true; },
            () => { blackboard.hunger += blackboard.normalHungerIncrement * Time.deltaTime; },
            () =>
            {
                arrive.enabled = false;
            }
        );

        State EATING = new State("EATING",
            () => { timeSinceLastBite = 100; },
            () => {
                if (timeSinceLastBite >= 1 / blackboard.bitesPerSecond)
                {
                    plankton.SendMessage("BeBitten");
                    blackboard.hunger -= blackboard.planktonHungerDecrement;
                    timeSinceLastBite = 0;
                }
                else
                {
                    timeSinceLastBite += Time.deltaTime;
                }
            },
            () => { /* do nothing in particular when exiting*/ }
        );



        /* STAGE 2: create the transitions with their logic(s)
         * --------------------------------------------------- */

        Transition hungryAndPlanktonDetected = new Transition("Plankton Detected",
           () => {
               if (!blackboard.Hungry()) return false;
               plankton = SensingUtils.FindInstanceWithinRadius(gameObject,
                                    blackboard.planktonLabel, blackboard.planktonDetectableRadius);
               return plankton != null;
           },
           () => { blackboard.globalBlackboard.AnnouncePlankton(plankton); }
        );

        Transition hungryAndPlanktonAnnounced = new Transition("Plankton Announced",
           () => {
               return blackboard.Hungry()
                           && blackboard.globalBlackboard.announcedPlankton != null;
           },
           () => { plankton = blackboard.globalBlackboard.announcedPlankton; }
        );

        Transition hungryAndNoPlankton = new Transition("Hungry NoPlankton",
           () => {
               if (!blackboard.Hungry()) return false;
               plankton = SensingUtils.FindInstanceWithinRadius(gameObject,
                                    blackboard.planktonLabel, blackboard.planktonDetectableRadius);
               return plankton == null;
           },
           () => { }
        );

        Transition planktonVanished = new Transition("Plankton vanished",
            () => { return plankton == null || plankton.Equals(null); }
        );

        Transition planktonReached = new Transition("Plankton reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, plankton) < blackboard.planktonReachedRadius; } // write the condition checkeing code in {}
        );

        Transition satiated = new Transition("satiated",
            () => { return blackboard.Satited(); }
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ---------------------------------------------- */


        AddStates(WANDERING, SPREAD, REACHING, EATING);

        AddTransition(WANDERING, hungryAndPlanktonDetected, REACHING);
        AddTransition(WANDERING, hungryAndPlanktonAnnounced, REACHING);
        AddTransition(WANDERING, hungryAndNoPlankton, SPREAD);
        AddTransition(SPREAD, hungryAndPlanktonDetected, REACHING);
        AddTransition(SPREAD, hungryAndPlanktonAnnounced, REACHING);
        AddTransition(REACHING, planktonVanished, WANDERING);
        AddTransition(REACHING, planktonReached, EATING);
        AddTransition(EATING, planktonVanished, WANDERING);
        AddTransition(EATING, satiated, WANDERING);

        /* STAGE 4: set the initial state */

        initialState = WANDERING;
    }
}
