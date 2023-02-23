using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public GameObject m_BasicInfoScreen;
    public GameObject m_PauseScreen;
    public GameObject m_SharkBehaviour;
    public GameObject m_FishBehaviour;
    public GameObject m_PlanktonBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        // By default we show the interface inside the game
        GoToBasicInfoScreen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnOffAllInterface()
    {
        m_BasicInfoScreen.SetActive(false);
        m_PauseScreen.SetActive(false);
        m_SharkBehaviour.SetActive(false);
        m_FishBehaviour.SetActive(false);
        m_PlanktonBehaviour.SetActive(false);
    }

    public void GoToBasicInfoScreen()
    {
        // We turn off everything and activate what we want
        TurnOffAllInterface();
        m_BasicInfoScreen.SetActive(true);
    }

    public void GoToPauseScreen()
    {
        // We turn off everything and activate what we want
        TurnOffAllInterface();
        m_PauseScreen.SetActive(true);
    }

    public void GoToSharkBehaviour()
    {
        // We turn off everything and activate what we want
        TurnOffAllInterface();
        m_SharkBehaviour.SetActive(true);
    }

    public void GoToFishBehaviour()
    {
        // We turn off everything and activate what we want
        TurnOffAllInterface();
        m_FishBehaviour.SetActive(true);
    }

    public void GoToPlanktonBehaviour()
    {
        // We turn off everything and activate what we want
        TurnOffAllInterface();
        m_PlanktonBehaviour.SetActive(true);
    }

    public void RestartGame()
    {
        
    }

    public void ExitGame()
    {
        
    }
}
