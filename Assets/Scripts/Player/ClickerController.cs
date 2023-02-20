using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickerController : MonoBehaviour
{
    [SerializeField] 
    private GameObject m_Meet;

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
            Clicked();

        if (Input.GetMouseButtonDown(1))
            m_Meet.SetActive(false);
    }

    void Clicked()
    {
        Ray l_Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit l_RayCast = new RaycastHit();

        if (Physics.Raycast(l_Ray, out l_RayCast)) {
            if (m_Meet)
            {
                m_Meet.transform.position = new Vector3(l_RayCast.point.x, l_RayCast.point.y, 0);
                m_Meet.SetActive(true);
            }
        }
    }
}
