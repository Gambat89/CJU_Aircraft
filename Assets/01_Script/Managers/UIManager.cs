using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject flightUI;  // 기본 UI
    public GameObject viewingUI; // 관람모드 UI

    public GameObject viewingBtn; // 관람모드 Btn

    public GameObject contentBox;

    public GameObject flightHelpBox;   // 비행모드 조작법 UI
    public GameObject droneHelpBox;    // 관람모드 조작법 UI

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        HelpUIControl();
    }

    #region "UIControl"

    void HelpUIControl()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            flightHelpBox.SetActive(!flightHelpBox.activeSelf);
            droneHelpBox.SetActive(!droneHelpBox.activeSelf);
        }
    }

    public void ViewingUIControl(bool OnOff)
    {
        viewingUI.SetActive(OnOff);
    }

    public void FlightUIControl(bool OnOff)
    {
        flightUI.SetActive(OnOff);
    }

    #endregion
}
