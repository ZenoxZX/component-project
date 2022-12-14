using UnityEngine;
using ZenoxZX.HealthSystem;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthPresenter : MonoBehaviour
{
    public PresentType presentType = PresentType.Text;
    public ImageType healthImageType;
    public ImageType armorImageType;
    public TextType textType = TextType.MainValue;
    public AffixType affixType = AffixType.None;
    public string healthPrefix, armorPrefix, healthSuffix, armorSuffix;
    public HealthSystem target;
    public Image healthImage, armorImage;
    public Image[] healthImages, armorImages; 
    public TextMeshProUGUI healthTMP, armorTMP;
    private bool started = false, isSub = false;
    private bool CanSub => started && !isSub;

    #region MONO

    private void Start()
    {
        started = true;

        HealthComponent_OnHealthChange(new HealthChangeArgs(target.HealthComponent.Health, target.HealthComponent.MaxHealth, target.HealthComponent.Armor, target.HealthComponent.MaxArmor));

        if (CanSub && target != null)
        {
            isSub = true;
            target.HealthComponent.OnHealthChange += HealthComponent_OnHealthChange;
        }

        else if (target == null) Debug.Log("Failure! Target is null. Can't subscribed event!");
    }

    private void OnEnable()
    {
        if (CanSub && target != null)
        {
            isSub = true;
            target.HealthComponent.OnHealthChange += HealthComponent_OnHealthChange;
        }

        else if (target == null) Debug.Log("Failure! Target is null. Can't subscribed event!");
    }

    private void OnDisable()
    {
        isSub = false;
        target.HealthComponent.OnHealthChange -= HealthComponent_OnHealthChange;
    }

    #endregion
    private void HealthComponent_OnHealthChange(HealthChangeArgs args)
    {
        switch (presentType)
        {
            case PresentType.Text: DrawText(args); break;
            case PresentType.Image: DrawImage(args); break;
            case PresentType.Both: DrawImage(args); DrawText(args); break;
        }
    }

    private void DrawText(HealthChangeArgs args)
    {
        switch (textType)
        {
            case TextType.MainValue:

                switch (affixType)
                {
                    case AffixType.None: healthTMP.text = $"{args.Health}"; break;
                    case AffixType.Prefix: healthTMP.text = $"{healthPrefix}{args.Health}"; break;
                    case AffixType.Suffix: healthTMP.text = $"{args.Health}{healthSuffix}"; break;
                    case AffixType.Both: healthTMP.text = $"{healthPrefix}{args.Health}{healthSuffix}"; break;
                }

                if (target.healthConfig.damageType == DamageType.HealthOnly || armorTMP == null) break;

                switch (affixType)
                {
                    case AffixType.None: armorTMP.text = $"{args.Armor}"; break;
                    case AffixType.Prefix: armorTMP.text = $"{armorPrefix}{args.Armor}"; break;
                    case AffixType.Suffix: armorTMP.text = $"{args.Armor}{armorSuffix}"; break;
                    case AffixType.Both: armorTMP.text = $"{armorPrefix}{args.Armor}{armorSuffix}"; break;
                }

                break;

            case TextType.BothValue:

                switch (affixType)
                {
                    case AffixType.None: healthTMP.text = $"{args.Health} / {args.MaxHealth}"; break;
                    case AffixType.Prefix: healthTMP.text = $"{healthPrefix}{args.Health} / {args.MaxHealth}"; break;
                    case AffixType.Suffix: healthTMP.text = $"{args.Health} / {args.MaxHealth}{healthSuffix}"; break;
                    case AffixType.Both: healthTMP.text = $"{healthPrefix}{args.Health} / {args.MaxHealth}{healthSuffix}"; break;
                }

                if (target.healthConfig.damageType == DamageType.HealthOnly || armorTMP == null) break;

                switch (affixType)
                {
                    case AffixType.None: armorTMP.text = $"{args.Armor} / {args.MaxArmor}"; break;
                    case AffixType.Prefix: armorTMP.text = $"{armorPrefix}{args.Armor} / {args.MaxArmor}"; break;
                    case AffixType.Suffix: armorTMP.text = $"{args.Armor} / {args.MaxArmor}{armorSuffix}"; break;
                    case AffixType.Both: armorTMP.text = $"{armorPrefix}{args.Armor} / {args.MaxArmor}{armorSuffix}"; break;
                }

                break;
        }
    }

    private void DrawImage(HealthChangeArgs args)
    {
        switch (healthImageType)
        {
            case ImageType.Single: healthImage.fillAmount = args.HealthRatio; break;
            case ImageType.Multiple: CalculateMultipleImages(ref healthImages, args); break;
        }

        if (target.healthConfig.damageType == DamageType.HealthOnly) return;

        switch (armorImageType)
        {
            case ImageType.Single:
                if (armorImage == null) return;
                armorImage.fillAmount = args.ArmorRatio; 
                break;
            case ImageType.Multiple:
                if (armorImages == null) return;
                CalculateMultipleImages(ref armorImages, args, false); 
                break;
        }
    }

    public void CalculateMultipleImages(ref Image[] images, HealthChangeArgs args, bool isHealth = true)
    {
        int imageCount = images.Length;
        float ratio = isHealth ? args.HealthRatio : args.ArmorRatio;
        float ratioPerImage = 1f / imageCount;

        for (int i = 0; i < imageCount; i++)
        {
            float fillAmount = Math.Min(ratio / ratioPerImage, 1);
            images[i].fillAmount = fillAmount;
            ratio = Math.Max(ratio - (fillAmount / imageCount), 0);
        }
    }

    public enum ImageType { Single, Multiple }
    public enum TextType { MainValue, BothValue }
    public enum AffixType { None, Prefix, Suffix, Both }
    public enum PresentType { Text, Image, Both }
}


