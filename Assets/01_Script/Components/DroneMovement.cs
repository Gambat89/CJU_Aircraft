using System.Collections;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    float playerSpeed = 20f;
    float mouseX, mouseY;

    // ȭ�� ȸ�� �� ���콺 �ΰ���
    [SerializeField] float sensitivity = 5.0f;
    // ȭ�� ����ȸ�� �ִ밪 
    [SerializeField] float maxYAngle = 80.0f;

    // �÷��̾� ���� ȸ����
    Quaternion startRotation = Quaternion.Euler(10f, 90f, 0f);
    // ī�޶��� ���� ȸ����
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
        Move();         // WASD ������
        Rotation();     // ���콺 ȭ�� ȸ��
    }

    void Move()
    {
        // �÷��̾� ������ => ���콺�� ȸ���Ͽ� �Ĵٺ��� �������� �̵��ϵ��� ��
        playerMoveVec = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        transform.position += transform.rotation * playerMoveVec * playerSpeed * Time.deltaTime;

        // Left Shift�� ������ �޸��� ����
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

        // x�� ���� ���� ����(ȭ�� ���� ȸ�� => ���� ȸ���� �÷��̾� �Ӹ� ������Ʈ�� ���� ���� ������ �÷��̾� ȸ���� �ƴ� ī�޶� ȸ��)
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
        camera.transform.localRotation = Quaternion.Euler(currentRotation.y, 0, 0);

        // y�� ȸ��(ȭ�� �¿� ȸ��)
        transform.rotation = Quaternion.Euler(0, currentRotation.x, 0);
    }
}
