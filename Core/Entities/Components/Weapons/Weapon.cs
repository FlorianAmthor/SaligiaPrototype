using SuspiciousGames.Saligia.Core.Entities;
using System;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Components.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        protected Entity ownerEntity;
        public LayerMask targetLayers;

        public bool IsInAttack { get; protected set; }

        public void SetOwner(Entity entity)
        {
            ownerEntity = entity;
        }

        public abstract void Init(Action<Entity> hitCallback = null);

        /// <summary>
        /// This will normaly be called by the corresponding attack animation
        /// </summary>
        public abstract void BeginAttack();
        /// <summary>
        /// This will normally be sent by the animation and should only be called when a weapon attack was interrupted
        /// </summary>
        public abstract void EndAttack();

        public virtual void ResetAlreadyHit() { }
    }
}