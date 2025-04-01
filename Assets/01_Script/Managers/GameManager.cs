using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public AircraftController aircraft;
    public DroneMovement drone;

    public Runway lastestRunway;    // 마지막 착륙 Runway
    public Runway currentRunway;    // 현재 착륙 Runway

    public Vector3 dronePosition;
    Vector3 droneY = Vector3.up * 5f;

    private void Awake()
    {
        instance = this;

        drone.gameObject.SetActive(false);
        aircraft = FindObjectOfType<AircraftController>();

        
    }

    private void Start()
    {
        Init();
    }

    #region ("Init")

    private void Init()
    {
        aircraft.transform.SetParent(lastestRunway.transform.GetChild(1));
        aircraft.transform.position = lastestRunway.transform.GetChild(2).position;
        aircraft.aircraftState = AircraftController.AircraftState.Landing;

        dronePosition = currentRunway.transform.position + droneY;

        lastestRunway.StartLanding();
    }

    #endregion

    #region "ChangeMode"

    // 관람모드
    public void ViewingMode()
    {
        drone.gameObject.SetActive(true);
        aircraft.gameObject.SetActive(false);

        drone.transform.position = dronePosition + droneY;
    }

    // 비행모드
    public void FlightMode()
    {
        // 드론 비활성화 & 비행기 활성화
        drone.gameObject.SetActive(false);
        aircraft.gameObject.SetActive(true);
    }

    #endregion
}
