using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewingButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => Viewing());
    }

    void Viewing()
    {
        GameManager.instance.ViewingMode();

        UIManager.instance.ViewingUIControl(true);
        UIManager.instance.FlightUIControl(false);
    }
}
