using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealable
{
    public HealthComponent HealthComponent { get; }
    public void Heal(float value) => HealthComponent.Heal(value);
    public void OverHeal(float value) => HealthComponent.OverHeal(value);
    public void Revive(float? value = null) => HealthComponent.Revive(value);

    public HealthComponent.State GetState => HealthComponent.GetState;
    public bool IsWounded => HealthComponent.IsWounded;
    public bool IsDead => HealthComponent.GetState == HealthComponent.State.Dead;
    public float HealthRatio => HealthComponent.HealthRatio;
}
