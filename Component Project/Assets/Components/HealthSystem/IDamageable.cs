using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public HealthComponent HealthComponent { get; }
    public void TakeDamage(float value) => HealthComponent.TakeDamage(value);
    public HealthComponent.State GetState => HealthComponent.GetState;
    public bool IsWounded => HealthComponent.IsWounded;
    public bool IsDamageable => HealthComponent.GetState == HealthComponent.State.Alive;
    public bool IsDead => HealthComponent.GetState == HealthComponent.State.Dead;
    public float HealthRatio => HealthComponent.HealthRatio;
}
