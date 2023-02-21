using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using UnityEditor.Search;

[CreateAssetMenu(fileName = "FSM_PlanktonCollection", menuName = "Finite State Machines/FSM_PlanktonCollection", order = 1)]
public class FSM_PlanktonCollection : FiniteStateMachine
{
    private FISH_Blackboard blackboard;
    private Arrive arrive;
    private GameObject seed;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        blackboard = GetComponent<FISH_Blackboard>();
        arrive = GetComponent<Arrive>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        if (seed != null)
        {
            seed.transform.SetParent(null);
            seed.tag = blackboard.planktonLabel;
        }

        //Disable Steerings
        base.DisableAllSteerings();

        base.OnExit();
    }

    public override void OnConstruction()
    {
        //FSM
        //FiniteStateMachine TWO_POINT_WANDERING = ScriptableObject.CreateInstance<FSM_TwoPointWandering>();
        //TWO_POINT_WANDERING.Name = "TWO_POINT_WANDERING";


        //States

        State goingToSeed = new State("Going_To_Seed",
           () => { arrive.target = seed; arrive.enabled = true; },
           () => { },
           () => { arrive.enabled = false; }
       );

        State transportingSeedToNest = new State("Transporting_Seed_To_Nest",
           () => { arrive.target = blackboard.coral; arrive.enabled = true; seed.transform.SetParent(gameObject.transform); },
           () => { },
           () => { arrive.enabled = false; seed.transform.SetParent(null); }
       );


        //Transitions

        Transition nearbySeedDetected = new Transition("NearbySeedDetected",
            () => {
                seed = SensingUtils.FindRandomInstanceWithinRadius(gameObject, blackboard.planktonLabel, blackboard.planktonDetectableRadius);
                return seed != null;
            },

            () => { }
        );

        Transition seedReached = new Transition("Seed Reached",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, seed)
                < blackboard.planktonReachedRadius;
            },

            () => { seed.tag = "NO_SEED"; }
         );

        Transition nestReached = new Transition("Nest Reached",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, blackboard.coral)
                < blackboard.coralReackedRadius;
            },

            () => { }
         );


        //Add States and Transitions
        //AddStates(TWO_POINT_WANDERING, goingToSeed, transportingSeedToNest);

        //AddTransition(goingToSeed, seedReached, transportingSeedToNest);
        //AddTransition(transportingSeedToNest, nestReached, TWO_POINT_WANDERING);
        //AddTransition(TWO_POINT_WANDERING, nearbySeedDetected, goingToSeed);

        ////Initial State
        //initialState = TWO_POINT_WANDERING;

    }
}
