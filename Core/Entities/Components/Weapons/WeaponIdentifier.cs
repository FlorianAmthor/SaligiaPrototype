using System;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Entities.Components.Weapons
{
    [Serializable]
    public class WeaponIdentifier
    {
        public WeaponType weaponType;
        [Tooltip("This should be set if an you have multiple weapons of the same type. E.g. two MeleeWeapons")]
        public string weaponName;

        public WeaponIdentifier(WeaponType weaponType, string weaponName)
        {
            this.weaponType = weaponType;
            this.weaponName = weaponName;
        }

        public bool EqualsValues(WeaponIdentifier weaponIdentifier)
        {
            return weaponType == weaponIdentifier.weaponType && weaponName == weaponIdentifier.weaponName;
        }
    }
}
