using System.Collections;
using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;  // �ν����Ϳ��� ����
    [SerializeField] private Transform firePoint;        // �������� ���۵� ��ġ (�ѱ� ��)
    [SerializeField] private float maxDistance = 100f;   // ������ �ִ� �Ÿ�
    [SerializeField] private GameObject warningUI;       // ������ ��� UI
    [SerializeField] private GameObject obstaclePrefab;  // ��ֹ� ������
    [SerializeField] private int obstacleNum = 10;            // ��ֹ� ����
    [SerializeField] private float Range = 5f;            // ��ֹ� ����
    [SerializeField] private float obstaclePosY = 10;            // ��ֹ� ����

    private bool isWarningActive = false;                // ��� Ȱ��ȭ ����

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

        // ����ĳ��Ʈ�� �������� �´� ��ġ ã��
        if (Physics.Raycast(startPos, direction, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
            Debug.Log("Hit: " + hit.collider.CompareTag("Player"));
                // ��� UI Ȱ��ȭ
                warningUI.SetActive(true);
                isWarningActive = true;
                lineRenderer.enabled = false;
                StopAllCoroutines();
                StartCoroutine(ObstacleDrop(hit.collider.gameObject.transform.position));
            }
            else
            {
                // �������� ���� ��ġ���� ���� �׸�
                DrawLaser(startPos, hit.point);
            }
        }
        else
        {
            // �ִ� �Ÿ����� ���� �׸�
            DrawLaser(startPos, startPos + direction * maxDistance);
        }
    }

    void DrawLaser(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // ������ ���� ��� ���̰� �Ϸ��� �ڷ�ƾ ���
        StopAllCoroutines();
        StartCoroutine(HideLaserRoutine());
    }

    IEnumerator HideLaserRoutine()
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f); // 0.05�� ���ȸ� ������ ����
        lineRenderer.enabled = false;
    }

    IEnumerator ObstacleDrop(Vector3 pos)
    {
        for(int i = 0; i < obstacleNum; i++)
        {
            // ��ֹ� �������� ����
            Instantiate(obstaclePrefab, pos + new Vector3(Random.Range(-Range, Range), obstaclePosY, Random.Range(-Range, Range)), Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
        isWarningActive = false;
    }
}

