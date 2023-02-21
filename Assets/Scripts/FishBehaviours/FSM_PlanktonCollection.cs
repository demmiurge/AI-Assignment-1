using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using UnityEditor.Search;
using System.ComponentModel;

[CreateAssetMenu(fileName = "FSM_PlanktonCollection", menuName = "Finite State Machines/FSM_PlanktonCollection", order = 1)]
public class FSM_PlanktonCollection : FiniteStateMachine
{
    private FISH_Blackboard blackboard;
    private ArrivePlusOA arrive;
    private float timeSinceLastBite;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        blackboard = GetComponent<FISH_Blackboard>();
        arrive = GetComponent<ArrivePlusOA>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        if (blackboard.plankton != null)
        {
            blackboard.plankton.transform.SetParent(null);
            blackboard.plankton.tag = blackboard.planktonLabel;
        }

        //Disable Steerings
        base.DisableAllSteerings();

        base.OnExit();
    }

    public override void OnConstruction()
    {
        //FSM
        FiniteStateMachine FISH_FEED = ScriptableObject.CreateInstance<FSM_FishFeed>();
        FISH_FEED.Name = "FISH_FEED";


        //States

        State REACHING = new State("REACHING PLANKTON",
            () => { arrive.target = blackboard.plankton; arrive.enabled = true;},
            () => { blackboard.hunger += blackboard.normalHungerIncrement * Time.deltaTime; },
            () =>
            {
                arrive.enabled = false;
            }
        );

        State TRANSPORTING = new State("TRANSPORTING",
           () => { arrive.target = blackboard.coral; arrive.enabled = true; blackboard.plankton.transform.SetParent(gameObject.transform); },
           () => { blackboard.hunger += blackboard.normalHungerIncrement * Time.deltaTime; },
           () => { arrive.enabled = false; blackboard.plankton.transform.SetParent(null); }
       );

        State EATING = new State("EATING",
            () => { timeSinceLastBite = 100; },
            () => {
                if (timeSinceLastBite >= 1 / blackboard.bitesPerSecond)
                {
                    blackboard.plankton.SendMessage("BeBitten");
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


        //Transitions

        Transition hungryAndPlanktonDetected = new Transition("Plankton Detected",
           () => {
               Debug.Log("Checking out");
               if (!blackboard.Hungry()) return false;
               blackboard.plankton = SensingUtils.FindInstanceWithinRadius(gameObject,
                                    blackboard.planktonLabel, blackboard.planktonDetectableRadius);
               return blackboard.plankton != null;
           },
           () => { blackboard.globalBlackboard.AnnouncePlankton(blackboard.plankton); }
        );

        Transition hungryAndPlanktonAnnounced = new Transition("Plankton Announced",
           () => {
               return blackboard.Hungry()
                           && blackboard.globalBlackboard.announcedPlankton != null;
           },
           () => { blackboard.plankton = blackboard.globalBlackboard.announcedPlankton; }
        );

        Transition planktonVanished = new Transition("Plankton vanished",
            () => { return blackboard.plankton == null || blackboard.plankton.Equals(null); }
        );

        Transition planktonReached = new Transition("Plankton Reached",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, blackboard.plankton)
                < blackboard.planktonReachedRadius;
            },

            () => { /*blackboard.plankton.tag = blackboard.noPlanktonLabel;*/ }
         );

        Transition coralReached = new Transition("Coral Reached",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, blackboard.coral)
                < blackboard.coralReachedRadius;
            },

            () => { blackboard.plankton.tag = blackboard.noPlanktonLabel; }
         );

        Transition satiated = new Transition("satiated",
            () => { return blackboard.Satiated(); }
        );


        //Add States and Transitions
        AddStates(FISH_FEED, REACHING, TRANSPORTING, EATING);


        AddTransition(REACHING, planktonVanished, FISH_FEED);
        AddTransition(REACHING, planktonReached, TRANSPORTING);
        AddTransition(TRANSPORTING, planktonVanished, FISH_FEED);
        AddTransition(TRANSPORTING, coralReached, EATING);
        AddTransition(EATING, planktonVanished, FISH_FEED);
        AddTransition(EATING, satiated, FISH_FEED);
        AddTransition(FISH_FEED, hungryAndPlanktonDetected, REACHING);
        AddTransition(FISH_FEED, hungryAndPlanktonAnnounced, REACHING);

        //Initial State
        initialState = FISH_FEED;

    }
}
