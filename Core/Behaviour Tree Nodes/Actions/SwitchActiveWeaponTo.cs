using SuspiciousGames.Saligia.Core.Entities.Components.Weapons;
using TheKiwiCoder;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.BehaviourTreeNodes.Actions
{
    public class SwitchActiveWeaponTo : ActionNode
    {
        [SerializeField] private WeaponIdentifier _weaponIdentifier;
        protected override void OnStart() { }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (!context.owner.WeaponComponent)
            {
                Debug.LogWarning($"Owner of Behaviour Tree has no WeaponComponent");
                return State.Failure;
            }

            if (context.owner.WeaponComponent.IsWeaponEquipped(_weaponIdentifier))
                return State.Success;

            if (context.owner.WeaponComponent.SwitchToWeapon(_weaponIdentifier.weaponType, _weaponIdentifier.weaponName))
                return State.Success;
            else
                return State.Failure;
        }
    }
}