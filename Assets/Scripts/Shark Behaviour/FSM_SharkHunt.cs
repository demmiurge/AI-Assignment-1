using UnityEngine;

[CreateAssetMenu(fileName = "FSM_SharkHunt", menuName = "Finite State Machines/FSM_SharkHunt", order = 1)]
public class FSM_SharkHunt : FiniteStateMachine
{
    private GameObject m_Fish, m_OtherFish;
    private PursuePlusOA m_Pursue;
    private Shark_Blackboard m_Blackboard;
    private float m_PursueTime;
    private float m_RestingTime;

    public override void OnEnter()
    {
        m_Pursue = GetComponent<PursuePlusOA>();
        m_Blackboard = GetComponent<Shark_Blackboard>();
        m_PursueTime = 0;
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/

        FiniteStateMachine SALMON = ScriptableObject.CreateInstance<FSM_SharkSalmon>();
        SALMON.Name = "SALMON";

        State PursuingFish = new State("Pursuing",
          () => { m_Pursue.enabled = true; m_PursueTime = 0; m_Pursue.target = m_Fish; },
          () => { m_PursueTime += Time.deltaTime; },
          () => { m_Pursue.enabled = false; }
      );

        State Eating = new State("EatingFish",
           () => { m_Fish.transform.parent = transform; m_Fish.tag = "TRAPPED"; },
           () => { },
           () => { Destroy(m_Fish); }
       );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition fishDetected = new Transition("Fish Detected",
            () => {
                m_Fish = SensingUtils.FindInstanceWithinRadius(gameObject, "FISH", m_Blackboard.m_FishDetectionRadius);
                return m_Fish != null;
            },
            () => { }
        );

        Transition fishReached = new Transition("Fish Reached",
          () => { return SensingUtils.DistanceToTarget(gameObject, m_Fish) <= m_Blackboard.m_FishReachedRadius; }
      );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/

        AddStates(SALMON, PursuingFish, Eating);

        AddTransition(SALMON, fishDetected, PursuingFish);
        AddTransition(PursuingFish, fishReached, Eating);


        /* STAGE 4: set the initial state*/

        initialState = SALMON; 

    }
}
