using SuspiciousGames.Saligia.Core.Entities;
using SuspiciousGames.Saligia.Core.Entities.Components;
using SuspiciousGames.Saligia.Core.Entities.Components.Weapons;
using SuspiciousGames.Saligia.Core.Entities.Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SuspiciousGames.Saligia.Core.Targeting
{
    public class TargetingComponent : EntityComponent
    {
        [SerializeField] private float _targetingAngle;
        [SerializeField, FormerlySerializedAs("_layerMask")] private LayerMask _targetableLayers;
        [SerializeField] private LayerMask _sightBlockingLayers;

        private List<Tuple<float, Entity>> _enemiesInRange;

        public Entity Target { get; private set; }
        public bool HasTarget => Target != null;
        public UnityEvent<Entity> onTargetChange, onTargetLost;

        private bool _isPlayerEntity;

        void Start()
        {
            _enemiesInRange = new List<Tuple<float, Entity>>();
            _isPlayerEntity = Owner is PlayerEntity;
        }

        private List<Tuple<float, Entity>> GetCloseEnemies(float range, LayerMask layermask)
        {
            List<Tuple<float, Entity>> enemiesInRange = new();
            var colliders = Physics.OverlapSphere(transform.position, range, layermask, QueryTriggerInteraction.Ignore);

            foreach (var collider in colliders)
            {
                if (!collider.TryGetComponent(out Entity entity))
                    continue;
                enemiesInRange.Add(new Tuple<float, Entity>(Vector3.Distance(transform.position, entity.transform.position), entity));
            }

            enemiesInRange.Sort((Tuple<float, Entity> tuple1, Tuple<float, Entity> tuple2) =>
            {
                return tuple1.Item1.CompareTo(tuple2.Item1);
            });

            return enemiesInRange;
        }

        public bool FindTarget(float range)
        {
            return FindTarget(range, _targetableLayers);
        }

        public bool FindTarget(float range, LayerMask layerMask)
        {
            if (Target != null)
                return true;

            _enemiesInRange.Clear();

            _enemiesInRange = GetCloseEnemies(range, layerMask);

            foreach (var tuple in _enemiesInRange)
            {
                if (Vector3.Angle(transform.forward, (tuple.Item2.transform.position - transform.position).normalized) <= _targetingAngle / 2)
                {
                    if (!IsInLineOfSight(tuple.Item2))
                        continue;
                    Target = tuple.Item2;
                    onTargetChange?.Invoke(Target);
                    return true;
                }
            }
            return false;
        }

        public bool IsInLineOfSight(Entity entity)
        {
            var ownerPosition = Owner.transform.position;
            Vector3 toTarget = (entity.transform.position + Vector3.up * 0.5f) - (ownerPosition + Vector3.up * 0.5f);

            return !Physics.Raycast(ownerPosition + Vector3.up * 0.5f, toTarget, toTarget.magnitude, _sightBlockingLayers, QueryTriggerInteraction.Ignore);
        }

        /// <summary>
        /// Finds a target if the current Target is not set or gets the next closest Target depending on the current target
        /// </summary>
        /// <param name="range">Maximum range for target finding</param>
        /// <param name="value">This should be an input value. Left of current target = negative value. Right of current target = positive value.</param>
        /// <returns> Returns false if no target could be found</returns>
        public bool FindNextTarget(float range, float value)
        {
            if (Target == null)
                return FindTarget(range);

            bool targetIsToTheRight = value > 0;

            _enemiesInRange.Clear();
            _enemiesInRange = GetCloseEnemies(range, _targetableLayers);

            Vector3 vectorToEnemy = Vector3.zero;
            float smallestAngle = 360;
            Entity newTarget = Target;
            foreach (var tuple in _enemiesInRange)
            {
                if (tuple.Item2 == Target || !IsInLineOfSight(tuple.Item2))
                    continue;
                vectorToEnemy = tuple.Item2.transform.position - transform.position;
                var angle = Vector3.Angle(transform.forward, vectorToEnemy);
                var a = LineTestForPoint(transform.position, Target.transform.position, tuple.Item2.transform.position);

                if (angle <= _targetingAngle / 2 && angle < smallestAngle)
                {
                    if (targetIsToTheRight && a < 0 || !targetIsToTheRight && a > 0)
                    {
                        smallestAngle = angle;
                        newTarget = tuple.Item2;
                    }
                }
            }

            Target = newTarget;
            onTargetChange.Invoke(Target);
            return true;
        }

        public void ResetTarget()
        {
            Target = null;
            MarkTarget();
        }

        public void OnWeaponChange(WeaponContainer weapon)
        {
            if (weapon != null)
                return;
            onTargetLost.Invoke(Target);
            Target = null;
            MarkTarget();
        }

        /// <summary>
        /// Tests whether the point <paramref name="c"/> is left, right or on the line through <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <param name="a">First point on the line</param>
        /// <param name="b">Second point on the line</param>
        /// <param name="c">Point to test against</param>
        /// <returns>Negative value for left, zero for on the line and positive value for right</returns>
        private float LineTestForPoint(Vector3 a, Vector3 b, Vector3 c)
        {
            return (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z);
        }

        private void MarkTarget()
        {
            //TODO: make a target marker appear above the head of the target
            //if (Target != null)
            //    Target.GetComponent<Renderer>().material.color = Color.red;

            //TODO: remove marker appear above the head of the previous target
            //if (_previousTarget != null)
            //    _previousTarget.GetComponent<Renderer>().material.color = Color.white;


        }

        private void Update()
        {
            if (Target == null)
                return;

            if (!Target.HealthComponent || Target.HealthComponent.IsDead)
            {
                onTargetLost.Invoke(Target);
                Target = null;
                MarkTarget();
                return;
            }

            if (_isPlayerEntity && (Vector3.Distance(transform.position, Target.transform.position) > ((PlayerEntity)Owner).MaxActionRange || !IsInLineOfSight(Target)))
            {
                onTargetLost.Invoke(Target);
                Target = null;
                MarkTarget();
                return;
            }
        }
    }
}
