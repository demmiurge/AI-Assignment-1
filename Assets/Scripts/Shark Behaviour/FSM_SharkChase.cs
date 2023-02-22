using UnityEngine;

[CreateAssetMenu(fileName = "FSM_SharkChase", menuName = "Finite State Machines/FSM_SharkChase", order = 1)]
public class FSM_SharkChase : FiniteStateMachine
{
    private GameObject m_Fish, m_OtherFish;
    private PursuePlusOA m_Pursue;
    private ArrivePlusOA m_Arrive;
    private Shark_Blackboard m_Blackboard;
    private SteeringContext m_SteeringContext;
    private float m_PursueTime;
    private float m_RestingTime;

    public override void OnEnter()
    {
        m_Pursue = GetComponent<PursuePlusOA>();
        m_Arrive = GetComponent<ArrivePlusOA>();
        m_Blackboard = GetComponent<Shark_Blackboard>();
        m_SteeringContext = GetComponent<SteeringContext>();
        m_PursueTime = 0;
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/
        FiniteStateMachine Wandering = ScriptableObject.CreateInstance<FSM_SharkSalmon>(); 

        State Hiding = new State("Hiding",
            () => { }, 
            () => { }, 
            () => { }  
        );

        State Pursuing = new State("Pursuing",
           () => { m_PursueTime = 0; m_Pursue.target = m_Fish; m_Pursue.enabled = true; },
           () => { m_PursueTime += Time.deltaTime; },
           () => { m_Pursue.enabled = false; }
       );

        State Resting = new State("Resting",
           () => { m_RestingTime = 0; },
           () => { m_RestingTime += Time.deltaTime; },
           () => { }
       );

        State Eating = new State("EatingFish",
           () => { m_Fish.transform.parent = transform; m_Fish.tag = "TRAPPED"; },
           () => { },
           () => { Destroy(m_Fish); }
       );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition fishDetected = new Transition("Fish Detected",
            () => { m_Fish = SensingUtils.FindInstanceWithinRadius(gameObject, "FISH", m_Blackboard.m_FishDetectionRadius);
                return m_Fish != null;
            }, 
            () => { } 
        );

        Transition readyHunt = new Transition("Ready to Hunt",
            () => { return m_RestingTime >= m_Blackboard.m_RestingTime; }
        );

        Transition pursueTooLong = new Transition("Pursue too long",
            () => { return m_PursueTime > m_Blackboard.m_PursueTime; }
        );

        Transition fishCloser = new Transition("Fish Closer",
           () => {
               m_OtherFish = SensingUtils.FindInstanceWithinRadius(gameObject, "FISH",
                                                                  m_Blackboard.m_FishDetectionRadius);
               return
                   m_OtherFish != null &&
                   SensingUtils.DistanceToTarget(gameObject, m_OtherFish)
                   < SensingUtils.DistanceToTarget(gameObject, m_Fish);
           },
           () => { m_Fish = m_OtherFish; }
       );

        Transition fishEscaped = new Transition("Fish escaped",
            () => {
                return SensingUtils.DistanceToTarget(gameObject, m_Fish)
                       >= m_Blackboard.m_FishEscaped;
            }
        );

        Transition fishReached = new Transition("Fish Reached",
           () => { return SensingUtils.DistanceToTarget(gameObject, m_Fish) <= m_Blackboard.m_FishReachedRadius; }
       );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/
            
        AddStates(Wandering, Hiding, Pursuing, Eating);

        AddTransition(Wandering, fishDetected, Pursuing);

        AddTransition(Pursuing, fishReached, Eating);
        AddTransition(Pursuing, fishEscaped, Wandering);
        AddTransition(Pursuing, pursueTooLong, Resting);
        AddTransition(Pursuing, fishCloser, Pursuing);

        AddTransition(Resting, readyHunt, Wandering);

        /* STAGE 4: set the initial state*/

        initialState = Wandering;


    }
}