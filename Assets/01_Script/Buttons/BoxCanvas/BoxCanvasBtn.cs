using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxCanvasBtn : MonoBehaviour
{
    TextMeshProUGUI btnTxt;

    bool isOn = false;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btnTxt = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        btn.onClick.AddListener(() => OnOff());
    }

    void OnOff()
    {
        if (isOn)
        {
            btnTxt.text = "¢º";
            transform.parent.GetComponent<RectTransform>().position += Vector3.left * 550f;
            isOn = false;
        }
        else
        {
            btnTxt.text = "¢¸";
            transform.parent.GetComponent<RectTransform>().position += Vector3.right * 550f;
            isOn = true;
        }
    }
}
