using UnityEngine;

[CreateAssetMenu(fileName = "FSM_Plankton", menuName = "Finite State Machines/FSM_Plankton", order = 1)]
public class FSM_Plankton : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private Wander wander;
    private Arrive arrive;
    private GameObject seaweed;
    private PLANKTON_Blackboard blackboard;
    private float timeEating;
    private ParticleSystem particleSystem;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        wander = GetComponent<Wander>();
        arrive = GetComponent<Arrive>();
        blackboard = GetComponent<PLANKTON_Blackboard>();
        particleSystem = GetComponent<ParticleSystem>();
        seaweed = blackboard.seaweed;
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
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         
        State varName = new State("StateName",
            () => { }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

         */
        State WANDERING = new("WANDERING",
            () => { wander.enabled = true; particleSystem.Play(); },
            () => { blackboard.hunger += blackboard.DEFAULT_HUNGER_INCREMENT * Time.deltaTime; },
            () => { wander.enabled = false; });

        State REACHING = new("REACHING LIGHT",
            () => { arrive.target = seaweed; arrive.enabled = true; },
            () => { blackboard.hunger += blackboard.DEFAULT_HUNGER_INCREMENT * Time.deltaTime; },
            () => { arrive.enabled = false; });

        State PHOTOSYNTHESIS = new("PHOTOSYNTHESIS",
            () => { timeEating = 0f; },
            () => { if (timeEating < blackboard.TIME_TO_FEED) blackboard.feedingTime += Time.deltaTime; },
            () => { });

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        Transition hungryAndLightDetected = new("Light Detected",
            () =>
            {
                if (!blackboard.Hungry()) return false;
                seaweed = SensingUtils.FindInstanceWithinRadius(gameObject, "LIGHT", blackboard.LIGHT_DETECTABLE_RADIUS);
                return seaweed != null;
            });

        Transition lightReached = new("Light Reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, seaweed) < blackboard.LIGHT_REACHED_RADIUS; });

        Transition satiated = new("Satiated",
            () => { return blackboard.Satiated(); });


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(WANDERING, REACHING, PHOTOSYNTHESIS);
        AddTransition(WANDERING, hungryAndLightDetected, REACHING);
        AddTransition(REACHING, lightReached, PHOTOSYNTHESIS);
        AddTransition(PHOTOSYNTHESIS, satiated, WANDERING);

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = WANDERING;
    }
}
