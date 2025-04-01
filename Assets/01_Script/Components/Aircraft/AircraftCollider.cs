using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftCollider : MonoBehaviour
{
    public bool isCollide;

    private void OnTriggerEnter(Collider other)
    {
        //Collide someting bad
        if (other.CompareTag("Ground") || other.CompareTag("Building"))
        {
            isCollide = true;
        }
    }
}