using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickerController : MonoBehaviour
{
    [SerializeField] 
    private GameObject m_Meet;
    private HUDManager m_HUDManager;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Meet) m_Meet.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos;

        if (Input.GetMouseButtonDown(0))
            m_Meet.tag = "NOTSALMON";
        if (Input.GetMouseButtonUp(0))
            Clicked();

        if (Input.GetMouseButtonDown(1))
        {
            m_Meet.SetActive(false);
            m_Meet.tag = "NOTSALMON";
        }
    }

    void Clicked()
    {
        // Force a code detection if the game is starting
        if (m_HUDManager.m_StartingScreen.activeSelf) return;

        // Force a code detection if the game is paused
        if (Time.timeScale == 0) return;

        Ray l_Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit l_RayCast = new RaycastHit();

        if (Physics.Raycast(l_Ray, out l_RayCast)) {
            if (m_Meet && l_RayCast.collider.tag == "AREATOPUTFOOD")
            {
                m_Meet.transform.position = new Vector3(l_RayCast.point.x, l_RayCast.point.y, 0);
                m_Meet.SetActive(true);
                m_Meet.tag = "SALMON";
            }
        }
    }
}
