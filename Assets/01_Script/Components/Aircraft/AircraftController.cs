using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Rigidbody))]
public class AircraftController : MonoBehaviour
{
    public enum AircraftState
    {
        Flying,
        Landing,
        Takeoff,
    }

    public Action crashCameraAction;
    public Action reviveCameraAction;

    #region Private variables

    public List<AircraftCollider> airPlaneColliders = new List<AircraftCollider>();

    private float maxSpeed = 0.6f;
    private float speedMultiplier;
    private float currentYawSpeed;
    private float currentPitchSpeed;
    private float currentRollSpeed;
    private float currentSpeed;
    private float lastEngineSpeed;

    private bool aircraftIsDead;

    private Rigidbody rb;

    //Input variables
    private float inputH;
    private float inputV;
    private bool inputTurbo;
    private bool inputYawLeft;
    private bool inputYawRight;

    #endregion

    #region Aircraft variables

    public AircraftState aircraftState;

    [Header("Wing trail effects")]
    [Range(0.01f, 1f)]
    [SerializeField] private float trailThickness = 0.045f;
    [SerializeField] private TrailRenderer[] wingTrailEffects;

    [Header("Rotating speeds")]
    [Range(5f, 500f)]
    [SerializeField] private float yawSpeed = 50f;

    [Range(5f, 500f)]
    [SerializeField] private float pitchSpeed = 100f;

    [Range(5f, 500f)]
    [SerializeField] private float rollSpeed = 200f;

    [Header("Rotating speeds multiplers when turbo is used")]
    [Range(0.1f, 5f)]
    [SerializeField] private float yawTurboMultiplier = 0.3f;

    [Range(0.1f, 5f)]
    [SerializeField] private float pitchTurboMultiplier = 0.5f;

    [Range(0.1f, 5f)]
    [SerializeField] private float rollTurboMultiplier = 1f;

    [Header("Moving speed")]
    [Range(5f, 100f)]
    [SerializeField] private float defaultSpeed = 10f;

    [Range(10f, 200f)]
    [SerializeField] private float turboSpeed = 20f;

    [Range(0.1f, 50f)]
    [SerializeField] private float accelerating = 10f;

    [Range(0.1f, 50f)]
    [SerializeField] private float deaccelerating = 5f;

    [Header("Sideway force")]
    [Range(0.1f, 15f)]
    [SerializeField] private float sidewaysMovement = 15f;

    [Range(0.001f, 0.05f)]
    [SerializeField] private float sidewaysMovementXRot = 0.012f;

    [Range(0.1f, 5f)]
    [SerializeField] private float sidewaysMovementYRot = 1.5f;

    [Range(-1, 1f)]
    [SerializeField] private float sidewaysMovementYPos = 0.1f;

    [Header("Engine propellers settings")]
    [Range(10f, 10000f)]
    [SerializeField] private float propelSpeedMultiplier = 100f;

    [SerializeField] private GameObject propeller;

    [Header("Colliders")]
    [SerializeField] private Transform crashCollidersRoot;

    #endregion

    private void Start()
    {
        //Setup speeds
        maxSpeed = defaultSpeed;
        currentSpeed = defaultSpeed;
        ChangeSpeedMultiplier(1f);

        //Get and set rigidbody
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        SetupColliders(crashCollidersRoot);
    }

    private void Update()
    {
        HandleInputs();

        switch (aircraftState)
        {
            case AircraftState.Flying:
                FlyingUpdate();
                break;

            case AircraftState.Landing:
                LandingUpdate();
                break;

            case AircraftState.Takeoff:
                TakeoffUpdate();
                break;
        }
    }

    #region Flying State

    private void FlyingUpdate()
    {
        UpdatePropeller();

        //Airplane move only if not dead
        if (!aircraftIsDead)
        {
            Movement();
            SidewaysForceCalculation();
        }
        else
        {
            ChangeWingTrailEffectThickness(0f);
        }

        //Crash
        if (!aircraftIsDead && HitSometing())
        {
            Crash();
        }
    }

    private void SidewaysForceCalculation()
    {
        float _mutiplierXRot = sidewaysMovement * sidewaysMovementXRot;
        float _mutiplierYRot = sidewaysMovement * sidewaysMovementYRot;

        float _mutiplierYPos = sidewaysMovement * sidewaysMovementYPos;

        //Right side 
        if (transform.localEulerAngles.z > 270f && transform.localEulerAngles.z < 360f)
        {
            float _angle = (transform.localEulerAngles.z - 270f) / (360f - 270f);
            float _invert = 1f - _angle;

            transform.Rotate(Vector3.up * (_invert * _mutiplierYRot) * Time.deltaTime);
            transform.Rotate(Vector3.right * (-_invert * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);

            transform.Translate(transform.up * (_invert * _mutiplierYPos) * Time.deltaTime);
        }

        //Left side
        if (transform.localEulerAngles.z > 0f && transform.localEulerAngles.z < 90f)
        {
            float _angle = transform.localEulerAngles.z / 90f;

            transform.Rotate(-Vector3.up * (_angle * _mutiplierYRot) * Time.deltaTime);
            transform.Rotate(Vector3.right * (-_angle * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);

            transform.Translate(transform.up * (_angle * _mutiplierYPos) * Time.deltaTime);
        }

        //Right side down
        if (transform.localEulerAngles.z > 90f && transform.localEulerAngles.z < 180f)
        {
            float _angle = (transform.localEulerAngles.z - 90f) / (180f - 90f);
            float _invert = 1f - _angle;

            transform.Translate(transform.up * (_invert * _mutiplierYPos) * Time.deltaTime);
            transform.Rotate(Vector3.right * (-_invert * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);
        }

        //Left side down
        if (transform.localEulerAngles.z > 180f && transform.localEulerAngles.z < 270f)
        {
            float _angle = (transform.localEulerAngles.z - 180f) / (270f - 180f);

            transform.Translate(transform.up * (_angle * _mutiplierYPos) * Time.deltaTime);
            transform.Rotate(Vector3.right * (-_angle * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);
        }
    }

    private void Movement()
    {
        //Move forward
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        //Store last speed
        lastEngineSpeed = currentSpeed;

        //Rotate airplane by inputs
        transform.Rotate(Vector3.forward * -inputH * currentRollSpeed * Time.deltaTime);
        transform.Rotate(Vector3.right * inputV * currentPitchSpeed * Time.deltaTime);

        //Rotate yaw
        if (inputYawRight)
        {
            transform.Rotate(Vector3.up * currentYawSpeed * Time.deltaTime);
        }

        if (inputYawLeft)
        {
            transform.Rotate(-Vector3.up * currentYawSpeed * Time.deltaTime);
        }

        //Accelerate and deacclerate
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += accelerating * Time.deltaTime;
        }
        else
        {
            currentSpeed -= deaccelerating * Time.deltaTime;
        }

        //Turbo
        if (inputTurbo)
        {
            //Set speed to turbo speed and rotation to turbo values
            maxSpeed = turboSpeed;

            currentYawSpeed = yawSpeed * yawTurboMultiplier;
            currentPitchSpeed = pitchSpeed * pitchTurboMultiplier;
            currentRollSpeed = rollSpeed * rollTurboMultiplier;

            //Effects
            ChangeWingTrailEffectThickness(trailThickness);
        }
        else
        {
            //Speed and rotation normal
            maxSpeed = defaultSpeed * speedMultiplier;

            currentYawSpeed = yawSpeed;
            currentPitchSpeed = pitchSpeed;
            currentRollSpeed = rollSpeed;

            //Effects
            ChangeWingTrailEffectThickness(0f);
        }
    }

    #endregion

    #region Landing State

    public void AddCurrentRunway(Runway _landingThisRunway)
    {
        GameManager.instance.currentRunway = _landingThisRunway;
    }

    public void AddLatestRunway(Runway _landingThisRunway)
    {
        GameManager.instance.lastestRunway = _landingThisRunway;
    }

    //My trasform is runway landing adjuster child
    private void LandingUpdate()
    {
        UpdatePropeller();

        ChangeWingTrailEffectThickness(0f);

        //Stop speed
        currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime);

        //Set local rotation to zero
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0f, 0f, 0f), 2f * Time.deltaTime);
    }

    #endregion

    #region Takeoff State

    private void TakeoffUpdate()
    {
        UpdatePropeller();

        //Reset colliders
        foreach (AircraftCollider _airPlaneCollider in airPlaneColliders)
        {
            _airPlaneCollider.isCollide = false;
        }

        //Accelerate
        if (currentSpeed < turboSpeed)
        {
            currentSpeed += (accelerating * 2f) * Time.deltaTime;
        }

        //Move forward
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        //Far enough from the runaway go back to flying state
        float _distanceToRunway = Vector3.Distance(transform.position, GameManager.instance.currentRunway.transform.position);
        if (_distanceToRunway > GameManager.instance.currentRunway.GetTakeOffLength())
        {
            GameManager.instance.currentRunway = null;
            aircraftState = AircraftState.Flying;
        }
    }

    #endregion

    #region Private methods

    private void UpdatePropeller()
    {
        if (!aircraftIsDead)
        {
            //Rotate propellers if any
            if (propeller)
            {
                RotatePropeller(propeller, currentSpeed * propelSpeedMultiplier);
            }
        }
        else
        {
            //Rotate propellers if any
            if (propeller)
            {
                RotatePropeller(propeller, 0f);
            }
        }
    }

    private void SetupColliders(Transform _root)
    {
        //Get colliders from root transform
        Collider[] colliders = _root.GetComponentsInChildren<Collider>();

        //If there are colliders put components in them
        for (int i = 0; i < colliders.Length; i++)
        {
            //Change collider to trigger
            colliders[i].isTrigger = true;

            GameObject _currentObject = colliders[i].gameObject;

            //Add airplane collider to it and put it on the list
            AircraftCollider _airplaneCollider = _currentObject.AddComponent<AircraftCollider>();
            airPlaneColliders.Add(_airplaneCollider);
        }
    }

    private void RotatePropeller(GameObject _rotateThing, float _speed)
    {
        _rotateThing.transform.Rotate(Vector3.right * -_speed * Time.deltaTime);
    }

    private void ChangeWingTrailEffectThickness(float _thickness)
    {
        for (int i = 0; i < wingTrailEffects.Length; i++)
        {
            wingTrailEffects[i].startWidth = Mathf.Lerp(wingTrailEffects[i].startWidth, _thickness, Time.deltaTime * 10f);
        }
    }

    private bool HitSometing()
    {
        for (int i = 0; i < airPlaneColliders.Count; i++)
        {
            if (airPlaneColliders[i].isCollide)
            {
                //Reset colliders
                foreach (AircraftCollider _airPlaneCollider in airPlaneColliders)
                {
                    _airPlaneCollider.isCollide = false;
                }
                return true;
            }
        }
        return false;
    }

    #endregion

    #region Public methods

    public void Crash()
    {
        //Invoke action
        crashCameraAction?.Invoke();

        //Set rigidbody to non cinematic
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        //Add last speed to rb
        rb.AddForce(transform.forward * lastEngineSpeed, ForceMode.VelocityChange);

        //Change every collider trigger state and remove rigidbodys
        for (int i = 0; i < airPlaneColliders.Count; i++)
        {
            airPlaneColliders[i].GetComponent<Collider>().isTrigger = false;
        }

        //Kill player
        aircraftIsDead = true;
    }

    public void Revive()
    {
        reviveCameraAction?.Invoke();

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        for (int i = 0; i < airPlaneColliders.Count; i++)
        {
            airPlaneColliders[i].GetComponent<Collider>().isTrigger = true;
        }

        transform.rotation = Quaternion.identity;

        aircraftState = AircraftState.Landing;

        aircraftIsDead = false;
    }

    #endregion

    #region Variables

    public bool GetAircraftDead()
    {
        return aircraftIsDead;
    }

    /// <summary>
    /// With this you can adjust the default speed between one and zero
    /// </summary>
    /// <param name="_speedMultiplier"></param>
    public void ChangeSpeedMultiplier(float _speedMultiplier)
    {
        if (_speedMultiplier < 0f)
        {
            _speedMultiplier = 0f;
        }

        if (_speedMultiplier > 1f)
        {
            _speedMultiplier = 1f;
        }

        speedMultiplier = _speedMultiplier;
    }

    #endregion

    #region Inputs

    private void HandleInputs()
    {
        //Rotate inputs
        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");

        //Yaw axis inputs
        inputYawLeft = Input.GetKey(KeyCode.Q);
        inputYawRight = Input.GetKey(KeyCode.E);

        //Turbo
        inputTurbo = Input.GetKey(KeyCode.LeftShift);
    }

    #endregion
}