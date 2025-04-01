using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitBtn : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => Exit());
    }

    void Exit()
    {
        GameManager.instance.FlightMode();

        // ������ UI ĵ���� Ȱ��ȭ
        UIManager.instance.FlightUIControl(true);

        // ������� UI ĵ���� ��Ȱ��ȭ
        UIManager.instance.ViewingUIControl(false);
    }
}
