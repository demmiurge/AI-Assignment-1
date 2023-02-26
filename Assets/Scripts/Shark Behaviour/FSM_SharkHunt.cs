using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "FSM_SharkHunt", menuName = "Finite State Machines/FSM_SharkHunt", order = 1)]
public class FSM_SharkHunt : FiniteStateMachine
{
    private GameObject m_Fish, m_OtherFish;
    private Pursue m_Pursue;
    private Shark_Blackboard m_Blackboard;
    private float m_PursueTime;
    private float m_ElapsedTime;

    public override void OnEnter()
    {
        m_Pursue = GetComponent<Pursue>();
        m_Blackboard = GetComponent<Shark_Blackboard>();
        m_PursueTime = 0;
        m_ElapsedTime = 0;
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

        FiniteStateMachine SALMON = ScriptableObject.CreateInstance<FSM_SharkSalmon>();
        SALMON.Name = "SALMON";

        State PursuingFish = new State("Pursuing",
          () => { m_Pursue.enabled = true; m_PursueTime = 0; m_Pursue.target = m_Fish; },
          () => { m_PursueTime += Time.deltaTime; },
          () => { m_Pursue.enabled = false; }
      );


        State Eating = new State("EatingFish",
           () => { m_ElapsedTime = 0f; m_Fish.transform.parent = transform; m_Fish.tag = "TRAPPED"; },
           () => { m_ElapsedTime += Time.deltaTime; },
           () =>
           {
               m_Blackboard.m_Hunger -= m_Blackboard.m_FishHungerDecrement;
               m_Blackboard.m_HUDManager.AddPoints(m_Blackboard.m_FishPoints);
               Destroy(m_Fish);
           }
       );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition fishDetected = new Transition("Fish Detected",
            () => { if (m_Blackboard.m_Hunger >= m_Blackboard.m_HungerTooHigh)
                {
                    m_Fish = SensingUtils.FindInstanceWithinRadius(gameObject, "FISH", m_Blackboard.m_FishDetectionRadius);
                }
                return m_Fish != null;
            },
            () => { }
        );

        Transition readyHunt = new Transition("Ready to Hunt",
          () => { return m_Blackboard.m_Hunger >= m_Blackboard.m_HungerTooHigh; },
          () => { }
      );

        Transition pursueTooLong = new Transition("Pursue too long",
            () => { return m_PursueTime >= m_Blackboard.m_PursueTime; },
            () => { }
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

        Transition fishVanished = new Transition("Fish Vanished",
            () => {
                return m_Fish.tag == "HIDDEN_FISH";
            },
            () => { }
        );

        Transition fishEaten= new Transition("Fish Eaten",
           () => { return m_ElapsedTime >= m_Blackboard.m_TimeToEat; },
           () => { }
       );

        Transition fishReached = new Transition("Fish Reached",
          () => { return SensingUtils.DistanceToTarget(gameObject, m_Fish) <= m_Blackboard.m_FishReachedRadius; },
          () => { }
      );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/

        AddStates(SALMON, PursuingFish, Eating);

        AddTransition(SALMON, fishDetected, PursuingFish);

        AddTransition(PursuingFish, fishReached, Eating);
        AddTransition(PursuingFish, fishCloser, PursuingFish);
        AddTransition(PursuingFish, fishVanished, SALMON);

        AddTransition(Eating, fishEaten, SALMON);

        /* STAGE 4: set the initial state*/

        initialState = SALMON; 

    }
}
