using System;
using UnityEditor;
using UnityEngine;
using ZenoxZX.HealthSystem;

[CustomEditor(typeof(HealthSystem))]
public class HealthSystemEditor : Editor
{
    DrawType drawType = DrawType.EditOnInspector;
    HealthSystem MyTarget => (HealthSystem)target;
    Texture Logo => Resources.Load<Texture>("1");
    private bool[] eventToggles = new bool[6];
    private string[] eventNames = { "On Death", "On Revive", "On Take Damage", "On Heal", "On Restore Armor", "On Health Change"};

    private SerializedProperty m_OnDeathEvent;
    private SerializedProperty m_OnReviveEvent;
    private SerializedProperty m_OnTakeDamageEvent;
    private SerializedProperty m_OnHealEvent;
    private SerializedProperty m_OnRestoreArmorEvent;
    private SerializedProperty m_OnHealthChangeEvent;


    private void OnEnable()
    {
        Debug.Log("OnEnable");
        drawType = MyTarget.drawType;
        m_OnDeathEvent = serializedObject.FindProperty(nameof(MyTarget.OnDeath));
        m_OnReviveEvent = serializedObject.FindProperty(nameof(MyTarget.OnRevive));
        m_OnTakeDamageEvent = serializedObject.FindProperty(nameof(MyTarget.OnTakeDamage));
        m_OnHealEvent = serializedObject.FindProperty(nameof(MyTarget.OnHeal));
        m_OnRestoreArmorEvent = serializedObject.FindProperty(nameof(MyTarget.OnRestoreArmor));
        m_OnHealthChangeEvent = serializedObject.FindProperty(nameof(MyTarget.OnHealthChange));
    }


    public override void OnInspectorGUI()
    {
       // base.OnInspectorGUI();
        DrawLabel();
        DrawTypeSelection();
        DrawEvents();
    }

    private void DrawLabel()
    {
        GUILayout.Label(Logo);
    }

    private void DrawTypeSelection()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Health Config");
        drawType = (DrawType)GUILayout.Toolbar((int)drawType, Enum.GetNames(typeof(DrawType)).SplitCapitalsArray());
        EditorGUILayout.EndHorizontal();
        MyTarget.drawType = drawType;
        switch (drawType)
        {
            case DrawType.EditOnInspector: DrawIns(); break;
            case DrawType.GetFromConfig: DrawSO(); break;
        }

        void DrawIns()
        {
            MyTarget.healthConfig.damageType = (DamageType)EditorGUILayout.EnumPopup("Damage Type", MyTarget.healthConfig.damageType);
            MyTarget.healthConfig.maxHealth = EditorGUILayout.FloatField("Max Health", MyTarget.healthConfig.maxHealth);
            if (MyTarget.healthConfig.damageType != DamageType.HealthOnly) MyTarget.healthConfig.maxArmor = EditorGUILayout.FloatField("Max Armor", MyTarget.healthConfig.maxArmor);
            MyTarget.healthConfig.startFullHP = EditorGUILayout.Toggle("Start Full HP", MyTarget.healthConfig.startFullHP);
            if (!MyTarget.healthConfig.startFullHP) MyTarget.healthConfig.startHP = EditorGUILayout.Slider("Start HP", MyTarget.healthConfig.startHP, .1f, MyTarget.healthConfig.maxHealth);
            if (MyTarget.healthConfig.damageType != DamageType.HealthOnly) MyTarget.healthConfig.startArmor = EditorGUILayout.Slider("Start Armor", MyTarget.healthConfig.startArmor, 0, MyTarget.healthConfig.maxArmor);
            if (MyTarget.healthConfig.damageType == DamageType.Dynamic)
            {
                EditorGUILayout.Space(5);
                GUILayout.Label("Dynamic Ratio");
                float healthRatio = EditorGUILayout.Slider("Health Ratio", MyTarget.healthConfig.dynamicDamageRatio.HealthRatio, 0, 1);
                float armorRatio = EditorGUILayout.Slider("Armor Ratio", MyTarget.healthConfig.dynamicDamageRatio.ArmorRatio, 0, 1);
                var dynamicDamageRatio = new DynamicDamageRatio(healthRatio, armorRatio);
                MyTarget.healthConfig.dynamicDamageRatio = dynamicDamageRatio;
                if (GUILayout.Button("Default Ratio")) MyTarget.healthConfig.dynamicDamageRatio = DynamicDamageRatio.Default;
            }
            MyTarget.healthConfig.useDebug = EditorGUILayout.Toggle("Use Debug", MyTarget.healthConfig.useDebug);
               
        }

        void DrawSO()
        {
            MyTarget.healthConfig_SO = (HealthConfig_SO)EditorGUILayout.ObjectField("Health Config", MyTarget.healthConfig_SO, typeof(HealthConfig_SO), false);
        }
    }


    private void DrawEvents()
    {
        EditorGUILayout.Space(5);
        GUILayout.Label("Events");
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        MyTarget.B_OnDeath = GUILayout.Toggle(MyTarget.B_OnDeath, "On Death", "ToolbarButton");
        MyTarget.B_OnRevive = GUILayout.Toggle(MyTarget.B_OnRevive, "On Revive", "ToolbarButton");
        MyTarget.B_OnTakeDamage = GUILayout.Toggle(MyTarget.B_OnTakeDamage, "On Take Damage", "ToolbarButton");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        MyTarget.B_OnHeal = GUILayout.Toggle(MyTarget.B_OnHeal, "On Heal", "ToolbarButton");
        MyTarget.B_OnRestoreArmor = GUILayout.Toggle(MyTarget.B_OnRestoreArmor, "On Restore Armor", "ToolbarButton");
        MyTarget.B_OnHealthChange = GUILayout.Toggle(MyTarget.B_OnHealthChange, "On Health Change", "ToolbarButton");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        if (MyTarget.B_OnDeath) EditorGUILayout.PropertyField(m_OnDeathEvent);
        if (MyTarget.B_OnRevive) EditorGUILayout.PropertyField(m_OnReviveEvent);
        if (MyTarget.B_OnTakeDamage) EditorGUILayout.PropertyField(m_OnTakeDamageEvent);

        if (MyTarget.B_OnHeal) EditorGUILayout.PropertyField(m_OnHealEvent);
        if (MyTarget.B_OnRestoreArmor) EditorGUILayout.PropertyField(m_OnRestoreArmorEvent);
        if (MyTarget.B_OnHealthChange) EditorGUILayout.PropertyField(m_OnHealthChangeEvent);
        EditorGUILayout.EndVertical();
    }
}

