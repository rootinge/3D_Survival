using System.Collections;
using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;  // 인스펙터에서 연결
    [SerializeField] private Transform firePoint;        // 레이저가 시작될 위치 (총구 등)
    [SerializeField] private float maxDistance = 100f;   // 레이저 최대 거리
    [SerializeField] private GameObject warningUI;       // 레이저 경고 UI
    [SerializeField] private GameObject obstaclePrefab;  // 장애물 프리팹
    [SerializeField] private int obstacleNum = 10;            // 장애물 개수
    [SerializeField] private float Range = 5f;            // 장애물 개수
    [SerializeField] private float obstaclePosY = 10;            // 장애물 개수

    private bool isWarningActive = false;                // 경고 활성화 상태

    private void Start()
    {
        isWarningActive = false;
        warningUI.SetActive(false);
    }
    private void FixedUpdate()
    {
        if (isWarningActive) return;
        ShootLaser();
    }

    void ShootLaser()
    {
        RaycastHit hit;
        Vector3 startPos = firePoint.position; 
        Vector3 direction = firePoint.up;

        // 레이캐스트로 레이저가 맞는 위치 찾기
        if (Physics.Raycast(startPos, direction, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
            Debug.Log("Hit: " + hit.collider.CompareTag("Player"));
                // 경고 UI 활성화
                warningUI.SetActive(true);
                isWarningActive = true;
                lineRenderer.enabled = false;
                StopAllCoroutines();
                StartCoroutine(ObstacleDrop(hit.collider.gameObject.transform.position));
            }
            else
            {
                // 레이저가 맞은 위치까지 선을 그림
                DrawLaser(startPos, hit.point);
            }
        }
        else
        {
            // 최대 거리까지 선을 그림
            DrawLaser(startPos, startPos + direction * maxDistance);
        }
    }

    void DrawLaser(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // 레이저 선이 잠깐만 보이게 하려면 코루틴 사용
        StopAllCoroutines();
        StartCoroutine(HideLaserRoutine());
    }

    IEnumerator HideLaserRoutine()
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f); // 0.05초 동안만 레이저 보임
        lineRenderer.enabled = false;
    }

    IEnumerator ObstacleDrop(Vector3 pos)
    {
        for(int i = 0; i < obstacleNum; i++)
        {
            // 장애물 프리팹을 생성
            Instantiate(obstaclePrefab, pos + new Vector3(Random.Range(-Range, Range), obstaclePosY, Random.Range(-Range, Range)), Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
        isWarningActive = false;
    }
}

