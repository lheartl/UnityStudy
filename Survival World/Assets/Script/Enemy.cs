using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State {
        Idle,Chasing,Attacking
    }
    State currentState;
    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;

    Material skinMaterial;
    Color originalColour;
    //공격 딜레이시간
    float timeBetweenAttacks = 1;
    //다음번 공격 가능 시간
    float nextAttackTime;
    //공격 한계 거리
    float attackDistanceThreshold = 0.5f;

    float damage = 1;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;
    // Use this for initialization
    protected override void Start () {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null) {
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            skinMaterial = GetComponent<Renderer>().material;
            originalColour = skinMaterial.color;

            currentState = State.Chasing;
            StartCoroutine(UpdatePath());
        }
      
    }

    void OnTargetDeath() {
        hasTarget = false;

        currentState = State.Idle;
    }
	// Update is called once per frame
	void Update () {
        if (hasTarget) {
            //공격 가능 시간일 경우에만 작동
            if (Time.time > nextAttackTime)
            {
                //목표 거리의 제곱
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;

                //목표물과의 거리가 공격 한계거리보다 작을 경우
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
       
       
    }

    IEnumerator Attack(){
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        //목표 대상 - 현재 위치 한다음 normalized 하면 방향 벡터값을 가져옴
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        //대상의 위치에 - 방향벡터 * 대상과 자신의 반지름
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius);
        float percent = 0;
        float attackSpeed = 3;

        skinMaterial.color = Color.red;

        bool hasAppliedDamage = false;

        while (percent <=1) {

            if (percent>= 0.5f && !hasAppliedDamage) {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            //interpolation ( 보간) 값 - 알려진 점들의 위치를 참조하여 집합의 일정 범위의 선을 새롭게 그리는 방법
            // 공식 : y = 4(-x^2 +x)
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            //Lerp 메소드는 두 벡터 사이에 비례값 ( 0에서 1사이) 을 반환
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            //return 윗부분 까지 실행이 되고 멈춘 다음 Update 함수가 종료 되면 아래부분 실행 ( Update 에서 코루틴을 실행했기 때문)
            yield return null;
        }

        pathfinder.enabled = true;
        currentState = State.Chasing;
        skinMaterial.color = originalColour;
    }

    IEnumerator UpdatePath() {
        float refreshRate = .25f;

        while (hasTarget) {
            if (currentState == State.Chasing) {
                //목표 대상 - 현재 위치 한다음 normalized 하면 방향 벡터값을 가져옴
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                //대상의 위치에 - 방향벡터 * 대상과 자신의 반지름
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius+attackDistanceThreshold/2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
