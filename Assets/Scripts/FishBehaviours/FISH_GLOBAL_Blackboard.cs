using UnityEngine;

public class FISH_GLOBAL_Blackboard : MonoBehaviour
{
    public GameObject announcedPlankton;      // the cheese some mouse has found
    public float planktonAnnounceTTL = 30f;   // the time the announce will last

    public float elapsedTime = 0;

    public void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= planktonAnnounceTTL)
            announcedPlankton = null; // 
    }

    public void AnnouncePlankton(GameObject Plankton)
    {
        announcedPlankton = Plankton;
        elapsedTime = 0f;
    }
}