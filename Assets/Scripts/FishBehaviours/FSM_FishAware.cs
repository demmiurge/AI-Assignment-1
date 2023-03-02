//using FSMs;
using UnityEngine;
//using Steerings;

[CreateAssetMenu(fileName = "FSM_FishAware", menuName = "Finite State Machines/FSM_FishAware", order = 1)]
public class FSM_FishAware : FiniteStateMachine
{
    private FleePlusOA flee;
    private ArrivePlusFlee arrivePlusFlee;
    private FISH_Blackboard blackboard;
    private SteeringContext steeringContext;
    private GameObject peril;
    private float elapsedTime = 0f;
    private float normalSpeed, normalAcceleration;
    private Color normalColor;

    public override void OnEnter()
    {

        flee = GetComponent<FleePlusOA>();
        arrivePlusFlee = GetComponent<ArrivePlusFlee>();
        blackboard = GetComponent<FISH_Blackboard>();
        steeringContext = GetComponent<SteeringContext>();

        normalSpeed = steeringContext.m_MaxSpeed;
        normalAcceleration = steeringContext.m_MaxAcceleration;

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        steeringContext.m_MaxSpeed = normalSpeed;
        steeringContext.m_MaxAcceleration = normalAcceleration;

        DisableAllSteerings();

        base.OnExit();
    }

    public override void OnConstruction()
    {

        FiniteStateMachine PLANKTON_COLLECTION = ScriptableObject.CreateInstance<FSM_PlanktonCollection>();
        PLANKTON_COLLECTION.Name = "PLANKTON_COLLECTION";

        State FLEE_PERIL = new State("FLEEING",
            () =>
            {
                if (Random.value > 0.6f)
                {
                    float boostFactor = 1.3f + Random.value;
                    steeringContext.m_MaxSpeed *= boostFactor;
                    steeringContext.m_MaxAcceleration *= boostFactor;
                }
                blackboard.SetNearestCoralHideout();
                arrivePlusFlee.target = blackboard.coral;
                arrivePlusFlee.peril = peril;
                arrivePlusFlee.enabled = true;
            },
            () => { blackboard.hunger += blackboard.normalHungerIncrement * Time.deltaTime; },
            () =>
            {
                steeringContext.m_MaxSpeed = normalSpeed;
                steeringContext.m_MaxAcceleration = normalAcceleration;
                arrivePlusFlee.enabled = false;
            }
        );

        State HIDING = new State("Hiding",
           () => { gameObject.tag = blackboard.hiddenTag; },
           () => { blackboard.hunger += blackboard.normalHungerIncrement * Time.deltaTime; },
           () => { gameObject.tag = blackboard.defaultTag; }
       );

        State TRAPPED = new State("Trapped",
           () => { },
           () => { },
           () => { }
       );

        Transition perilDetected = new Transition("Peril detected",
            () =>
            {
                peril = SensingUtils.FindInstanceWithinRadius(gameObject, blackboard.perilLabel, blackboard.perilDetectableRadius);
                return peril != null;
            }, // write the condition checkeing code in {}
            () => { }
        );

        Transition perilEvaded = new Transition("Peril evaded",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, peril) >= blackboard.perilSafetyRadius;
            }, // write the condition checkeing code in {}
            () => { }
        );

        Transition hidden = new Transition("Hidden",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, blackboard.coral) <= blackboard.coralReachedRadius;
            }, // write the condition checkeing code in {}
            () => { }
        );

        Transition isTrapped = new Transition("Is trapped",
            () =>
            {
                return gameObject.tag == blackboard.trappedTag;
            }, // write the condition checkeing code in {}
            () => { }
        );


        AddStates(PLANKTON_COLLECTION, FLEE_PERIL, HIDING, TRAPPED);

        AddTransition(PLANKTON_COLLECTION, perilDetected, FLEE_PERIL);
        AddTransition(FLEE_PERIL, isTrapped, TRAPPED);
        AddTransition(FLEE_PERIL, hidden, HIDING);
        AddTransition(FLEE_PERIL, perilEvaded, PLANKTON_COLLECTION);
        AddTransition(HIDING, perilEvaded, PLANKTON_COLLECTION);

        initialState = PLANKTON_COLLECTION;
    }
}
