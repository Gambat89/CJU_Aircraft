using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    public Transform[] cameraPosObject;

    enum cameraMod { aircraft, pilot };

    cameraMod camMod;

    private void Start()
    {
        camMod = 0;
    }

    private void Update()
    {
        switchCam();

        switch (camMod)
        {
            case cameraMod.aircraft:
                transform.position = cameraPosObject[0].position;
                break;

            case cameraMod.pilot:
                transform.position = cameraPosObject[1].position;
                break;
        }
    }

    private void switchCam()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) camMod = cameraMod.aircraft;
        if (Input.GetKeyDown(KeyCode.Alpha2)) camMod = cameraMod.pilot;
    }
}
