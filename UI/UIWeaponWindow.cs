using SuspiciousGames.Saligia.Core.Player;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace SuspiciousGames.Saligia.UI
{
    public class UIWeaponWindow : UIWindow
    {
        [Header("Content Texts")]
        [SerializeField] private LocalizeStringEvent _weaponNameLocalizeStringEvent;
        [SerializeField] private LocalizeStringEvent _weaponFlavorLocalizeStringEvent;
        [SerializeField] private LocalizeStringEvent _weaponAttackDescriptionLocalizeStringEvent;
        [SerializeField] private LocalizeStringEvent _movementAbilityNameLocalizeStringEvent;
        [SerializeField] private LocalizeStringEvent _movementAbilityDescriptionLocalizeStringEvent;

        [SerializeField] private Image _weaponImage;

        [SerializeField] private Sprite _scytheSprite;
        [SerializeField] private Sprite _grimoireSprite;

        public void OnWeaponSelectionChange(PlayerWeaponType playerWeaponType)
        {
            if (playerWeaponType == PlayerWeaponType.Scythe)
            {
                _weaponNameLocalizeStringEvent.StringReference.TableEntryReference = "scytheName";
                _weaponFlavorLocalizeStringEvent.StringReference.TableEntryReference = "scytheFlavourText";
                _weaponAttackDescriptionLocalizeStringEvent.StringReference.TableEntryReference = "scytheAttackPatternDescription";
                _movementAbilityNameLocalizeStringEvent.StringReference.TableEntryReference = "scytheMovementAbilityName";
                _movementAbilityDescriptionLocalizeStringEvent.StringReference.TableEntryReference = "scytheMovementAbilityDescription";
                _weaponImage.sprite = _scytheSprite;
            }
            else if (playerWeaponType == PlayerWeaponType.Grimoire)
            {
                _weaponNameLocalizeStringEvent.StringReference.TableEntryReference = "grimoireName";
                _weaponFlavorLocalizeStringEvent.StringReference.TableEntryReference = "grimoireFlavourText";
                _weaponAttackDescriptionLocalizeStringEvent.StringReference.TableEntryReference = "grimoireAttackPatternDescription";
                _movementAbilityDescriptionLocalizeStringEvent.StringReference.TableEntryReference = "grimoireMovementAbilityDescription";
                _weaponImage.sprite = _grimoireSprite;
            }
        }
    }
}