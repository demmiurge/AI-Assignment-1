using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationFix : MonoBehaviour
{
    public GameObject m_ObjetToRotate;
    public GameObject m_ThisObject;

    // Start is called before the first frame update
    void Start()
    {
        //m_ObjetToRotate = GetComponentInChildren<GameObject>();
        m_ObjetToRotate = GameObject.FindGameObjectWithTag("MODEL");
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        if(m_ThisObject.transform.rotation.z > 90f)
        {
            m_ObjetToRotate.transform.rotation = Quaternion.Euler(-180, 180, 0);
        }
        else if(m_ThisObject.transform.rotation.z < -90)
        {
            m_ObjetToRotate.transform.rotation = Quaternion.Euler(180, 180, 0);
        }
    }
}
