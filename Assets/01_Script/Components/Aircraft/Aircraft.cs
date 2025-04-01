using System.Security;
using UnityEngine;

public class Aircraft : MonoBehaviour
{
    //개발자 입력 정보
    public Aircraft_Info info;

    //추력 조절 변수
    float thrustThrottle;
    //추력
    Vector3 thrustVec;
    //디버그용 벡터 배열
    Vector3[] DebugForces = new Vector3[3];

    Forces force;

    void FixedUpdate()
    {
        AddForcesToWings();
    }

    void AddForcesToWings()
    {
        // 비행기 방향 조절
        Quaternion InputSteerAngle = Quaternion.Euler(20 * Input.GetAxis("Pitch") / (1 + info.rigid.velocity.magnitude * 0.1f), 20 * Input.GetAxis("Yaw") / (1 + info.rigid.velocity.magnitude * 0.5f), 20 * Input.GetAxis("Roll") / (1 + info.rigid.velocity.magnitude * 0.1f));
        force = AerodynamicsForce(out DebugForces, InputSteerAngle, info.rigid, transform, info.airDensity, info.wingArea, info.wingLength, info.windRange);

        foreach (WheelCollider wheel in info.aircraftWheel) 
        { 
            wheel.motorTorque = thrustVec.magnitude * 0.01f; 
            wheel.brakeTorque = Input.GetAxis("DragPositive") * 2000;
            wheel.steerAngle = 20 * Input.GetAxis("Yaw");
        }

        //추력
        thrustThrottle = Mathf.Clamp(thrustThrottle + Input.GetAxis("Thrust"), 0, 100); //추력 조절
        thrustVec = transform.forward * thrustThrottle * info.ThrustScalar; //추력 연산 결과 (기체 앞방향을 바라보는 벡터 * 추력 조절 수치 * 추력 입력 정보)

        info.rigid.AddForce(force.Force + (info.rigid.velocity.magnitude >= 500 ? Vector3.zero : thrustVec));
        info.rigid.AddTorque(force.Torque);

        info.rigid.angularDrag = 1 + info.rigid.velocity.magnitude * 0.001f;
    }

    Forces AerodynamicsForce(out Vector3[] AerodynamicsForces, Quaternion steerAngle, Rigidbody rigid, Transform transform, float airDensity, float wingArea, float wingLength, float windRange)
    {
        Vector3 Lift; //양력
        Vector3 Drag; //항력
        Vector3 Moment; //공기역학적 모멘트

        //공기역학적 힘 연산 유체 정보
        Vector3 wind = Vector3.one * Random.Range(-windRange, windRange); //유체 속도 변동
        Vector3 airFlowVelocity = -rigid.velocity - Vector3.Cross(rigid.angularVelocity, transform.position - rigid.worldCenterOfMass) + wind; //유체 상대 속도 (-기체 속도 - 각속도와 질량 중심부터 회전 중심까지의 위치 벡터의 외적 (중심점을 기준으로한 등속 원운동 속도 (w×r)) + 유체 속도 변동)
        Vector3 localAirFlow = transform.InverseTransformDirection(airFlowVelocity); //로컬 좌표 기준 유체 상대 속도
        Vector3 airFlowDirection = transform.TransformDirection(localAirFlow.normalized); //월드 좌표 기준 유체 진행 방향

        float angleOfAttack = Mathf.Atan2(localAirFlow.y, -localAirFlow.z) * Mathf.Rad2Deg; //받음각
        float airMomentum = airDensity * localAirFlow.sqrMagnitude * wingArea; //공기역학적 힘에 영향을 주는 유체 운동량 (p_air = mv = ρv²∆tS) (∆t는 이후 미분시 약분되므로 생략)

        coefficient aerodynamicsCoe = new coefficient(angleOfAttack); //받음각에 따른 공기역학적 힘의 계수
        coefficient aerodynamicsCoe_Cos = new coefficient(90 - angleOfAttack); //코사인 기준 계수

        //양력
        Vector3 LiftDirection = Vector3.Cross(airFlowDirection, -transform.right); //양력 방향
        Lift = LiftDirection * 0.5f * aerodynamicsCoe.liftCoefficient * airMomentum; //양력 연산 결과

        //항력
        Vector3 DragDirection = airFlowDirection; //항력 방향
        Drag = DragDirection * 0.5f * aerodynamicsCoe.dragCoefficient * airMomentum + DragDirection * 0.5f * aerodynamicsCoe_Cos.dragCoefficient * airMomentum * Input.GetAxis("DragPositive") * 0.07f; // 항력 연산 결과

        //공기역학적 모멘트
        Moment = (-transform.right * new coefficient(steerAngle.x).momentCoefficient + -transform.up * new coefficient(steerAngle.y).momentCoefficient + transform.forward * new coefficient(steerAngle.z).momentCoefficient) * 0.5f * airMomentum * wingLength; //공기역학적 모멘트 연산 결과

        AerodynamicsForces = new Vector3[3] { Lift, Drag, Moment }; //공기역학적 힘 연산 결과 저장
        return new Forces(Lift + Drag, Moment); //연산 결과 반환
    }
}

[System.Serializable]
public struct Aircraft_Info
{
    [Header("UserControl")]
    public float ThrustScalar; //추력 스칼라값 입력
    public float windRange; //유체 속도 변동 범위

    [Header("Aerodynamics Parameter")]
    public float airDensity; //유체 밀도
    public float wingArea; //날개 넓이
    public float wingLength; //날개 길이

    [Header("Component")]
    public Rigidbody rigid;
    public Camera camera;
    public WheelCollider[] aircraftWheel;
}

public struct Forces
{
    public Vector3 Force; public Vector3 Torque;

    public Forces(Vector3 InputForce, Vector3 InputTorque)
    {
        Force = InputForce; Torque = InputTorque;
    }
}

public struct coefficient
{
    public float liftCoefficient; public float dragCoefficient; public float momentCoefficient;

    public coefficient(float angleOfAttack)
    {
        float AOA_Rad = angleOfAttack * Mathf.Deg2Rad;

        liftCoefficient = 0.8f * Mathf.Sin(2 * AOA_Rad);
        dragCoefficient = 0.8f * Mathf.Sin(2 * AOA_Rad - Mathf.PI * 0.5f) + 0.8f;
        momentCoefficient = -0.6f * Mathf.Sin(AOA_Rad * 0.5f);
    }
}

public enum ForceType
{
    Lift,
    Drag,
    Moment
}