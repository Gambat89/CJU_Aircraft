using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingArea : MonoBehaviour
{
    [SerializeField] private Runway runway;

    private void OnTriggerEnter(Collider other)
    {
        //Check if colliding object has airplane collider component
        if (other.CompareTag("Aircraft"))
        {
            //Calculate that the plane is coming from the right direction
            Vector3 dirFromLandingAreaToPlayerPlane = (transform.position - other.transform.position).normalized;
            float _directionFloat = Vector3.Dot(transform.forward, dirFromLandingAreaToPlayerPlane);

            //If direction is right start landing
            if (_directionFloat > 0.5f)
            {
                runway.GetLandingAdjust().position = GameManager.instance.aircraft.transform.position;
                runway.StartLanding();

                GameManager.instance.aircraft.aircraftState = AircraftController.AircraftState.Landing;
                GameManager.instance.aircraft.AddCurrentRunway(runway);
                GameManager.instance.aircraft.AddLatestRunway(runway);
                GameManager.instance.dronePosition = transform.parent.position;

                UIManager.instance.viewingBtn.SetActive(true);
            }
        }
    }
}