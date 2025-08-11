using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cannon : MonoBehaviour, IInteractable
{
    string name = "����";
    string description = "[F] Ű�� ���� ����";
    [SerializeField] Transform barrel;
    [SerializeField] Transform launchPosition;
    public GameObject cannonCamera;

    [Header("���� ����")]
    public float rotationSpeed = 30f; // ȸ�� �ӵ�
    public float minPitch = -20f; // �ּ� ��ġ ����
    public float maxPitch = 60f; // �ִ� ��ġ ����

    [Header("�߻� ����")]
    public float launchForce = 1000f; // �߻� ��

    private PlayerInput cannonInput; // �÷��̾� �Է� ������Ʈ
    private GameObject player;

    private Vector2 aimInput; // ���� �Է�
    private float currentPitch; // ���� ��ġ ����

    private void Awake()
    {
        cannonInput = GetComponent<PlayerInput>();
        cannonInput.enabled = false;
        cannonCamera.SetActive(false);
        cannonInput.DeactivateInput(); // ���� �Է� ��Ȱ��ȭ
        currentPitch = launchPosition.rotation.eulerAngles.x;
    }

    string IInteractable.GetInteractPrompt()
    {
        string str = $"{name}\n{description}";
        return str;
    }

    void IInteractable.OnInteract()
    {
        player = CharacterManager.Instance.Player.gameObject; // �÷��̾� ������Ʈ ��������
        player.GetComponent<PlayerInput>().enabled = false;
        player.GetComponent<PlayerInput>().DeactivateInput();
        player.SetActive(false); // �÷��̾� ��Ȱ��ȭ
        cannonCamera.SetActive(true); // ���� ī�޶� Ȱ��ȭ
        cannonInput.enabled = true;
        cannonInput.ActivateInput(); // ���� �Է� Ȱ��ȭ
    }

    void Update()
    {
        // �÷��̾ ž�� ���� ��
        if (cannonInput.enabled)
        {
            HandleAiming();
        }
    }

    private void HandleAiming()
    {
        // �¿� ȸ�� (Yaw)
        transform.Rotate(Vector3.up, aimInput.x * rotationSpeed * Time.deltaTime);

        // ���� ȸ�� (Pitch)
        currentPitch -= aimInput.y * rotationSpeed * Time.deltaTime; // ��ġ ���� ���
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch); // ��ġ ������ ����
        barrel.localEulerAngles = new Vector3(currentPitch, 0, 0); // ������ ��ġ ���� ����
    }

    public void OnCannonMovement(InputAction.CallbackContext context)
    {
        // WASD �Է��� Vector2 ������ ��� ����
        aimInput = context.ReadValue<Vector2>();
    }

    public void OnLaunch(InputAction.CallbackContext context)
    {
        if (!context.performed) return;


        cannonInput.DeactivateInput(); // ���� �Է� ��Ȱ��ȭ
        cannonInput.enabled = false;

        // �÷��̾ �ٽ� Ȱ��ȭ�ϰ� ��ġ��Ŵ
        player.SetActive(true);
        player.transform.position = launchPosition.position;

        // �÷��̾��� PlayerInput�� �ٽ� Ȱ��ȭ
        PlayerInput playerInputComponent = player.GetComponent<PlayerInput>();
        playerInputComponent.enabled = true;
        playerInputComponent.ActivateInput();
        Vector3 rot = new Vector3(0, barrel.rotation.eulerAngles.y, 0);
        player.transform.rotation = Quaternion.Euler(rot); // �÷��̾��� ȸ�� ����
        CharacterManager.Instance.Player.controller.blockMovement = true; // �÷��̾� ������ ����

        // �÷��̾� �߻�
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            
            Debug.Log(launchPosition.up);
            playerRb.velocity = Vector3.zero;
            playerRb.AddForce(launchPosition.forward * launchForce, ForceMode.Impulse);
            
        }

        // ������ �ٽ� ��Ȱ��ȭ ���·� ���ư�
        cannonCamera.SetActive(false);
        
    }
}
  