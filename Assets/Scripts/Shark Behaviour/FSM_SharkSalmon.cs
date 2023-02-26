using UnityEngine;

[CreateAssetMenu(fileName = "FSM_SharkSalmon", menuName = "Finite State Machines/FSM_SharkSalmon", order = 1)]
public class FSM_SharkSalmon : FiniteStateMachine
{
    private Shark_Blackboard m_Blackboard;
    private WanderAroundPlusAvoid m_WanderAround;
    private Arrive m_Arrive;
    private GameObject m_Salmon;
    private float m_elapsedTime = 0;

    public override void OnEnter()
    {
        m_Blackboard = GetComponent<Shark_Blackboard>();
        m_WanderAround = GetComponent<WanderAroundPlusAvoid>();
        m_Arrive = GetComponent<Arrive>();
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/
         
        State wanderState = new State("Wandering",
            () => { m_WanderAround.enabled = true; m_WanderAround.attractor = m_Blackboard.target_B; }, 
            () => { }, 
            () => { m_WanderAround.enabled = false; }   
        );

        State reachSalmon = new State("Reaching_Salmon",
            () => { m_Arrive.enabled = true; m_Arrive.target = m_Salmon; },
            () => { },
            () => { m_Arrive.enabled = false; }
        );

        State eatSalmon = new State("Eating_Salmon",
           () => { m_elapsedTime = 0; },
           () => { m_elapsedTime += Time.deltaTime; },
           () =>
           {
               m_Blackboard.m_Hunger -= m_Blackboard.m_SalmonHungerDecrement;
               m_Blackboard.m_HUDManager.AddPoints(m_Blackboard.m_SalmonPoints);
               m_Salmon.SetActive(false); 
               m_Salmon.tag = "NOTSALMON";
           }
       );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition SalmonDetected = new Transition("Salmon Detected",
            () => { m_Salmon = SensingUtils.FindInstanceWithinRadius(gameObject, "SALMON", m_Blackboard.m_SalmonDetectionRadius);
                return m_Salmon != null;
            }, 
            () => { } 
        );

        Transition SalmonReached = new Transition("Salmon Reached",
            () => {
                return SensingUtils.DistanceToTarget(gameObject, m_Salmon) < m_Blackboard.m_SalmonReachedRadius;},
            () => { }
        );

        Transition SalmonVanished = new Transition("Salmon Vanished",
            () =>
            {
                return m_Salmon.tag == "NOTSALMON"; },
            () => { }
        );

        Transition TimeOut = new Transition("Time Out",
           () => { return m_elapsedTime >= m_Blackboard.m_TimeToEat; },
           () => { }
       );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/
            
        AddStates(wanderState, reachSalmon, eatSalmon);

        AddTransition(wanderState, SalmonDetected, reachSalmon);
        AddTransition(reachSalmon, SalmonVanished, wanderState);
        AddTransition(reachSalmon, SalmonReached, eatSalmon);
        AddTransition(eatSalmon, SalmonVanished, wanderState);
        AddTransition(eatSalmon, TimeOut, wanderState);


        /* STAGE 4: set the initial state*/

        initialState = wanderState;

    }
}
