using SuspiciousGames.Saligia.Core.Entities;
using System;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Components.Weapons
{
    public class RangeWeapon : Weapon
    {
        [SerializeField] Transform _muzzleTransform;
        public Transform MuzzleTransform => _muzzleTransform;

        private void Start()
        {

        }

        public override void Init(Action<Entity> hitCallback = null)
        {

        }

        public override void BeginAttack()
        {
        }

        public override void EndAttack()
        {
        }
    }
}