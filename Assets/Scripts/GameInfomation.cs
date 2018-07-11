using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class GameInfomation : MonoBehaviour
{
    public Text GameId;
    public InputField GamePrice;
    public Button m_Button;
    public Toggle m_Toggle;

    private UnityAction<string, string, bool> m_action;

    public void SetData(string gamename, PriceData gamePrice, UnityAction<string, string, bool> action)
    {
        GameId.text = gamename.ToString();
        GamePrice.text = gamePrice.price.ToString();
        m_Toggle.isOn = gamePrice.canplay;
        if (gamePrice.canplay)
        {
            GamePrice.interactable = true;
        }
        else
        {
            GamePrice.interactable = false;
        }
        GamePrice.onEndEdit.AddListener(ChangeText);
        m_action += action;
        m_Button.onClick.AddListener(ClickButton);
        m_Toggle.onValueChanged.AddListener(ChangeValue);
    }

    private void ChangeValue(bool arg0)
    {
        if (arg0)
        {
            GamePrice.interactable = true;
        }
        else
        {
            GamePrice.interactable = false;
            m_action(GameId.text, GamePrice.text, false);
        }
    }

    private void ChangeText(string arg0)
    {
        if (m_action != null)
        {
            m_action(GameId.text, arg0, true);
        }
    }
    public

    void ClickButton()
    {
        /*GamePrice.Select()*/
        ;
    }



}
