using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject flightUI;  // �⺻ UI
    public GameObject viewingUI; // ������� UI

    public GameObject viewingBtn; // ������� Btn

    public GameObject contentBox;

    public GameObject flightHelpBox;   // ������ ���۹� UI
    public GameObject droneHelpBox;    // ������� ���۹� UI

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
