using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenoxZX.HealthSystem;

public class Test : MonoBehaviour, IHealthComponentCallbacks, IDamageable, IHealable
{
    public HealthComponent HealthComponent { get; private set; }

    private void Start()
    {
        HealthComponent = new HealthComponent(100, 100, useDebug: true, AttachedGo: gameObject);
        IDamageable damageable = this;
        IHealable healable = this;

        damageable.TakeDamage(5);
        healable.Heal(6);
        healable.OverHeal(1);
        this.HealthComponent.Invulnerable = true;
        damageable.TakeDamage(5);
        damageable.Kill();
    }

    public void OnDeath()
    {
        throw new System.NotImplementedException();
    }

    public void OnHeal(float value)
    {
        throw new System.NotImplementedException();
    }

    public void OnHealthChanged(HealthChangeArgs args)
    {
        throw new System.NotImplementedException();
    }

    public void OnRevive()
    {
        throw new System.NotImplementedException();
    }

    public void OnTakeDamage(float value)
    {
        throw new System.NotImplementedException();
    }
}
