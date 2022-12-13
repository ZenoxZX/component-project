using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZenoxZX.HealthSystem;


[CustomEditor(typeof(HealthConfig_SO))]
public class HealthConfig_SOEditor : Editor
{
    HealthConfig_SO MyTarget => (HealthConfig_SO)target;
    Texture Logo => Resources.Load<Texture>("1");

    public override void OnInspectorGUI()
    {
        DrawLabel();
        DrawInspector();
    }

    private void DrawLabel()
    {
        GUILayout.Label(Logo);
        GUILayout.Label("Health Config");
        EditorGUILayout.Space(5);
    }

    private void DrawInspector()
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
}
