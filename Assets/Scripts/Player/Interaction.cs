using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f; // 상호작용 체크 주기
    private float lastCheckTime; // 마지막 체크 시간
    public float maxCheckDistance; // 최대 상호작용 거리
    public LayerMask layerMask; // 상호작용 가능한 레이어 마스크

    public GameObject curInteractGameObjcet; // 현재 상호작용 가능한 게임 오브젝트
    private IInteractable curInteractable; // 현재 상호작용 가능한 인터페이스

    public TextMeshProUGUI promptText; // 상호작용 프롬프트 텍스트 UI
    private Camera camera; // 카메라 참조

    void Start()
    {
        camera = Camera.main; // 메인 카메라를 가져옴
    }


    void Update()
    {
        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time; // 마지막 체크 시간 업데이트

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // 화면 중앙에서 Ray 생성
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObjcet)
                {
                    // 현재 상호작용 가능한 게임 오브젝트 업데이트
                    curInteractGameObjcet = hit.collider.gameObject;
                    // IInteractable 인터페이스를 가진 컴포넌트 가져오기
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText(); // 프롬프트 텍스트 설정
                }
            }
            else
            {
                curInteractable = null; // Ray가 아무것도 맞추지 못했을 때
                curInteractGameObjcet = null; // 현재 상호작용 가능한 게임 오브젝트 초기화
                promptText.gameObject.SetActive(false); // 프롬프트 텍스트 비활성화
            }
        }

    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true); // 프롬프트 텍스트 활성화
        promptText.text = curInteractable.GetInteractPrompt(); // 프롬프트 텍스트 설정
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && curInteractable !=null)
        {
            curInteractable.OnInteract(); // 상호작용 메서드 호출
            curInteractGameObjcet = null; // 현재 상호작용 가능한 게임 오브젝트 초기화
            curInteractable = null; // 현재 상호작용 가능한 인터페이스 초기화
            promptText.gameObject.SetActive(false); // 상호작용 후 프롬프트 텍스트 비활성화
        }
    }
}
