using System.Collections;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    float playerSpeed = 20f;
    float mouseX, mouseY;

    // 화면 회전 시 마우스 민감도
    [SerializeField] float sensitivity = 5.0f;
    // 화면 상하회전 최대값 
    [SerializeField] float maxYAngle = 80.0f;

    // 플레이어 시작 회전값
    Quaternion startRotation = Quaternion.Euler(10f, 90f, 0f);
    // 카메라의 시작 회전값
    Quaternion camStartRotation = Quaternion.Euler(-5, 60f, 0f);

    Vector3 playerMoveVec;
    Vector2 currentRotation;

    new Camera camera;

    private void Start()
    {
        currentRotation.x = startRotation.eulerAngles.y;
        currentRotation.y = startRotation.eulerAngles.x;

        transform.rotation = Quaternion.Euler(0, currentRotation.x, 0);

        camera = transform.Find("Camera").GetComponent<Camera>();
        camera.transform.localRotation = camStartRotation;
    }

    void Update()
    {
        Move();         // WASD 움직임
        Rotation();     // 마우스 화면 회전
    }

    void Move()
    {
        // 플레이어 움직임 => 마우스로 회전하여 쳐다보는 방향으로 이동하도록 함
        playerMoveVec = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        transform.position += transform.rotation * playerMoveVec * playerSpeed * Time.deltaTime;

        // Left Shift를 누르면 달리기 상태
        if (Input.GetKey(KeyCode.LeftShift)) playerSpeed = 50f;
        else playerSpeed = 20f;

        if (Input.GetKey(KeyCode.LeftControl)) transform.position += Vector3.down * 10f * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space)) transform.position += Vector3.up * 10f * Time.deltaTime;
    }

    void Rotation()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = -Input.GetAxis("Mouse Y");

        currentRotation.x += mouseX * sensitivity;
        currentRotation.y += mouseY * sensitivity;

        // x축 조정 끝값 설정(화면 상하 회전 => 상하 회전은 플레이어 머리 오브젝트가 따로 없기 때문에 플레이어 회전이 아닌 카메라만 회전)
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
        camera.transform.localRotation = Quaternion.Euler(currentRotation.y, 0, 0);

        // y축 회전(화면 좌우 회전)
        transform.rotation = Quaternion.Euler(0, currentRotation.x, 0);
    }
}
