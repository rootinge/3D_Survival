using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    private Transform desPos;

    [SerializeField] private float speed = 1f; // 이동 속도
    [SerializeField] private float waitTime = 1f; // 대기 시간
    private bool isWaiting = false; // 대기 중인지 여부
    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPos.position; // 시작 위치로 설정
        desPos = endPos; // 목표 위치를 끝 위치로 설정
        isWaiting = false; // 초기에는 대기 중이 아님
    }

    private void FixedUpdate()
    {
        // 현재 위치와 목표 위치 사이의 거리를 계산
        float distance = Vector3.Distance(transform.position, desPos.position);
        if(!isWaiting)
        {
            // 목표 위치에 도달하지 않았다면 이동
            if (distance > 0.01f)
            {
                // 목표 위치로 이동
                transform.position = Vector3.MoveTowards(transform.position, desPos.position, speed * Time.fixedDeltaTime);
            }
            else
            {
                // 목표 위치에 도달했으면 대기 시간 후에 목표 위치를 변경
                StartCoroutine(WaitAndChangeTarget());
            }
        }

    }

    IEnumerator WaitAndChangeTarget()
    {
        isWaiting = true; // 대기 중으로 설정
        yield return new WaitForSeconds(waitTime); // 대기 시간 동안 대기

        desPos = desPos == endPos ? startPos : endPos; // 목표 위치를 변경
        isWaiting = false; // 대기 중이 아님으로 설정

        yield return null; // 다음 프레임으로 넘어감
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 플레이어라면
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어를 플랫폼에 부모로 설정하여 플랫폼과 함께 이동하도록 함
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // 충돌이 끝났을 때 플레이어가 플랫폼에서 떨어지면 부모 설정을 해제
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
