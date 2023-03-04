using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FSM_Plankton", menuName = "Finite State Machines/FSM_Plankton", order = 1)]
public class FSM_Plankton : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private WanderAround _wanderAround;
    private Arrive _arrive;
    private GameObject _light;
    private PLANKTON_Blackboard _blackboard;
    private ParticleSystem _particleSystem;
    private ParticleSystem _siblingParticleSystem;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        _wanderAround = GetComponent<WanderAround>();
        _arrive = GetComponent<Arrive>();
        _blackboard = GetComponent<PLANKTON_Blackboard>();
        _particleSystem = GetComponent<ParticleSystem>();
        _siblingParticleSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
        _siblingParticleSystem.Stop();
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
            () => { _wanderAround.enabled = true; _particleSystem.Play(); _siblingParticleSystem.Stop(); },
            () => { _blackboard.Hunger += _blackboard.DefaultHungerIncrement * Time.deltaTime; },
            () => { _wanderAround.enabled = false; });

        State REACHING = new("REACHING LIGHT",
            () => { _arrive.enabled = true; _arrive.target = _light; },
            () => { _blackboard.Hunger += _blackboard.DefaultHungerIncrement * Time.deltaTime; },
            () => { _arrive.enabled = false; });

        State PHOTOSYNTHESIS = new("PHOTOSYNTHESIS",
            () => { },
            () => { _blackboard.FeedingTime += Time.deltaTime; },
            () => { _blackboard.ResetHunger(); });

        State TRAPPED = new("TRAPPED",
            () => { _particleSystem.Stop(); _siblingParticleSystem.Play(); },
            () => { },
            () => { });

        State DIE = new("DIE",
            () => gameObject.SetActive(false),
            () => { },
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
                if (!_blackboard.Hungry()) return false;
                _light = SensingUtils.FindInstanceWithinRadius(gameObject, "LIGHT", _blackboard.LightDetectableRadius);
                return _light != null;
            });

        Transition lightReached = new("Light Reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, _light) < _blackboard.LightReachedRadius; });

        Transition satiated = new("Satiated",
            () => { return _blackboard.AteEnough(); });

        Transition isTrapped = new("Is Trapped",
            () => { return gameObject.CompareTag("PLANKTON_TRAPPED"); });

        Transition death = new("Death",
            () => { return gameObject.CompareTag("NO_PLANKTON") || _blackboard.NoPlanktonLeft(); });


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(WANDERING, REACHING, PHOTOSYNTHESIS, TRAPPED, DIE);

        AddTransition(WANDERING, hungryAndLightDetected, REACHING);
        AddTransition(REACHING, lightReached, PHOTOSYNTHESIS);
        AddTransition(PHOTOSYNTHESIS, satiated, WANDERING);
        AddTransition(WANDERING, isTrapped, TRAPPED);
        AddTransition(REACHING, isTrapped, TRAPPED);
        AddTransition(PHOTOSYNTHESIS, isTrapped, TRAPPED);
        AddTransition(TRAPPED, death, DIE);


        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = WANDERING;
    }
}
