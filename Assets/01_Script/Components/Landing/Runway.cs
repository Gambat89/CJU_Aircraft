using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runway : MonoBehaviour
{
    private bool landing;
    private bool landingCompleted;

    private Vector3 landingAdjusterStartLocalPos;

    private KeyCode launchKey = KeyCode.Space;

    [Header("Runway references")]

    private LandingArea landingArea;
    private Transform landingAdjuster;
    private Transform landingFinalPos;

    [Header("Runway Info")]

    private float landingSpeed = 1f;

    [Header("Takeoff settings")]

    [SerializeField] [Range(10f, 50f)]
    private float takeOffLength = 30f;

    private void Awake()
    {
        // Set Runway references
        landingArea = transform.GetChild(0).GetComponent<LandingArea>();
        landingAdjuster = transform.GetChild(1);
        landingFinalPos = transform.GetChild(2);

        landingAdjusterStartLocalPos = landingAdjuster.localPosition;
    }

    private void Update()
    {
        //Airplane is landing (Landing area add airplane controller reference)
        if (landing)
        {
            //Move landing adjuster to landing final pos position
            if (!landingCompleted)
            {
                landingSpeed += Time.deltaTime;
                landingAdjuster.localPosition = Vector3.Lerp(landingAdjuster.localPosition, landingFinalPos.localPosition, landingSpeed * Time.deltaTime);

                float _distanceToLandingFinalPos = Vector3.Distance(landingAdjuster.position, landingFinalPos.position);
                if (_distanceToLandingFinalPos < 0.1f)
                {
                    landingCompleted = true;
                }
            }
            else
            {
                landingAdjuster.localPosition = Vector3.Lerp(landingAdjuster.localPosition, landingFinalPos.localPosition, landingSpeed * Time.deltaTime);

                //Launch airplane
                if (Input.GetKeyDown(launchKey) && GameManager.instance.aircraft.aircraftState != AircraftController.AircraftState.Flying)
                {
                    GameManager.instance.aircraft.aircraftState = AircraftController.AircraftState.Takeoff;
                    UIManager.instance.viewingBtn.SetActive(false);
                }

                //Reset runway if landing airplane is taking off
                if (GameManager.instance.aircraft.aircraftState == AircraftController.AircraftState.Flying)
                {
                    GameManager.instance.aircraft.transform.SetParent(null);
                    landing = false;
                    landingCompleted = false;
                    landingSpeed = 1f;
                    landingAdjuster.localPosition = landingAdjusterStartLocalPos;
                }
            }
        }
    }

    //Landing area add airplane controller reference
    public void StartLanding()
    {
        landing = true;

        //Set airplane to landing adjuster child
        GameManager.instance.aircraft.transform.SetParent(landingAdjuster.transform);

    }

    public float GetTakeOffLength()
    {
        return takeOffLength;
    }

    public Transform GetLandingAdjust()
    {
        return landingAdjuster;
    }
}