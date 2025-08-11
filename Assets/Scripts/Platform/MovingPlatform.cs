using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    private Transform desPos;

    [SerializeField] private float speed = 1f; // �̵� �ӵ�
    [SerializeField] private float waitTime = 1f; // ��� �ð�
    private bool isWaiting = false; // ��� ������ ����
    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPos.position; // ���� ��ġ�� ����
        desPos = endPos; // ��ǥ ��ġ�� �� ��ġ�� ����
        isWaiting = false; // �ʱ⿡�� ��� ���� �ƴ�
    }

    private void FixedUpdate()
    {
        // ���� ��ġ�� ��ǥ ��ġ ������ �Ÿ��� ���
        float distance = Vector3.Distance(transform.position, desPos.position);
        if(!isWaiting)
        {
            // ��ǥ ��ġ�� �������� �ʾҴٸ� �̵�
            if (distance > 0.01f)
            {
                // ��ǥ ��ġ�� �̵�
                transform.position = Vector3.MoveTowards(transform.position, desPos.position, speed * Time.fixedDeltaTime);
            }
            else
            {
                // ��ǥ ��ġ�� ���������� ��� �ð� �Ŀ� ��ǥ ��ġ�� ����
                StartCoroutine(WaitAndChangeTarget());
            }
        }

    }

    IEnumerator WaitAndChangeTarget()
    {
        isWaiting = true; // ��� ������ ����
        yield return new WaitForSeconds(waitTime); // ��� �ð� ���� ���

        desPos = desPos == endPos ? startPos : endPos; // ��ǥ ��ġ�� ����
        isWaiting = false; // ��� ���� �ƴ����� ����

        yield return null; // ���� ���������� �Ѿ
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� �÷��̾���
        if (collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾ �÷����� �θ�� �����Ͽ� �÷����� �Բ� �̵��ϵ��� ��
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // �浹�� ������ �� �÷��̾ �÷������� �������� �θ� ������ ����
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
