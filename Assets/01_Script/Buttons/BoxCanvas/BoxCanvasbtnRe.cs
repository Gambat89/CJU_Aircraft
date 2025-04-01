using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxCanvasbtnRe : MonoBehaviour
{
    TextMeshProUGUI btnTxt;

    bool isOff = true;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btnTxt = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        btn.onClick.AddListener(() => OnOff());
    }

    void OnOff()
    {
        if (isOff)
        {
            btnTxt.text = "¢º";
            transform.parent.GetComponent<RectTransform>().position += Vector3.left * 500f;
            isOff = false;
        }
        else
        {
            btnTxt.text = "¢¸";
            transform.parent.GetComponent<RectTransform>().position += Vector3.right * 500f;
            isOff = true;
        }
    }
}
