using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetBtn : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => Reset());
    }

    void Reset()
    {
        GameManager.instance.aircraft.Revive();
        GameManager.instance.aircraft.transform.position = GameManager.instance.lastestRunway.transform.GetChild(2).position;
        
        GameManager.instance.currentRunway = GameManager.instance.lastestRunway;

        GameManager.instance.lastestRunway.StartLanding();
    }
}
