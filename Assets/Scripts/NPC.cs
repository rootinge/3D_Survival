using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle, // NPC가 아무것도 하지 않는 상태
    Wandering, // NPC가 무작위로 돌아다니는 상태
    Attacking // NPC가 플레이어를 공격하는 상태
}

public class NPC : MonoBehaviour, IDamagalbe
{
    [Header("Stats")]
    public int health; // NPC의 체력
    public float walkSpeed; // NPC의 걷는 속도
    public float runSpeed; // NPC의 달리는 속도
    public ItemData[] dropOnDeath; // NPC가 죽었을 때 드랍하는 아이템들

    [Header("AI")]
    private NavMeshAgent agent; // NavMesh 에이전트
    public float detectDistance; // NPC가 플레이어를 감지할 수 있는 거리
    private AIState aiState; // 현재 AI 상태

    [Header("Wandering")]
    public float minWanderDistance; // NPC가 돌아다닐 최소 거리
    public float maxWanderDistance; // NPC가 돌아다닐 최대 거리
    public float minWanferWaitTime; // NPC가 돌아다닐 최소 대기 시간
    public float maxWanderWaitTime; // NPC가 돌아다닐 최대 대기 시간

    [Header("Combat")]
    public int damage; // NPC가 플레이어에게 주는 피해량
    public float attackRate; // NPC의 공격 속도
    private float lastAttackTime; // 마지막 공격 시간
    public float attackDistance; // NPC가 플레이어에게 공격할 수 있는 거리

    private float playerDistance; // NPC와 플레이어 사이의 거리

    public float fieldOfView = 120f; // NPC의 시야각

    private Animator animator; // NPC의 애니메이터
    private SkinnedMeshRenderer[] meshRenderers; // NPC의 메쉬 렌더러들 

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    void Start()
    {
        SetState(AIState.Wandering); // 초기 상태를 Wandering으로 설정
    }


    void Update()
    {
        // 플레이어와의 거리를 계산
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);

        // 움직이는 상태에 따라 애니메이션 상태 업데이트
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
                agent.speed = walkSpeed; // Idle 상태에서는 걷는 속도로 설정
                agent.isStopped = true; // NavMesh 에이전트를 멈춤
                break;
            case AIState.Wandering:
                agent.speed = walkSpeed; // Wandering 상태에서는 걷는 속도로 설정 
                agent.isStopped = false; // NavMesh 에이전트를 활성화
                break;
            case AIState.Attacking:
                agent.speed = runSpeed; // Attacking 상태에서는 달리는 속도로 설정
                agent.isStopped = false; // NavMesh 에이전트를 활성화
                break;
        }

        animator.speed = agent.speed / walkSpeed; // 애니메이터 속도를 NavMesh 에이전트 속도에 맞춤
    }

    // NPC가 Idle 또는 Wandering 상태일 때 호출되는 메서드
    void PassiveUpdate()
    {
        // 남은 거리가 0.1f 이하인 경우 Idle 상태로 전환
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle); // Wandering 상태에서 목적지에 도달하면 Idle 상태로 전환

            // 일정 시간 후에 새로운 위치로 이동
            Invoke("WanderToNewLocation", Random.Range(minWanferWaitTime, maxWanderWaitTime));
        }

        if (playerDistance < detectDistance)
        {
            SetState(AIState.Attacking); // 플레이어가 감지 범위 내에 있을 경우 Attacking 상태로 전환
        }
    }


    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return; // Idle 상태가 아닐 경우 무시

        SetState(AIState.Wandering); // Wandering 상태로 전환
        agent.SetDestination(GetWanderLocation());
    }

    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        int i = 0;
        do
        {
            // NavMesh에서 무작위 위치를 샘플링하여 NPC가 돌아다닐 위치를 결정
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30) break;

        } while (Vector3.Distance(transform.position, hit.position) < detectDistance);

        return hit.position; // 샘플링된 위치 반환
    }

    // NPC가 Attacking 상태일 때 호출되는 메서드
    void AttackingUpdate()
    {
        // 플레이어와의 거리가 공격 거리보다 가깝고 시야에 있을 때 공격
        if (playerDistance < attackDistance && IsPlayerInFieldOfView())
        {
            agent.isStopped = true; // NavMesh 에이전트를 멈춤

            if (Time.time - lastAttackTime > attackRate) // 공격 속도에 따라 공격
            {
                lastAttackTime = Time.time; // 마지막 공격 시간 업데이트

                // 플레이어에게 피해를 줌
                CharacterManager.Instance.Player.controller.GetComponent<IDamagalbe>().TakePhysicalDamage(damage);
                animator.speed = 1f; // 애니메이터 속도를 기본값으로 설정
                animator.SetTrigger("Attack"); // 공격 애니메이션 트리거
            }
        }
        else
        {
            if (playerDistance < detectDistance)
            {
                agent.isStopped = false; // 플레이어가 감지 범위 내에 있지만 공격 거리를 벗어난 경우 NavMesh 에이전트를 활성화
                NavMeshPath path = new NavMeshPath(); // NavMesh 경로를 생성
                // NPC가 못가는 위치인지 확인하고 경로 계산
                if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
                {
                   agent.SetDestination(CharacterManager.Instance.Player.transform.position); // 플레이어의 위치로 이동
                }
                else
                {
                    agent.SetDestination(transform.position); // 플레이어가 감지 범위를 벗어나면 NPC를 멈춤
                    agent.isStopped = true; // NavMesh 에이전트를 멈춤
                    SetState(AIState.Wandering); // Wandering 상태로 전환
                }
            }
            else
            {
                agent.SetDestination(transform.position); // 플레이어가 감지 범위를 벗어나면 NPC를 멈춤
                agent.isStopped = true; // NavMesh 에이전트를 멈춤
                SetState(AIState.Wandering); // Wandering 상태로 전환
            }
        }
    }

    // 플레이어가 NPC의 시야에 있는지 확인하는 메서드
    bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position; // 플레이어 방향 벡터
        float angle = Vector3.Angle(transform.forward, directionToPlayer); // NPC의 앞 방향과 플레이어 방향 사이의 각도
        return angle < fieldOfView * 0.5f; // 시야각의 절반보다 작으면 플레이어가 시야에 있음
    }

    public void TakePhysicalDamage(int damage)
    {
        health -= damage; // NPC의 체력 감소
        if (health <= 0)
        {
            Die(); // 체력이 0 이하가 되면 죽음 처리
        }

        StartCoroutine(DamageFlash()); // 피해를 받았을 때 DamageFlash 코루틴 호출
    }

    void Die()
    {
        for(int i = 0; i < dropOnDeath.Length; i++)
        {
            Instantiate(dropOnDeath[i].dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity); // 죽을 때 드랍 아이템 생성

        }
        Destroy(gameObject); // NPC 오브젝트 제거
    }

    IEnumerator DamageFlash()
    {
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.red; // 피해를 받았을 때 빨간색으로 변경
        }

        yield return new WaitForSeconds(0.1f); // 0.1초 대기

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = Color.white; // 다시 흰색으로 변경
        }
    }

}
