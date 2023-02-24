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
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/

        FiniteStateMachine SalmonState = ScriptableObject.CreateInstance<FSM_SharkHunt>();


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition TimeOut = new Transition("Time Out",
            () => { return elapsedTime >= m_Blackboard.intervalBetweenTimeOuts; },
            () => { gameObject.GetComponent<SteeringContext>().m_SeekWeight += m_Blackboard.incrementOfSeek; elapsedTime = 0; }
        );

        Transition notHungry = new Transition("Not Hungry",

            () => { return m_Blackboard.m_Hunger <= m_Blackboard.m_HungerLowEnough; }
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/

        AddStates(SalmonState);

        AddTransition(SalmonState, TimeOut, SalmonState);
        //AddTransition(goingB, locationBReached, goingA);


        /* STAGE 4: set the initial state*/
         
        initialState = SalmonState;
         

    }
}
