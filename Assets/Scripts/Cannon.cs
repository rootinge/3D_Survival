using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cannon : MonoBehaviour, IInteractable
{
    string name = "대포";
    string description = "[F] 키를 눌러 들어가기";
    [SerializeField] Transform barrel;
    [SerializeField] Transform launchPosition;
    public GameObject cannonCamera;

    [Header("조준 설정")]
    public float rotationSpeed = 30f; // 회전 속도
    public float minPitch = -20f; // 최소 피치 각도
    public float maxPitch = 60f; // 최대 피치 각도

    [Header("발사 설정")]
    public float launchForce = 1000f; // 발사 힘

    private PlayerInput cannonInput; // 플레이어 입력 컴포넌트
    private GameObject player;

    private Vector2 aimInput; // 조준 입력
    private float currentPitch; // 현재 피치 각도

    private void Awake()
    {
        cannonInput = GetComponent<PlayerInput>();
        cannonInput.enabled = false;
        cannonCamera.SetActive(false);
        cannonInput.DeactivateInput(); // 대포 입력 비활성화
        currentPitch = launchPosition.rotation.eulerAngles.x;
    }

    string IInteractable.GetInteractPrompt()
    {
        string str = $"{name}\n{description}";
        return str;
    }

    void IInteractable.OnInteract()
    {
        player = CharacterManager.Instance.Player.gameObject; // 플레이어 오브젝트 가져오기
        player.GetComponent<PlayerInput>().enabled = false;
        player.GetComponent<PlayerInput>().DeactivateInput();
        player.SetActive(false); // 플레이어 비활성화
        cannonCamera.SetActive(true); // 대포 카메라 활성화
        cannonInput.enabled = true;
        cannonInput.ActivateInput(); // 대포 입력 활성화
    }

    void Update()
    {
        // 플레이어가 탑승 중일 때
        if (cannonInput.enabled)
        {
            HandleAiming();
        }
    }

    private void HandleAiming()
    {
        // 좌우 회전 (Yaw)
        transform.Rotate(Vector3.up, aimInput.x * rotationSpeed * Time.deltaTime);

        // 상하 회전 (Pitch)
        currentPitch -= aimInput.y * rotationSpeed * Time.deltaTime; // 피치 각도 계산
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch); // 피치 각도를 제한
        barrel.localEulerAngles = new Vector3(currentPitch, 0, 0); // 대포의 피치 각도 설정
    }

    public void OnCannonMovement(InputAction.CallbackContext context)
    {
        // WASD 입력을 Vector2 값으로 계속 받음
        aimInput = context.ReadValue<Vector2>();
    }

    public void OnLaunch(InputAction.CallbackContext context)
    {
        if (!context.performed) return;


        cannonInput.DeactivateInput(); // 대포 입력 비활성화
        cannonInput.enabled = false;

        // 플레이어를 다시 활성화하고 위치시킴
        player.SetActive(true);
        player.transform.position = launchPosition.position;

        // 플레이어의 PlayerInput을 다시 활성화
        PlayerInput playerInputComponent = player.GetComponent<PlayerInput>();
        playerInputComponent.enabled = true;
        playerInputComponent.ActivateInput();
        Vector3 rot = new Vector3(0, barrel.rotation.eulerAngles.y, 0);
        player.transform.rotation = Quaternion.Euler(rot); // 플레이어의 회전 설정
        CharacterManager.Instance.Player.controller.blockMovement = true; // 플레이어 움직임 차단

        // 플레이어 발사
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            
            Debug.Log(launchPosition.up);
            playerRb.velocity = Vector3.zero;
            playerRb.AddForce(launchPosition.forward * launchForce, ForceMode.Impulse);
            
        }

        // 대포는 다시 비활성화 상태로 돌아감
        cannonCamera.SetActive(false);
        
    }
}
  