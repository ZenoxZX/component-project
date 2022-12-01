using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthComponent : IHealthComponent
{
    public float Health { get; private set; }
    public float MaxHealth { get; private set; }
    public string HealthString => Health.ToString("F0");
    public string MaxHealthString => MaxHealth.ToString("F0");
    public State GetState => Health > 0 ? State.Alive : State.Dead;
    public bool IsWounded => Health > 0 && Health < MaxHealth;
    public event Action OnTakeDamage, OnHeal, OnDeath, OnRevive;
    public event Action<HealthChangeArgs> OnHealthChanged;
    private readonly bool useDebug;

    public HealthComponent(float maxHealth, float? health = null, bool useDebug = false, Action<HealthChangeArgs> OnHealthChanged = null, Action OnTakeDamage = null, Action OnHeal = null, Action OnDeath = null, Action OnRevive = null)
    {
        MaxHealth = maxHealth;
        Health = health ?? maxHealth;
        if (OnHealthChanged != null) this.OnHealthChanged += OnHealthChanged;
        if (OnTakeDamage != null) this.OnTakeDamage += OnTakeDamage;
        if (OnHeal != null) this.OnHeal += OnHeal;
        if (OnDeath != null) this.OnDeath += OnDeath;
        if (OnRevive != null) this.OnRevive += OnRevive;

        this.useDebug = useDebug;
    }

    public void SetMaxHealth(float maxHealth, bool storeOldHealth = true, float? health = null)
    {
        MaxHealth = maxHealth;
        if (!storeOldHealth) Health = Math.Min(health ?? maxHealth, maxHealth);
        else Health = Math.Min(Health, MaxHealth);
        OnHealthChanged?.Invoke(new HealthChangeArgs { health = Health, maxHealth = MaxHealth });
    }

    public void TakeDamage(float value) 
    {
        if (GetState == State.Alive)
        {
            Health -= value;
            OnTakeDamage?.Invoke();

            if (Health <= 0)
            {
                Health = 0;
                OnDeath?.Invoke();
            }

            OnHealthChanged?.Invoke(new HealthChangeArgs { health = Health, maxHealth = MaxHealth });
        }

        else
        {
            LocalDebugger("Your are death you cant take damage!");
        }
    }

    public void Heal(float value)
    {
        if (GetState == State.Alive)
        {
            if (IsWounded)
            {
                Health = Math.Min(Health + value, MaxHealth);
                OnHeal?.Invoke();
                OnHealthChanged?.Invoke(new HealthChangeArgs { health = Health, maxHealth = MaxHealth });

                LocalDebugger($"[{value}] healed. Your health is [{Health}]");
            }

            else
            {
                LocalDebugger("Your health is already at Max Health");
            }
        }

        else
        {
            LocalDebugger("Your are death you cant heal!");
        }
    }

    public void Revive(float? health = null)
    {
        if (GetState == State.Dead)
        {
            Health =  health ?? MaxHealth;
            Health = Math.Min(Health, MaxHealth);
            OnRevive?.Invoke();
            OnHealthChanged(new HealthChangeArgs { health = Health, maxHealth = MaxHealth });
        }

        else LocalDebugger("Your are alive you cant revive!");

    }

    private void LocalDebugger(object message) { if (useDebug) Debug.Log(message); }
    public enum State { Alive, Dead }
}

public struct HealthChangeArgs
{
    public float health;
    public float maxHealth;
    public float HealthRatio => health / maxHealth;
    public string HealthString => health.ToString("F0");
    public string MaxHealthString => maxHealth.ToString("F0");
}

public interface IHealthComponentCallbacks
{
    public void OnTakeDamage();
    public void OnHeal();
    public void OnDeath();
    public void OnRevive();
    public void OnHealthChanged(HealthChangeArgs args);
}

public interface IHealthComponent
{
    public float Health { get; }
    public float MaxHealth { get; }
    public string HealthString => Health.ToString("F0");
    public string MaxHealthString => MaxHealth.ToString("F0");
    public HealthComponent.State GetState { get; }
    public bool IsWounded => Health > 0 && Health < MaxHealth;
    public void TakeDamage(float value);
    public void Heal(float value);
}