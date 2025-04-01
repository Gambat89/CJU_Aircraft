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

        // 비행모드 UI 캔버스 활성화
        UIManager.instance.FlightUIControl(true);

        // 관람모드 UI 캔버스 비활성화
        UIManager.instance.ViewingUIControl(false);
    }
}
