using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthComponent
{
    public float Health { get; private set; }
    public float MaxHealth { get; private set; }
    public bool Invulnerable { get; set; } = false;
    public event Action OnDeath, OnRevive;
    public event Action<float> OnTakeDamage, OnHeal;
    public event Action<HealthChangeArgs> OnHealthChanged;
    private readonly bool useDebug;
    private GameObject attachedGo;

    public HealthComponent(float maxHealth, float? health = null, bool useDebug = false, Action<HealthChangeArgs> OnHealthChanged = null, Action<float> OnTakeDamage = null, Action<float> OnHeal = null, Action OnDeath = null, Action OnRevive = null, GameObject AttachedGo = null)
    {
        MaxHealth = maxHealth;
        Health = health ?? maxHealth;
        if (OnHealthChanged != null) this.OnHealthChanged += OnHealthChanged;
        if (OnTakeDamage != null) this.OnTakeDamage += OnTakeDamage;
        if (OnHeal != null) this.OnHeal += OnHeal;
        if (OnDeath != null) this.OnDeath += OnDeath;
        if (OnRevive != null) this.OnRevive += OnRevive;

        this.useDebug = useDebug;
        this.attachedGo = AttachedGo;
    }

    public string HealthString => Health.ToString("F0");
    public string MaxHealthString => MaxHealth.ToString("F0");
    public State GetState => Health > 0 ? State.Alive : State.Dead;
    public bool IsWounded => Health > 0 && Health < MaxHealth;
    public float HealthRatio => Health / MaxHealth;

    public void SetMaxHealth(float maxHealth, bool storeOldHealth = true, float? health = null)
    {
        MaxHealth = maxHealth;
        if (!storeOldHealth) Health = Math.Min(health ?? maxHealth, maxHealth);
        else Health = Math.Min(Health, MaxHealth);
        OnHealthChanged?.Invoke(new HealthChangeArgs(Health, MaxHealth));
        LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name}'s" : "Your")} max health changed to {maxHealth}!");
    }

    public void Kill()
    {
        if (Invulnerable) { LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} invulnerable, cant die!"); return; }
        Health = 0;
        OnDeath?.Invoke();
        OnHealthChanged?.Invoke(new HealthChangeArgs(Health, MaxHealth));
        LocalDebugger($"{(attachedGo != null ? attachedGo.name : "You")} died!");
    }

    public void TakeDamage(float value) 
    {
        if (Invulnerable) { LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} invulnerable, cant take damage!"); return; }
        if (GetState == State.Alive)
        {
            Health -= value;
            OnTakeDamage?.Invoke(value);
            LocalDebugger($"{(attachedGo != null ? attachedGo.name : "You")} took {value} damage. Health is {(Health > 0 ? Health : 0)}");
            if (Health <= 0) { Kill(); return; }
            OnHealthChanged?.Invoke(new HealthChangeArgs(Health, MaxHealth));
        }

        else LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} death, cant take damage!");
    }

    public void Heal(float value)
    {
        if (GetState == State.Alive)
        {
            if (IsWounded)
            {
                Health = Math.Min(Health + value, MaxHealth);
                OnHeal?.Invoke(value);
                OnHealthChanged?.Invoke(new HealthChangeArgs(Health, MaxHealth));
                LocalDebugger($"{(attachedGo != null ? attachedGo.name : "You")} {value} healed. Health is {Health}");
            }

            else LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name}'s" : "Your")} health is already at Max Health");
        }

        else LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} death, cant heal!");
    }

    public void OverHeal(float value)
    {
        if (GetState == State.Alive)
        {
            Health += value;
            OnHeal?.Invoke(value);
            OnHealthChanged?.Invoke(new HealthChangeArgs(Health, MaxHealth));
            LocalDebugger($"{(attachedGo != null ? attachedGo.name : "You")} {value} overhealed. Health is {Health}");
        }

        else LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} death, cant heal!");
    }

    public void Revive(float? health = null)
    {
        if (GetState == State.Dead)
        {
            Health =  health ?? MaxHealth;
            Health = Math.Min(Health, MaxHealth);
            OnRevive?.Invoke();
            OnHealthChanged(new HealthChangeArgs(Health, MaxHealth));
            LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} revived!");
        }

        else LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} alive, cant revive!");
    }

    private void LocalDebugger(object message) { if (useDebug) Debug.Log(message); }
    public enum State { Alive, Dead }
}

public struct HealthChangeArgs
{
    public float health;
    public float maxHealth;

    public HealthChangeArgs(float health, float maxHealth)
    {
        this.health = health;
        this.maxHealth = maxHealth;
    }

    public float HealthRatio => health / maxHealth;
    public string HealthString => health.ToString("F0");
    public string MaxHealthString => maxHealth.ToString("F0");
}

public interface IHealthComponentCallbacks
{
    public void OnTakeDamage(float value);
    public void OnHeal(float value);
    public void OnDeath();
    public void OnRevive();
    public void OnHealthChanged(HealthChangeArgs args);
}