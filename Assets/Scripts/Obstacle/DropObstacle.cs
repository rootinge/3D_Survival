using UnityEngine;

public class DropObstacle : MonoBehaviour
{
    [SerializeField] int destroytime = 5; // 오브젝트 제거 시간
    [SerializeField] int damage = 5;      // 플레이어에게 줄 피해량
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Invoke("DestroySelf", destroytime); 
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && rb.velocity.y < -0.1f)
        {
            collision.gameObject.GetComponent<PlayerCondition>().TakePhysicalDamage(damage);
            Invoke("DestroySelf", 0.5f);
        }
    }
}
