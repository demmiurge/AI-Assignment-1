
using UnityEngine;
using TMPro;

public class FishSpawner : MonoBehaviour
{
    public GameObject sample;
    private FISH_GLOBAL_Blackboard globalBlackboard;

    public int numInstances = 10;
    public float interval = 5f; // one entity every interval seconds

    public bool markFirst = true;

    public bool aware = false;
    public bool final = false;

    private int generated = 0;
    private float elapsedTime = 0f; // time elapsed since last generation

    void Start()
    {
        //sample = Resources.Load<GameObject>("Fish1");
        if (sample == null)
            Debug.LogError("No Fish1 prefab found as a resource");

        globalBlackboard = GetComponent<FISH_GLOBAL_Blackboard>();
        if (globalBlackboard == null)
            globalBlackboard = gameObject.AddComponent<FISH_GLOBAL_Blackboard>();
    }


    void Update()
    {
        if (generated == numInstances)
            return;

        if (elapsedTime >= interval)
        {
            // spawn creating an instance...
            GameObject clone = Instantiate(sample);
            clone.transform.position = this.transform.position;

            // give the global blackboard to the clone
            clone.GetComponent<FISH_Blackboard>().globalBlackboard = globalBlackboard;


            if (final)
            {
                //clone.GetComponent<FSMExecutor>().fsm = ScriptableObject.CreateInstance<FSM_MouseFinal>();
            }
            else if (aware)
            {
                clone.GetComponent<FSMExecutor>().fsm = ScriptableObject.CreateInstance<FSM_FishAware>();
            }

            generated++;
            elapsedTime = 0;

            //if (markFirst && generated == 1)
            //{

            //    clone.AddComponent<TrailRenderer>();

            //    clone.GetComponent<FSMExecutor>().textMesh = GameObject.Find("Canvas/MouseStateText").GetComponent<TextMeshProUGUI>();

            //    FieldToText ft2 = GameObject.Find("Canvas/MouseHungerText").AddComponent<FieldToText>();
            //    ft2.listenedObject = clone;
            //    ft2.componentTypeName = "MOUSE_Blackboard";
            //    ft2.fieldName = "hunger";
            //}
        }
        else
        {
            elapsedTime += Time.deltaTime;
        }
    }
}
