using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingPlatform : MonoBehaviour
{
    [SerializeField] float jumpForce = 10f; // 점프 힘

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 플레이어인지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            
            // 플레이어의 Rigidbody 컴포넌트를 가져옴
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                // 점프 힘을 적용
                playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }
}
