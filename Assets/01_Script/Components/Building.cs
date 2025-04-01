using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public GameObject buildingDescription;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Aircraft"))
        {
            UIManager.instance.contentBox.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 440);
            buildingDescription.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Aircraft"))
        {
            UIManager.instance.contentBox.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 440);
            buildingDescription.SetActive(false);
        }
    }
}
