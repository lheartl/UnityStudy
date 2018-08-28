using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour,IDamageable {
    public float startingHP;
    protected float hp;
    protected bool dead;

    /**
     *Spawner 클래스에서 확인할수 있도록 이벤트 작성 
     * System.Action 은 델리게이트 메소드 (* 델리게이트 => 다른 메소드의 위치를 가르키고 부러 올수 있는 타입)
     * void를 리턴하고 아무런 입력을 받지 않음.
    */

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        hp = startingHP;
    }
    public void TakeHit(float damage, RaycastHit hit) {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0 && !dead)
        {
            Die();
        }
    }

    protected void Die() {
        dead = true;
        //OnDeath 이벤트가 null 이 아닐 경우 발동
        if (OnDeath != null) {
            OnDeath();
        }
        GameObject.Destroy(gameObject);

    }
}
