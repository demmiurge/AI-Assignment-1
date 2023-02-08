using UnityEngine;

[CreateAssetMenu(fileName = "FSM_Shark", menuName = "Finite State Machines/FSM_Shark", order = 1)]
public class FSM_Shark : FiniteStateMachine
{
    private WanderAround m_WanderAround;
    private SteeringContext m_SteeringContext;
    private Shark_Blackboard m_Blackboard;

    private float elapsedTime = 0;

    public override void OnEnter()
    {
        m_Blackboard = GetComponent<Shark_Blackboard>();
        m_WanderAround = GetComponent<WanderAround>();
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        //base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/
         
       State goingA = new State("Going_A",
           () => { m_WanderAround.enabled = true; m_WanderAround.attractor = m_Blackboard.target_A; elapsedTime = 0; },
           () => { elapsedTime += Time.deltaTime; }, 
           () => { m_WanderAround.enabled = false; }
       );

        State goingB = new State("Going_B",
           () => { m_WanderAround.enabled = true; m_WanderAround.attractor = m_Blackboard.target_B; elapsedTime = 0; },
           () => { elapsedTime += Time.deltaTime; },
           () => { m_WanderAround.enabled = false; }
       );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

         Transition locationAReached = new Transition("Location A Reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_Blackboard.target_A) <= m_Blackboard.locationReachedRadius; },
            () => { gameObject.GetComponent<SteeringContext>().m_SeekWeight = 0.2f; }          
        );

        Transition locationBReached = new Transition("Location B Reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_Blackboard.target_B) <= m_Blackboard.locationReachedRadius; },
            () => { gameObject.GetComponent<SteeringContext>().m_SeekWeight = 0.2f; }
        );

        Transition TimeOut = new Transition("Time Out",
            () => { return elapsedTime >= m_Blackboard.intervalBetweenTimeOuts; },
            () => { gameObject.GetComponent<SteeringContext>().m_SeekWeight += m_Blackboard.incrementOfSeek; elapsedTime = 0; }
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/
            
         AddStates(goingA, goingB);

        AddTransition(goingA, locationAReached, goingB);
        AddTransition(goingB, locationBReached, goingA);
        //AddTransition(goingA, TimeOut, goingA);
        //AddTransition(goingB, TimeOut, goingB);



        /* STAGE 4: set the initial state*/
         
        initialState = goingA;
         

    }
}
