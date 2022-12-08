using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZenoxZX.HealthSystem;

public class HealthSystem : MonoBehaviour, IDamageable, IHealable
{
    public HealthComponent HealthComponent { get; private set; }
    public DrawType drawType = DrawType.EditOnInspector;
    private IDamageable damageable;
    private IHealable healable;
    public HealthConfig_SO healthConfig_SO;
    public HealthConfig healthConfig;
    [HideInInspector] public bool B_OnDeath, B_OnRevive, B_OnTakeDamage, B_OnHeal, B_OnRestoreArmor, B_OnHealthChange;

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
        healthConfig = drawType == DrawType.GetFromConfig && healthConfig_SO != null ? healthConfig_SO.healthConfig : healthConfig;
        float? startHP = healthConfig.startFullHP ? (float?)null : healthConfig.startHP;
        HealthComponent = new HealthComponent(healthConfig.maxHealth,
                                              healthConfig.maxArmor,
                                              startHP,
                                              healthConfig.startArmor,
                                              healthConfig.damageType,
                                              healthConfig.dynamicDamageRatio,
                                              healthConfig.useDebug,
                                              gameObject,
                                              E_OnHealthChange,
                                              E_OnTakeDamage,
                                              E_OnHeal,
                                              E_OnRestoreArmor,
                                              E_OnDeath,
                                              E_OnRevive
                                              );
        damageable = this;
        healable = this;
    }
}
