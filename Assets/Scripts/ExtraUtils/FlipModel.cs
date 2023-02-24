using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipModel : MonoBehaviour
{
    public GameObject m_Model;
    Vector3 m_OriginalScale;

    // Start is called before the first frame update
    void Start()
    {
        m_OriginalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        float l_RotationZ = transform.rotation.eulerAngles.z;
        if (l_RotationZ >= 90 && l_RotationZ <= 270)
            m_Model.transform.localScale = ChangeScaleToFlip(-m_OriginalScale.y);
        else
            m_Model.transform.localScale = ChangeScaleToFlip(m_OriginalScale.y);
    }

    Vector3 ChangeScaleToFlip(float scaleY)
    {
        return new Vector3(m_OriginalScale.x, scaleY, m_OriginalScale.z);
    }
}
