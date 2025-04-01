using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebBtn : MonoBehaviour
{
    public string url = "https://www.cju.ac.kr/www/index.do";

    private Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OpenWebPage);
    }

    void OpenWebPage()
    {
        Application.OpenURL(url);
    }
}

