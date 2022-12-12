using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZenoxZX.HealthSystem
{
    public class HealthComponent
    {
        public float Health { get; private set; }
        public float MaxHealth { get; private set; }
        public float Armor { get; private set; }
        public float MaxArmor { get; private set; }
        public bool Invulnerable { get; set; } = false;

        public DamageType damageType = DamageType.HealthOnly;
        public DynamicDamageRatio dynamicDamageRatio = DynamicDamageRatio.Default;

        public event Action OnDeath, OnRevive, OnArmorDrain;
        public event Action<float> OnTakeDamage, OnHeal, OnRestoreArmor;
        public event Action<HealthChangeArgs> OnHealthChange;
        private readonly bool useDebug;
        private readonly GameObject attachedGo;

        public HealthComponent(float maxHealth,
                               float maxArmor,
                               float? health = null,
                               float? armor = null,
                               DamageType? damageType = null,
                               DynamicDamageRatio? customDynamicDamageRatio = null,
                               bool useDebug = false,
                               GameObject AttachedGo = null,
                               Action<HealthChangeArgs> OnHealthChange = null,
                               Action<float> OnTakeDamage = null,
                               Action<float> OnHeal = null,
                               Action<float> OnRestoreArmor = null,
                               Action OnDeath = null,
                               Action OnRevive = null
                              )
        {
            MaxHealth = maxHealth;
            Health = health ?? maxHealth;
            MaxArmor = maxArmor;
            Armor = armor ?? 0;

            this.damageType = damageType ?? this.damageType;
            dynamicDamageRatio = customDynamicDamageRatio ?? dynamicDamageRatio;
            this.useDebug = useDebug;
            attachedGo = AttachedGo;

            if (OnHealthChange != null) this.OnHealthChange += OnHealthChange;
            if (OnTakeDamage != null) this.OnTakeDamage += OnTakeDamage;
            if (OnHeal != null) this.OnHeal += OnHeal;
            if (OnRestoreArmor != null) this.OnRestoreArmor += OnRestoreArmor;
            if (OnDeath != null) this.OnDeath += OnDeath;
            if (OnRevive != null) this.OnRevive += OnRevive;
        }

        //~HealthComponent()
        //{
        //    // TODO
        //}

        public string HealthString => Health.ToString("F0");
        public string MaxHealthString => MaxHealth.ToString("F0");
        public State GetState => Health > 0 ? State.Alive : State.Dead;
        public bool IsWounded => Health > 0 && Health < MaxHealth;
        public bool HasArmor => Armor > 0;
        public bool CanRestoreArmor => Armor < MaxArmor;
        public float HealthRatio => Health / MaxHealth;

        public void SetMaxHealth(float maxHealth, float? health = null)
        {
            MaxHealth = maxHealth;
            Health = Math.Min(health ?? Health, MaxHealth);
            OnHealthChange?.Invoke(new HealthChangeArgs(Health, MaxHealth, Armor, MaxArmor));
            LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name}'s" : "Your")} max health changed to {maxHealth}!");
        }

        public void SetMaxArmor(float maxArmor, float? armor = null)
        {
            MaxArmor = maxArmor;
            Armor = Math.Min(armor ?? Armor, MaxArmor);
            OnHealthChange?.Invoke(new HealthChangeArgs(Health, MaxHealth, Armor, MaxArmor));
            LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name}'s" : "Your")} max armor changed to {maxArmor}!");
        }

        public void SetMaxHealthAndArmor(float maxHealth, float maxArmor, float? health = null, float? armor = null)
        {
            SetMaxHealth(maxHealth, health);
            SetMaxArmor(maxArmor, armor);
            LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name}'s" : "Your")} max health changed to {maxHealth} and max armor changed to {maxArmor}!");
        }

        public void Kill()
        {
            if (Invulnerable) { LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} invulnerable, cant die!"); return; }
            Health = 0;
            Armor = 0;
            OnDeath?.Invoke();
            OnHealthChange?.Invoke(new HealthChangeArgs(Health, MaxHealth, Armor, MaxArmor));
            LocalDebugger($"{(attachedGo != null ? attachedGo.name : "You")} died!");
        }

        public void TakeDamage(float value, DamageType? damageType = null)
        {
            if (Invulnerable) { LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} invulnerable, cant take damage!"); return; }
            if (GetState == State.Alive)
            {
                damageType ??= this.damageType;

                switch (damageType)
                {
                    case DamageType.HealthOnly:

                        Health -= value;

                        break;

                    case DamageType.ArmorFirst:

                        if (value > Armor)
                        {
                            float remainDamage = value - Armor;
                            Armor = 0;
                            Health -= remainDamage;
                        }

                        else Armor -= value;

                        break;

                    case DamageType.Dynamic:

                        if (HasArmor)
                        {
                            float armorDamage = value * dynamicDamageRatio.ArmorRatio;
                            float healthDamage = value * dynamicDamageRatio.HealthRatio;
                            Armor = Math.Max(Armor - armorDamage, 0);
                            Health -= healthDamage; 
                        }

                        else Health -= value;

                        break;
                }


                OnTakeDamage?.Invoke(value);
                LocalDebugger($"{(attachedGo != null ? attachedGo.name : "You")} took {value} damage. Health is {(Health > 0 ? Health : 0)}, Armor is {(Armor > 0 ? Armor : 0)}");
                if (Health <= 0) { Kill(); return; }
                OnHealthChange?.Invoke(new HealthChangeArgs(Health, MaxHealth, Armor, MaxArmor));
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
                    OnHealthChange?.Invoke(new HealthChangeArgs(Health, MaxHealth, Armor, MaxArmor));
                    LocalDebugger($"{(attachedGo != null ? attachedGo.name : "You")} {value} healed. Health is {Health}");
                }

                else LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name}'s" : "Your")} health is already at Max Health!");
            }

            else LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} death, cant heal!");
        }

        public void OverHeal(float value)
        {
            if (GetState == State.Alive)
            {
                Health += value;
                OnHeal?.Invoke(value);
                OnHealthChange?.Invoke(new HealthChangeArgs(Health, MaxHealth, Armor, MaxArmor));
                LocalDebugger($"{(attachedGo != null ? attachedGo.name : "You")} {value} overhealed. Health is {Health}.");
            }

            else LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} death, cant heal!");
        }

        public void RestoreArmor(float value)
        {
            if (GetState == State.Alive)
            {
                if (CanRestoreArmor)
                {
                    Armor = Math.Min(Armor + value, MaxArmor);
                    OnRestoreArmor?.Invoke(value);
                    LocalDebugger($"{(attachedGo != null ? attachedGo.name : "You")} {value} armor restored. Armor is {Armor}.");
                }

                else LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name}'s" : "Your")} armor is already at Max Armor!");
            }

            else LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} death, cant restore armor!");
        }

        public void Revive(float? health = null, float? armor = null)
        {
            if (GetState == State.Dead)
            {
                Health = Math.Min(health ?? Health, MaxHealth);
                Armor = Math.Min(armor ?? Armor, MaxArmor);
                OnRevive?.Invoke();
                OnHealthChange(new HealthChangeArgs(Health, MaxHealth, Armor, MaxArmor));
                LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} revived!");
            }

            else LocalDebugger($"{(attachedGo != null ? $"{attachedGo.name} is" : "You are")} alive, cant revive!");
        }

        private void LocalDebugger(object message) { if (useDebug) Debug.Log(message); }
        public enum State { Alive, Dead }
    }

    public struct HealthChangeArgs
    {
        public readonly float Health { get; }
        public readonly float MaxHealth { get; }
        public readonly float Armor { get; }
        public readonly float MaxArmor { get; }

        public HealthChangeArgs(float health, float maxHealth, float armor, float maxArmor)
        {
            Health = health;
            MaxHealth = maxHealth;
            Armor = armor;
            MaxArmor = maxArmor;
        }

        public float HealthRatio => Health / MaxHealth;
        public float ArmorRatio => Armor / MaxArmor;
        public string HealthString => Health.ToString("F0");
        public string MaxHealthString => MaxHealth.ToString("F0");
        public string ArmorString => Armor.ToString("F0");
        public string MaxArmorString => MaxArmor.ToString("F0");
    }

    [Serializable]
    public struct DynamicDamageRatio
    {
        /// <summary>
        /// Must be between 0 - 1 !
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Damage ratio for Health from total damage")]
        [field: Range(0, 1)]
        public float HealthRatio { get; private set; }

        /// <summary>
        /// Must be between 0 - 1 !
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Damage ratio for Armor from total damage")]
        [field: Range(0, 1)]
        public float ArmorRatio { get; private set; }

        /// <summary>
        /// Both parameters Must be between 0 - 1 !
        /// </summary>
        /// <param name="healthRatio"> Must be between 0 - 1 !</param>
        /// <param name="armorRatio"> Must be between 0 - 1 !</param>
        public DynamicDamageRatio(float healthRatio, float armorRatio)
        {
            HealthRatio = healthRatio;
            ArmorRatio = armorRatio;
        }

        public static DynamicDamageRatio Default => new DynamicDamageRatio(.2f, .4f);
    }

    public interface IHealthComponentCallbacks
    {
        public void OnTakeDamage(float value);
        public void OnHeal(float value);
        public void OnRestoreArmor(float value);
        public void OnDeath();
        public void OnRevive();
        public void OnHealthChanged(HealthChangeArgs args);
    }

    [Serializable]
    public class HealthConfig
    {      
        public DamageType damageType = DamageType.HealthOnly;
        public float maxHealth = 100, maxArmor = 100;
        public bool startFullHP = true;
        public float startHP = 0, startArmor = 0;
        public DynamicDamageRatio dynamicDamageRatio = DynamicDamageRatio.Default;
        public bool useDebug = false;
    }

    public enum DrawType { EditOnInspector, GetFromConfig }
    public enum DamageType { HealthOnly, ArmorFirst, Dynamic }
}