using UnityEngine;
using ZenoxZX.HealthSystem;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthPresenter : MonoBehaviour
{
    public PresentType presentType = PresentType.Text;
    public ImageType imageType;
    public TextType textType = TextType.MainValue;
    public AffixType affixType = AffixType.None;
    public string healthPrefix, armorPrefix, healthSuffix, armorSuffix;
    public HealthSystem target;
    public Image healthImage, armorImage;
    public Image[] healthImages_A, armorImages_A; 
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
                    case AffixType.None:

                        healthTMP.text = $"{args.Health}";
                        armorTMP.text = $"{args.Armor}";

                        break;

                    case AffixType.Prefix:

                        healthTMP.text = $"{healthPrefix}{args.Health}";
                        armorTMP.text = $"{armorPrefix}{args.Armor}";

                        break;

                    case AffixType.Suffix:

                        healthTMP.text = $"{args.Health}{healthSuffix}";
                        armorTMP.text = $"{args.Armor}{armorSuffix}";

                        break;
                    case AffixType.Both:

                        healthTMP.text = $"{healthPrefix}{args.Health}{healthSuffix}";
                        armorTMP.text = $"{armorPrefix}{args.Armor}{armorSuffix}";

                        break;
                }
                break;

            case TextType.BothValue:

                switch (affixType)
                {
                    case AffixType.None:

                        healthTMP.text = $"{args.Health} / {args.MaxHealth}";
                        armorTMP.text = $"{args.Armor} / {args.MaxArmor}";

                        break;

                    case AffixType.Prefix:

                        healthTMP.text = $"{healthPrefix}{args.Health} / {args.MaxHealth}";
                        armorTMP.text = $"{armorPrefix}{args.Armor} / {args.MaxArmor}";

                        break;

                    case AffixType.Suffix:

                        healthTMP.text = $"{args.Health} / {args.MaxHealth}{healthSuffix}";
                        armorTMP.text = $"{args.Armor} / {args.MaxArmor}{armorSuffix}";

                        break;
                    case AffixType.Both:

                        healthTMP.text = $"{healthPrefix}{args.Health} / {args.MaxHealth}{healthSuffix}";
                        armorTMP.text = $"{armorPrefix}{args.Armor} / {args.MaxArmor}{armorSuffix}";

                        break;
                }
                break;
        }
    }

    private void DrawImage(HealthChangeArgs args)
    {
        switch (imageType)
        {
            case ImageType.BothSingle:

                healthImage.fillAmount = args.HealthRatio;
                armorImage.fillAmount = args.ArmorRatio;

                break;
            case ImageType.HealthSingleArmorMultiple:

                healthImage.fillAmount = args.HealthRatio;
                CalculateMultipleImages(ref armorImages_A, args, false);

                break;
            case ImageType.HealthMultipleArmorSingle:

                CalculateMultipleImages(ref healthImages_A, args);
                armorImage.fillAmount = args.ArmorRatio;

                break;
            case ImageType.BothMultiple:

                CalculateMultipleImages(ref healthImages_A, args);
                CalculateMultipleImages(ref armorImages_A, args, false);

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

    public enum ImageType { BothSingle, HealthSingleArmorMultiple, HealthMultipleArmorSingle, BothMultiple }
    public enum TextType { MainValue, BothValue }
    public enum AffixType { None, Prefix, Suffix, Both }
    public enum PresentType { Text, Image, Both }
}


