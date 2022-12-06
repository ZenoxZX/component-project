using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZenoxZX.HealthSystem;

public class HealthSystem : MonoBehaviour, IDamageable, IHealable
{
    public HealthComponent HealthComponent { get; private set; }
    private IDamageable damageable;
    private IHealable healable;

    #region FIELDS AND PROPERTIES

    [SerializeField] DamageType damageType = DamageType.HealthOnly;
    [SerializeField] float maxHealth = 100, maxArmor = 100;
    [SerializeField] bool startFullHP = true;
    [SerializeField] float startHP = 0, startArmor = 0;
    [SerializeField] bool useCustomDynamicDamageRatio = false;
    [SerializeField] DynamicDamageRatio dynamicDamageRatio = DynamicDamageRatio.Default;
    [SerializeField] bool useDebug = false;

    #endregion

    #region UNITY EVENTS

    public UnityEvent OnDeath, OnRevive;
    public UnityEvent<float> OnTakeDamage, OnHeal, OnRestoreArmor;
    public UnityEvent<HealthChangeArgs> OnHealthChange;
    private void E_OnHealthChange(HealthChangeArgs args) => OnHealthChange?.Invoke(args);
    private void E_OnTakeDamage(float value) => OnTakeDamage?.Invoke(value);
    private void E_OnHeal(float value) => OnHeal?.Invoke(value);
    private void E_OnRestoreArmor(float value) => OnRestoreArmor?.Invoke(value);
    private void E_OnDeath() => OnDeath?.Invoke();
    private void E_OnRevive() => OnRevive?.Invoke();

    #endregion

    private void Start()
    {
        float? startHP = startFullHP ? (float?)null : this.startHP;
        HealthComponent = new HealthComponent(maxHealth, maxArmor, startHP, startArmor, damageType, dynamicDamageRatio, useDebug, gameObject, E_OnHealthChange, E_OnTakeDamage, E_OnHeal, E_OnRestoreArmor, E_OnDeath, E_OnRevive);
        damageable = this;
        healable = this;
    }
}
