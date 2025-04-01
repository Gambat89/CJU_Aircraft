using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailBtn : MonoBehaviour
{
    public GameObject detail;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => Detail());
    }

    void Detail()
    {
        if (detail != null)
        {
            detail.SetActive(!detail.activeSelf);
        }
    }
}
