using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AircraftCamera : MonoBehaviour
{
    private CinemachineBrain brain;

    [Header("References")]
    [SerializeField] private CinemachineFreeLook freeLook;

    [Header("Camera values")]
    [SerializeField] private float cameraDefaultFov = 60f;
    [SerializeField] private float cameraTurboFov = 40f;

    private void OnEnable()
    {
        GameManager.instance.aircraft.crashCameraAction += Crash;
        GameManager.instance.aircraft.reviveCameraAction += Revive;
    }

    private void OnDisable()
    {
        GameManager.instance.aircraft.crashCameraAction -= Crash;
        GameManager.instance.aircraft.reviveCameraAction -= Revive;
    }

    private void Start()
    {
        brain = GetComponent<CinemachineBrain>();
    }

    private void Update()
    {
        CameraFovUpdate();
    }

    private void CameraFovUpdate()
    {
        //Turbo
        if (!GameManager.instance.aircraft.GetAircraftDead() && GameManager.instance.aircraft.aircraftState == AircraftController.AircraftState.Flying)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                ChangeCameraFov(cameraTurboFov);
            }
            else
            {
                ChangeCameraFov(cameraDefaultFov);
            }
        }
        else
        {
            ChangeCameraFov(cameraDefaultFov);
        }
    }

    public void ChangeCameraFov(float _fov)
    {
        float _deltatime = Time.deltaTime * 100f;
        freeLook.m_Lens.FieldOfView = Mathf.Lerp(freeLook.m_Lens.FieldOfView, _fov, 0.05f * _deltatime);
    }

    private void Crash()
    {
        //Change update method after crash
        brain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
    }

    private void Revive()
    {
        brain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.LateUpdate;
    }
}