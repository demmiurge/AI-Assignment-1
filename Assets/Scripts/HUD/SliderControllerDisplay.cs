using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using TMPro;

public class SliderControllerDisplay : MonoBehaviour
{
    public Slider m_Slider;
    public string m_FieldCurrentName;
    public string m_FieldMinName;
    public string m_FieldMaxName;

    public GameObject m_Agent;
    public TextMeshProUGUI m_TextMeshProUGUI;
    private Component m_Blackboard;

    // Start is called before the first frame update
    void Start()
    {
        m_Blackboard = null;

        if (m_Agent)
        {
            Component[] l_ListComponents;
            l_ListComponents = m_Agent.GetComponents(typeof(Component));

            foreach (Component l_Component in l_ListComponents)
            {
                if (l_Component.GetType().ToString().ToLower().Contains("blackboard"))
                    m_Blackboard = l_Component;
            }

            if (!m_Blackboard) return;

            m_Slider.maxValue = GetMax();
            m_Slider.minValue = GetMin();
            m_Slider.value = GetCurrent();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Agent)
        {
            float l_Perc;

            m_Slider.value = GetCurrent();

            if (GetMin() >= 0)
                l_Perc = ((m_Slider.value - GetMin()) * 100) / GetMax();
            else
                l_Perc = (m_Slider.value * 100) / GetMax();

            m_TextMeshProUGUI.text = $"{l_Perc:0}%";
        }
    }

    float GetMin()
    {
        if (!string.IsNullOrEmpty(m_FieldMinName) && GetValueFromNameByBlackboard(m_FieldMinName) >= 0)
            return GetValueFromNameByBlackboard(m_FieldMinName);
        return 0;
    }

    float GetMax()
    {
        if (!string.IsNullOrEmpty(m_FieldMaxName) && GetValueFromNameByBlackboard(m_FieldMaxName) >= 0)
            return GetValueFromNameByBlackboard(m_FieldMaxName);
        return 100;
    }

    float GetCurrent()
    {
        return GetValueFromNameByBlackboard(m_FieldCurrentName);
    }

    float GetValueFromNameByBlackboard(string p_Field)
    {
        Type l_Type;
        FieldInfo l_Field;

        l_Type = m_Blackboard.GetType();

        if (l_Type.GetField(p_Field) != null)
            l_Field = l_Type.GetField(p_Field);
        else
            return -1.0f;

        return (float)l_Field.GetValue(m_Blackboard);
    }
}
