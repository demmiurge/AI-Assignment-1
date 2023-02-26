using UnityEngine;

[CreateAssetMenu(fileName = "FSM_FollowerFish", menuName = "Finite State Machines/FSM_FollowerFish", order = 1)]
public class FSM_FollowerFish : FiniteStateMachine
{
    private KeepPosition m_KeepPosition;
    private KeepPositionPlusArrive m_Arrive;
    private FishFollowers_Blackboard m_Blackboard;
    private GameObject m_Food;
    private float m_elapsedTime = 0;

    public override void OnEnter()
    {
        m_KeepPosition = GetComponent<KeepPosition>();
        m_Arrive = GetComponent<KeepPositionPlusArrive>();
        m_Blackboard = GetComponent<FishFollowers_Blackboard>();
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
         
        State followShark = new State("FollowingShark",
            () => { m_KeepPosition.enabled = true; m_KeepPosition.target = m_Blackboard.m_Shark; }, 
            () => { }, 
            () => { m_KeepPosition.enabled = false; }  
        );

        State reachSalmon = new State("Reach_Salmon",
           () => { m_Arrive.enabled = true; m_Arrive.target = m_Blackboard.m_Shark; m_Arrive.attractor = m_Food; },
           () => { },
           () => { m_Arrive.enabled = false; }
       );

        State eatSalmon = new State("Eating_Salmon",
           () => { m_elapsedTime = 0; },
           () => { m_elapsedTime += Time.deltaTime; },
           () => { }
       );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition SalmonDetected = new Transition("Salmon Detected",
            () => {
                m_Food = SensingUtils.FindInstanceWithinRadius(gameObject, "SALMON", m_Blackboard.m_SalmonDetectionRadius);
                return m_Food != null;
            },
            () => { }
        );

        Transition SalmonReached = new Transition("Salmon Reached",
           () => {
               return SensingUtils.DistanceToTarget(gameObject, m_Food) < m_Blackboard.m_SalmonReachedRadius;
           },
           () => { }
       );

        Transition SalmonVanished = new Transition("Salmon Vanished",
            () =>
            {
                return m_Food.tag == "NOTSALMON";
            },
            () => { }
        );

        Transition TimeOut = new Transition("Time Out",
           () => { return m_elapsedTime >= m_Blackboard.m_TimeToEat; },
           () => { }
       );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/
            
        AddStates(followShark, reachSalmon, eatSalmon);

        AddTransition(followShark, SalmonDetected, reachSalmon);
        AddTransition(reachSalmon, SalmonVanished, followShark);
        AddTransition(reachSalmon, SalmonReached, eatSalmon);
        AddTransition(eatSalmon, SalmonVanished, followShark);
        AddTransition(eatSalmon, TimeOut, followShark);



        /* STAGE 4: set the initial state*/

        initialState = followShark;

    }
}
