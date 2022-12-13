using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZenoxZX.HealthSystem;

[CustomEditor(typeof(HealthPresenter))]
public class HealthPresenterEditor : Editor
{
    HealthPresenter MyTarget => (HealthPresenter)target;
    Texture Logo => Resources.Load<Texture>("1");

    private SerializedProperty m_HealthImage, m_HealthImages;
    private SerializedProperty m_ArmorImage, m_ArmorImages;
    private SerializedProperty m_HealthPrefix, m_HealthSuffix;
    private SerializedProperty m_ArmorPrefix, m_ArmorSuffix;
    private SerializedProperty m_HealthTMP, m_ArmorTMP;

    private void OnEnable()
    {
        HealthPresenter healthPresenter = MyTarget;
        m_HealthImage = serializedObject.FindProperty(nameof(healthPresenter.healthImage));
        m_HealthImages = serializedObject.FindProperty(nameof(healthPresenter.healthImages_A));
        m_ArmorImage = serializedObject.FindProperty(nameof(healthPresenter.armorImage));
        m_ArmorImages = serializedObject.FindProperty(nameof(healthPresenter.armorImages_A));

        m_HealthPrefix = serializedObject.FindProperty(nameof(healthPresenter.healthPrefix));
        m_HealthSuffix = serializedObject.FindProperty(nameof(healthPresenter.healthSuffix));
        m_ArmorPrefix = serializedObject.FindProperty(nameof(healthPresenter.armorPrefix));
        m_ArmorSuffix = serializedObject.FindProperty(nameof(healthPresenter.armorSuffix));

        m_HealthTMP = serializedObject.FindProperty(nameof(healthPresenter.healthTMP));
        m_ArmorTMP = serializedObject.FindProperty(nameof(healthPresenter.armorTMP));
    }

    public override void OnInspectorGUI()
    {
        HealthPresenter healthPresenter = MyTarget;
        base.OnInspectorGUI();
        DrawLabel();
        DrawInspector(healthPresenter);
    }

    private void DrawLabel()
    {
        GUILayout.Label(Logo);
        EditorGUILayout.Space(5);
    }

    private void DrawInspector(HealthPresenter healthPresenter)
    {
        healthPresenter.presentType = (HealthPresenter.PresentType)EditorGUILayout.EnumPopup("Present Type", healthPresenter.presentType);
        DrawText(healthPresenter);
        DrawHealthImage(healthPresenter);
    }


    private void DrawHealthImage(HealthPresenter healthPresenter)
    {
        EditorGUILayout.Space(5);

        if (healthPresenter.presentType != HealthPresenter.PresentType.Text)
        {
            var healthConfig = healthPresenter.target.drawType == DrawType.GetFromConfig && healthPresenter.target.healthConfig_SO != null ? healthPresenter.target.healthConfig_SO.healthConfig : healthPresenter.target.healthConfig;


            if (healthConfig.damageType == DamageType.HealthOnly)
            {
                healthPresenter.healthImageType = (HealthPresenter.ImageType)EditorGUILayout.EnumPopup("Health Image Type", healthPresenter.healthImageType);
                EditorGUILayout.Space(5);
                DrawHP();
            }

            else
            {
                healthPresenter.armorImageType = (HealthPresenter.ImageType)EditorGUILayout.EnumPopup("Armor Image Type", healthPresenter.armorImageType);
                EditorGUILayout.Space(5);
                DrawHP();
                DrawMP();
            }


        }

        void DrawHP()
        {
            switch (healthPresenter.healthImageType)
            {
                case HealthPresenter.ImageType.Single: EditorGUILayout.PropertyField(m_HealthImage); break;
                case HealthPresenter.ImageType.Multiple: EditorGUILayout.PropertyField(m_HealthImages); break;
            }
        }

        void DrawMP()
        {
            switch (healthPresenter.armorImageType)
            {
                case HealthPresenter.ImageType.Single: EditorGUILayout.PropertyField(m_ArmorImage); break;
                case HealthPresenter.ImageType.Multiple: EditorGUILayout.PropertyField(m_ArmorImages); break;
            }
        }
    }

    private void DrawText(HealthPresenter healthPresenter)
    {
        EditorGUILayout.Space(5);

        if (healthPresenter.presentType != HealthPresenter.PresentType.Image)
        {
            var healthConfig = healthPresenter.target.drawType == DrawType.GetFromConfig && healthPresenter.target.healthConfig_SO != null ? healthPresenter.target.healthConfig_SO.healthConfig : healthPresenter.target.healthConfig;

            EditorGUILayout.PropertyField(m_HealthTMP);
            if (m_HealthTMP.objectReferenceValue == null) EditorGUILayout.HelpBox("Health TMP is Null", MessageType.Error);
            if (healthConfig.damageType != DamageType.HealthOnly) EditorGUILayout.PropertyField(m_ArmorTMP);
            EditorGUILayout.Space(5);
            healthPresenter.textType = (HealthPresenter.TextType)EditorGUILayout.EnumPopup("Text Type", healthPresenter.textType);
            healthPresenter.affixType = (HealthPresenter.AffixType)EditorGUILayout.EnumPopup("Affix Type", healthPresenter.affixType);

            if (healthConfig.damageType == DamageType.HealthOnly)
            {
                switch (healthPresenter.affixType)
                {
                    case HealthPresenter.AffixType.Prefix: EditorGUILayout.PropertyField(m_HealthPrefix); break;
                    case HealthPresenter.AffixType.Suffix: EditorGUILayout.PropertyField(m_HealthSuffix); break;
                    case HealthPresenter.AffixType.Both:
                        EditorGUILayout.PropertyField(m_HealthPrefix);
                        EditorGUILayout.PropertyField(m_HealthSuffix);
                        break;
                }
            }

            else
            {
                switch (healthPresenter.affixType)
                {
                    case HealthPresenter.AffixType.Prefix: 
                        EditorGUILayout.PropertyField(m_HealthPrefix);
                        EditorGUILayout.PropertyField(m_ArmorPrefix);
                        break;
                    case HealthPresenter.AffixType.Suffix:
                        EditorGUILayout.PropertyField(m_HealthSuffix);
                        EditorGUILayout.PropertyField(m_ArmorSuffix);
                        break;
                    case HealthPresenter.AffixType.Both:
                        EditorGUILayout.PropertyField(m_HealthPrefix);
                        EditorGUILayout.PropertyField(m_HealthSuffix);
                        EditorGUILayout.PropertyField(m_ArmorPrefix);
                        EditorGUILayout.PropertyField(m_ArmorSuffix);
                        break;
                }
            }
        }
    }
}
