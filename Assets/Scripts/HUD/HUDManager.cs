using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Input = UnityEngine.Input;

public class HUDManager : MonoBehaviour
{
    [Foldout("Keys and current scene", styled = true)]
    public KeyCode m_PrimaryPauseMenu = KeyCode.Escape;
    public KeyCode m_SecondaryPauseMenu = KeyCode.P;
    public KeyCode m_ContinueGame = KeyCode.Space;

    public string m_NameScene = "Work";

    [Foldout("HUD screens", styled = true)]
    public GameObject m_StartingScreen;
    public GameObject m_BasicInfoScreen;
    public GameObject m_PauseScreen;
    public GameObject m_SoundSettingsScreen;
    public GameObject m_SharkBehaviour;
    public GameObject m_FishBehaviour;
    public GameObject m_PlanktonBehaviour;
    public GameObject m_VictoryScreen;
    public GameObject m_DefeatScreen;

    [Foldout("Objects to Activate", styled = true)]
    public GameObject m_Shark;
    public GameObject m_Fish;
    public GameObject m_FishSpawner;
    public GameObject m_Plankton;

    [Foldout("Points", styled = true)]
    public int m_MaxPointsToWin = 100;
    private int m_CurrentPoints = 0;
    public TextMeshProUGUI m_TextScore;

    [Foldout("Timer", styled = true)]
    private bool m_TimerOn;
    public float m_TimerLeft = 300;
    public TextMeshProUGUI m_TextTimeLeft;

    [Foldout("Dev mode", styled = true)]
    public KeyCode m_AddPoints = KeyCode.F1;

    private float m_TimeScale;

    // Start is called before the first frame update
    void Start()
    {
        // First we get the current time
        m_TimeScale = Time.timeScale;

        // We always show the beginning explained first
        GoToStartingScreen();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_StartingScreen.activeSelf) {
            if (Input.GetKeyDown(m_ContinueGame))
                GoToBasicInfoScreen();
        }
        else if (m_VictoryScreen.activeSelf)
        {
            if (Input.GetKeyDown(m_ContinueGame))
                ExitGame();
        }
        else if (m_DefeatScreen.activeSelf)
        {
            if (Input.GetKeyDown(m_ContinueGame))
                ExitGame();
        }
        else
            Playing();

        DevMode(); // Only for testing
    }

    public void Playing()
    {
        Activate();
        if (m_BasicInfoScreen.activeSelf)
        {
            m_TimerOn = true;
            if (Input.GetKeyDown(m_PrimaryPauseMenu) || Input.GetKeyDown(m_SecondaryPauseMenu))
                GoToPauseScreen();
        }
        else
        {
            m_TimerOn = false;
            if (Input.GetKeyDown(m_PrimaryPauseMenu) || Input.GetKeyDown(m_SecondaryPauseMenu))
                GoToBasicInfoScreen();
        }

        if (m_TimerOn)
        {
            if (m_TimerLeft > 0)
            {
                m_TimerLeft -= Time.deltaTime;
                UpdateTime(m_TimerLeft);
            }
            else
            {
                m_TimerLeft = 0;
                m_TimerOn = false;
                GoToDefeatScreen();
            }
        }
    }

    private void DevMode()
    {
        if (Input.GetKeyDown(m_AddPoints))
            AddPoints(10);
    }

    public void AddPoints(int Points)
    {
        m_CurrentPoints += Points;

        if (m_CurrentPoints > m_MaxPointsToWin)
            m_CurrentPoints = m_MaxPointsToWin;

        if (m_CurrentPoints == m_MaxPointsToWin)
            GoToVictoryScreen();

        m_TextScore.text = $"Score: {m_CurrentPoints}/{m_MaxPointsToWin}";
    }

    public void TurnOffAllInterface()
    {
        m_StartingScreen.SetActive(false);
        m_BasicInfoScreen.SetActive(false);
        m_PauseScreen.SetActive(false);
        m_SoundSettingsScreen.SetActive(false);
        m_SharkBehaviour.SetActive(false);
        m_FishBehaviour.SetActive(false);
        m_PlanktonBehaviour.SetActive(false);
        m_VictoryScreen.SetActive(false);
        m_DefeatScreen.SetActive(false);
    }

    public void GoToVictoryScreen()
    {
        // We turn off everything and activate what we want
        //StopTime();
        Deactivate();
        TurnOffAllInterface();
        m_VictoryScreen.SetActive(true);
        AudioManager.Instance.PlayWinScreenSound();
    }

    public void GoToDefeatScreen()
    {
        // We turn off everything and activate what we want
        //StopTime();
        Deactivate();
        TurnOffAllInterface();
        m_DefeatScreen.SetActive(true);
        AudioManager.Instance.PlayFailScreenSound();
    }

    public void GoToStartingScreen()
    {
        // We turn off everything and activate what we want
        //StopTime();
        Deactivate();
        TurnOffAllInterface();
        m_StartingScreen.SetActive(true);
    }

    public void GoToBasicInfoScreen()
    {
        // We turn off everything and activate what we want
        AudioManager.Instance.FadeInMusic();
        TurnOffAllInterface();
        m_BasicInfoScreen.SetActive(true);
        ResumeTime();
    }

    public void GoToPauseScreen()
    {
        // We turn off everything and activate what we want
        AudioManager.Instance.FadeOutMusic();
        TurnOffAllInterface();
        m_PauseScreen.SetActive(true);
        StopTime();
    }

    public void GoToSoundSettingsScreen()
    {
        // We turn off everything and activate what we want
        TurnOffAllInterface();
        m_SoundSettingsScreen.SetActive(true);
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
        // Patch to restart level needs to call resume time
        ResumeTime();

        // Load scene
        SceneManager.LoadScene(m_NameScene);
        AudioManager.Instance.ResetMusic();
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #endif
        Application.Quit();
    }

    private void UpdateTime(float CurrentTime)
    {
        CurrentTime += 1;

        float l_Minutes = Mathf.FloorToInt(CurrentTime / 60);
        float l_Seconds = Mathf.FloorToInt(CurrentTime % 60);

        m_TextTimeLeft.text = string.Format("Time: {0:00}:{1:00}", l_Minutes, l_Seconds);
    }

    public void StopTime()
    {
        Time.timeScale = 0;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
    }

    public void Deactivate()
    {
        m_Shark.SetActive(false);
        m_Fish.SetActive(false);
        m_FishSpawner.SetActive(false);
        //m_Plankton.SetActive(false);
    }

    public void Activate()
    {
        m_Shark.SetActive(true);
        m_Fish.SetActive(true);
        m_FishSpawner.SetActive(true);
        //m_Plankton.SetActive(true);
    }
}
