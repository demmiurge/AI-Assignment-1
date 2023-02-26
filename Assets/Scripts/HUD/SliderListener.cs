using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class SliderListener : MonoBehaviour
{
    private TextMeshProUGUI m_MinText;
    private TextMeshProUGUI m_MaxText;
    private TextMeshProUGUI m_CurrVal;
    private TextMeshProUGUI m_LabelText;
    private Slider m_Slider;

    private object[] m_Pars = new object[1];

    public GameObject m_ListenerObject;
    public string m_ComponentType;
    public string m_FieldName;
    public string m_VisualHUDFieldName = null;

    void AutoFix()
    {
        if (m_MinText)
        {
            //Debug.Log("FOUND MIN VALUE");
        }
        else
        {
            //Debug.LogError("NotFoundMinValue");
        }

        if (m_MaxText)
        {
            //Debug.Log("FOUND MAX VALUE");
        }
        else
        {
            //Debug.LogError("NotFoundMaxValue");
        }

        if (m_CurrVal)
        {
            //Debug.Log("FOUND CURRENT VALUE");
        }
        else
        {
            //Debug.LogError("NotFoundCurrentValue");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        float value;

        m_Slider = this.GetComponent<Slider>();
        m_MinText = transform.Find("MinNumber").GetComponent<TextMeshProUGUI>();
        m_MaxText = transform.Find("MaxNumber").GetComponent<TextMeshProUGUI>();
        m_CurrVal = transform.Find("CurrentValue").GetComponent<TextMeshProUGUI>();
        m_LabelText = transform.Find("Title").GetComponent<TextMeshProUGUI>();

        AutoFix();
        ConvertText();

        m_MinText.text = m_Slider.minValue.ToString();
        m_MaxText.text = m_Slider.maxValue.ToString();

        // By default take SteeringContext as the component's type (retro compatibility)
        if (m_ComponentType == null || m_ComponentType.Length == 0)
            m_ComponentType = "SteeringContext";

        Component component = m_ListenerObject.GetComponent(m_ComponentType);

        Type type = component.GetType();
        FieldInfo field = type.GetField(m_FieldName);

        // in some case the field may be null because its non-existent. Even so,
        // there could be a setter with an "equivalent" name (SetFieldName,,,) 
        if (field != null)
        {
            // if the field exists, initialize slider with its value
            value = (float)field.GetValue(component);
            m_CurrVal.text = value.ToString("0.00");
            m_Slider.value = value;
        }
        else
        {

            // if the field does not exit, use the slider's inital value
            value = m_Slider.value;
            m_CurrVal.text = value.ToString("0.00");
        }

        // let's check if there's a setter for the field. If there's a setter the listener
        // attached to the slider will give priority to  this setter
        string setterName = "Set" + ("" + m_FieldName[0]).ToUpper() + m_FieldName.Substring(1);
        MethodInfo method = type.GetMethod(setterName);
        // the listener will decide...


        m_Slider.onValueChanged.AddListener((x) => {
            m_CurrVal.text = m_Slider.value.ToString("0.00");
            if (method != null)
            {
                // use the setter if possible
                m_Pars[0] = x;
                method.Invoke(component, m_Pars);
            }
            else if (field != null) // if no setter available change the field directly, if it is non-null
                field.SetValue(component, x);
        });
    }

    void ConvertText()
    {
        m_LabelText.text = m_VisualHUDFieldName?.Length > 0 ? m_VisualHUDFieldName : m_FieldName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
