using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public AircraftController aircraft;
    public DroneMovement drone;

    public Runway lastestRunway;    // ������ ���� Runway
    public Runway currentRunway;    // ���� ���� Runway

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

    // �������
    public void ViewingMode()
    {
        drone.gameObject.SetActive(true);
        aircraft.gameObject.SetActive(false);

        drone.transform.position = dronePosition + droneY;
    }

    // ������
    public void FlightMode()
    {
        // ��� ��Ȱ��ȭ & ����� Ȱ��ȭ
        drone.gameObject.SetActive(false);
        aircraft.gameObject.SetActive(true);
    }

    #endregion
}
