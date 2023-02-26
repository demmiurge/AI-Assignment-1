using UnityEngine;

[CreateAssetMenu(fileName = "FSM_Shark", menuName = "Finite State Machines/FSM_Shark", order = 1)]
public class FSM_Shark : FiniteStateMachine
{
    private Shark_Blackboard m_Blackboard;

    private float m_RestingTime = 0f;

    public override void OnEnter()
    {
        m_Blackboard = GetComponent<Shark_Blackboard>();
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

        FiniteStateMachine HUNTING = ScriptableObject.CreateInstance<FSM_SharkHunt>();

        State RESTING = new State("RESTING",
          () => { m_RestingTime = 0; },
          () => { m_RestingTime += Time.deltaTime; },
          () => { }
      );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition Rested = new Transition("Rested Enough",
            () => { return m_RestingTime >= m_Blackboard.m_RestingTime; },
            () => { m_Blackboard.m_Tiredness = 0f; }
        );

        Transition Tired = new Transition("Tired",

            () => { return m_Blackboard.m_Tiredness >= m_Blackboard.m_MaxTiredLevel; }
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/

        AddStates(HUNTING, RESTING);

        AddTransition(HUNTING, Tired, RESTING);
        AddTransition(RESTING, Rested, HUNTING);

        /* STAGE 4: set the initial state*/

        initialState = HUNTING;
         

    }
}
