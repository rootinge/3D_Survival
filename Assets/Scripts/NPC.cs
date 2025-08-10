using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle, // NPC�� �ƹ��͵� ���� �ʴ� ����
    Wandering, // NPC�� �������� ���ƴٴϴ� ����
    Attacking // NPC�� �÷��̾ �����ϴ� ����
}

public class NPC : MonoBehaviour, IDamagalbe
{
    [Header("Stats")]
    public int health; // NPC�� ü��
    public float walkSpeed; // NPC�� �ȴ� �ӵ�
    public float runSpeed; // NPC�� �޸��� �ӵ�
    public ItemData[] dropOnDeath; // NPC�� �׾��� �� ����ϴ� �����۵�

    [Header("AI")]
    private NavMeshAgent agent; // NavMesh ������Ʈ
    public float detectDistance; // NPC�� �÷��̾ ������ �� �ִ� �Ÿ�
    private AIState aiState; // ���� AI ����

    [Header("Wandering")]
    public float minWanderDistance; // NPC�� ���ƴٴ� �ּ� �Ÿ�
    public float maxWanderDistance; // NPC�� ���ƴٴ� �ִ� �Ÿ�
    public float minWanferWaitTime; // NPC�� ���ƴٴ� �ּ� ��� �ð�
    public float maxWanderWaitTime; // NPC�� ���ƴٴ� �ִ� ��� �ð�

    [Header("Combat")]
    public int damage; // NPC�� �÷��̾�� �ִ� ���ط�
    public float attackRate; // NPC�� ���� �ӵ�
    private float lastAttackTime; // ������ ���� �ð�
    public float attackDistance; // NPC�� �÷��̾�� ������ �� �ִ� �Ÿ�

    private float playerDistance; // NPC�� �÷��̾� ������ �Ÿ�

    public float fieldOfView = 120f; // NPC�� �þ߰�

    private Animator animator; // NPC�� �ִϸ�����
    private SkinnedMeshRenderer[] meshRenderers; // NPC�� �޽� �������� 

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    void Start()
    {
        SetState(AIState.Wandering); // �ʱ� ���¸� Wandering���� ����
    }


    void Update()
    {
        // �÷��̾���� �Ÿ��� ���
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);

        // �����̴� ���¿� ���� �ִϸ��̼� ���� ������Ʈ
        animator.SetBool("Moving", aiState != AIState.Idle);


        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;
        }

    }

    public void SetState(AIState state)
    {
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                agent.speed = walkSpeed; // Idle ���¿����� �ȴ� �ӵ��� ����
                agent.isStopped = true; // NavMesh ������Ʈ�� ����
                break;
            case AIState.Wandering:
                agent.speed = walkSpeed; // Wandering ���¿����� �ȴ� �ӵ��� ���� 
                agent.isStopped = false; // NavMesh ������Ʈ�� Ȱ��ȭ
                break;
            case AIState.Attacking:
                agent.speed = runSpeed; // Attacking ���¿����� �޸��� �ӵ��� ����
                agent.isStopped = false; // NavMesh ������Ʈ�� Ȱ��ȭ
                break;
        }

        animator.speed = agent.speed / walkSpeed; // �ִϸ����� �ӵ��� NavMesh ������Ʈ �ӵ��� ����
    }

    // NPC�� Idle �Ǵ� Wandering ������ �� ȣ��Ǵ� �޼���
    void PassiveUpdate()
    {
        // ���� �Ÿ��� 0.1f ������ ��� Idle ���·� ��ȯ
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle); // Wandering ���¿��� �������� �����ϸ� Idle ���·� ��ȯ

            // ���� �ð� �Ŀ� ���ο� ��ġ�� �̵�
            Invoke("WanderToNewLocation", Random.Range(minWanferWaitTime, maxWanderWaitTime));
        }

        if (playerDistance < detectDistance)
        {
            SetState(AIState.Attacking); // �÷��̾ ���� ���� ���� ���� ��� Attacking ���·� ��ȯ
        }
    }


    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return; // Idle ���°� �ƴ� ��� ����

        SetState(AIState.Wandering); // Wandering ���·� ��ȯ
        agent.SetDestination(GetWanderLocation());
    }

    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        int i = 0;
        do
        {
            // NavMesh���� ������ ��ġ�� ���ø��Ͽ� NPC�� ���ƴٴ� ��ġ�� ����
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30) break;

        } while (Vector3.Distance(transform.position, hit.position) < detectDistance);

        return hit.position; // ���ø��� ��ġ ��ȯ
    }

    // NPC�� Attacking ������ �� ȣ��Ǵ� �޼���
    void AttackingUpdate()
    {
        // �÷��̾���� �Ÿ��� ���� �Ÿ����� ������ �þ߿� ���� �� ����
        if (playerDistance < attackDistance && IsPlayerInFieldOfView())
        {
            agent.isStopped = true; // NavMesh ������Ʈ�� ����

            if (Time.time - lastAttackTime > attackRate) // ���� �ӵ��� ���� ����
            {
                lastAttackTime = Time.time; // ������ ���� �ð� ������Ʈ

                // �÷��̾�� ���ظ� ��
                CharacterManager.Instance.Player.controller.GetComponent<IDamagalbe>().TakePhysicalDamage(damage);
                animator.speed = 1f; // �ִϸ����� �ӵ��� �⺻������ ����
                animator.SetTrigger("Attack"); // ���� �ִϸ��̼� Ʈ����
            }
        }
        else
        {
            if (playerDistance < detectDistance)
            {
                agent.isStopped = false; // �÷��̾ ���� ���� ���� ������ ���� �Ÿ��� ��� ��� NavMesh ������Ʈ�� Ȱ��ȭ
                NavMeshPath path = new NavMeshPath(); // NavMesh ��θ� ����
                // NPC�� ������ ��ġ���� Ȯ���ϰ� ��� ���
                if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
                {
                   agent.SetDestination(CharacterManager.Instance.Player.transform.position); // �÷��̾��� ��ġ�� �̵�
                }
                else
                {
                    agent.SetDestination(transform.position); // �÷��̾ ���� ������ ����� NPC�� ����
                    agent.isStopped = true; // NavMesh ������Ʈ�� ����
                    SetState(AIState.Wandering); // Wandering ���·� ��ȯ
                }
            }
            else
            {
                agent.SetDestination(transform.position); // �÷��̾ ���� ������ ����� NPC�� ����
                agent.isStopped = true; // NavMesh ������Ʈ�� ����
                SetState(AIState.Wandering); // Wandering ���·� ��ȯ
            }
        }
    }

    // �÷��̾ NPC�� �þ߿� �ִ��� Ȯ���ϴ� �޼���
    bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position; // �÷��̾� ���� ����
        float angle = Vector3.Angle(transform.forward, directionToPlayer); // NPC�� �� ����� �÷��̾� ���� ������ ����
        return angle < fieldOfView * 0.5f; // �þ߰��� ���ݺ��� ������ �÷��̾ �þ߿� ����
    }

    public void TakePhysicalDamage(int damage)
    {
        health -= damage; // NPC�� ü�� ����
        if (health <= 0)
        {
            Die(); // ü���� 0 ���ϰ� �Ǹ� ���� ó��
        }

        StartCoroutine(DamageFlash()); // ���ظ� �޾��� �� DamageFlash �ڷ�ƾ ȣ��
    }

    void Die()
    {
        for(int i = 0; i < dropOnDeath.Length; i++)
        {
            Instantiate(dropOnDeath[i].dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity); // ���� �� ��� ������ ����

        }
        Destroy(gameObject); // NPC ������Ʈ ����
    }

    IEnumerator DamageFlash()
    {
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.red; // ���ظ� �޾��� �� ���������� ����
        }

        yield return new WaitForSeconds(0.1f); // 0.1�� ���

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.white; // �ٽ� ������� ����
        }
    }

}
