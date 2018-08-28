
using UnityEngine;


public interface IDamageable {

    void TakeHit(float dameage,RaycastHit hit);

    void TakeDamage(float dameage);
}