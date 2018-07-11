using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UserInfomation : MonoBehaviour
{

    public Text m_name;
    public Text m_state;
    public Toggle m_toggle;
    public Action<string, bool> action;

    public void SetData(string name, string state, bool ison, Action<string, bool> action)
    {
        m_name.text = name;
        m_state.text = state;
        m_toggle.isOn = ison;
        if (ison)
        {
            m_toggle.onValueChanged.AddListener(ChangeValue);
            this.action = action;
        }
        else
        {
            m_toggle.interactable = false;
        }
    }

    private void ChangeValue(bool arg0)
    {
        m_toggle.interactable = false;
        if (action != null)
        {
            action(m_name.text, false);
        }
        m_state.text = "不在线";
    }
}
