using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SharkHungerDisplay : MonoBehaviour
{
    public Slider m_Slider;

    public GameObject m_SharkAgent;

    public float m_TestHungry = 100;

    // Start is called before the first frame update
    void Start()
    {
        if (m_SharkAgent || true)
        {
            m_Slider.maxValue = GetMaxHunger();
            m_Slider.minValue = GetMinHunger();
            m_Slider.value = GetFullHunger();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_SharkAgent || true)
        {
            m_Slider.value = GetFullHunger();
        }
    }

    float GetMinHunger()
    {
        return 0;
    }

    float GetMaxHunger()
    {
        return 100;
    }

    float GetFullHunger()
    {
        return m_TestHungry;
    }
}
