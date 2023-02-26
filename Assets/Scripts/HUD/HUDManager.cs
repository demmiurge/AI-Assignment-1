using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    public KeyCode m_PrimaryPauseMenu = KeyCode.Escape;
    public KeyCode m_SecondaryPauseMenu = KeyCode.P;
    private KeyCode m_StartingGame = KeyCode.Space;

    public string m_NameScene = "Work";

    public GameObject m_StartingScreen;
    public GameObject m_BasicInfoScreen;
    public GameObject m_PauseScreen;
    public GameObject m_SharkBehaviour;
    public GameObject m_FishBehaviour;
    public GameObject m_PlanktonBehaviour;

    private float m_TimeScale;

    void Awake()
    {
        m_TimeScale = Time.timeScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        // We always show the beginning explained first
        GoToStartingScreen();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_StartingScreen.activeSelf) {
            if (Input.GetKeyDown(m_StartingGame))
                GoToBasicInfoScreen();
        }
        else
        {
            if (m_BasicInfoScreen.activeSelf)
            {
                if (Input.GetKeyDown(m_PrimaryPauseMenu) || Input.GetKeyDown(m_SecondaryPauseMenu))
                    GoToPauseScreen();
            }
            else
                if (Input.GetKeyDown(m_PrimaryPauseMenu) || Input.GetKeyDown(m_SecondaryPauseMenu))
                    GoToBasicInfoScreen();
        }
    }

    public void TurnOffAllInterface()
    {
        m_StartingScreen.SetActive(false);
        m_BasicInfoScreen.SetActive(false);
        m_PauseScreen.SetActive(false);
        m_SharkBehaviour.SetActive(false);
        m_FishBehaviour.SetActive(false);
        m_PlanktonBehaviour.SetActive(false);
    }

    public void GoToStartingScreen()
    {
        // We turn off everything and activate what we want
        StopTime();
        TurnOffAllInterface();
        m_StartingScreen.SetActive(true);
    }

    public void GoToBasicInfoScreen()
    {
        // We turn off everything and activate what we want
        TurnOffAllInterface();
        m_BasicInfoScreen.SetActive(true);
        ResumeTime();
    }

    public void GoToPauseScreen()
    {
        // We turn off everything and activate what we want
        TurnOffAllInterface();
        m_PauseScreen.SetActive(true);
        StopTime();
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
        SceneManager.LoadScene(m_NameScene);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #endif
        Application.Quit();
    }

    public void StopTime()
    {
        Time.timeScale = 0;
    }

    public void ResumeTime()
    {
        Time.timeScale = m_TimeScale;
    }
}
