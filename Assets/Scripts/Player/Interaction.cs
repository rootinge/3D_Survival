using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f; // ��ȣ�ۿ� üũ �ֱ�
    private float lastCheckTime; // ������ üũ �ð�
    public float maxCheckDistance; // �ִ� ��ȣ�ۿ� �Ÿ�
    public LayerMask layerMask; // ��ȣ�ۿ� ������ ���̾� ����ũ

    public GameObject curInteractGameObjcet; // ���� ��ȣ�ۿ� ������ ���� ������Ʈ
    private IInteractable curInteractable; // ���� ��ȣ�ۿ� ������ �������̽�

    public TextMeshProUGUI promptText; // ��ȣ�ۿ� ������Ʈ �ؽ�Ʈ UI
    private Camera camera; // ī�޶� ����

    void Start()
    {
        camera = Camera.main; // ���� ī�޶� ������
    }


    void Update()
    {
        if(Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time; // ������ üũ �ð� ������Ʈ

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // ȭ�� �߾ӿ��� Ray ����
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObjcet)
                {
                    // ���� ��ȣ�ۿ� ������ ���� ������Ʈ ������Ʈ
                    curInteractGameObjcet = hit.collider.gameObject;
                    // IInteractable �������̽��� ���� ������Ʈ ��������
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText(); // ������Ʈ �ؽ�Ʈ ����
                }
            }
            else
            {
                curInteractable = null; // Ray�� �ƹ��͵� ������ ������ ��
                curInteractGameObjcet = null; // ���� ��ȣ�ۿ� ������ ���� ������Ʈ �ʱ�ȭ
                promptText.gameObject.SetActive(false); // ������Ʈ �ؽ�Ʈ ��Ȱ��ȭ
            }
        }

    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true); // ������Ʈ �ؽ�Ʈ Ȱ��ȭ
        promptText.text = curInteractable.GetInteractPrompt(); // ������Ʈ �ؽ�Ʈ ����
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && curInteractable !=null)
        {
            curInteractable.OnInteract(); // ��ȣ�ۿ� �޼��� ȣ��
            curInteractGameObjcet = null; // ���� ��ȣ�ۿ� ������ ���� ������Ʈ �ʱ�ȭ
            curInteractable = null; // ���� ��ȣ�ۿ� ������ �������̽� �ʱ�ȭ
            promptText.gameObject.SetActive(false); // ��ȣ�ۿ� �� ������Ʈ �ؽ�Ʈ ��Ȱ��ȭ
        }
    }
}
