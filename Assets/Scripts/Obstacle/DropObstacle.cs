using UnityEngine;

public class DropObstacle : MonoBehaviour
{
    [SerializeField] int destroytime = 5; // ������Ʈ ���� �ð�
    [SerializeField] int damage = 5;      // �÷��̾�� �� ���ط�
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
