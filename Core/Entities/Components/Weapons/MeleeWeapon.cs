using SuspiciousGames.Saligia.Core.Entities;
using SuspiciousGames.Saligia.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.Saligia.Core.Components.Weapons
{
    public class MeleeWeapon : Weapon
    {
        [Serializable]
        public class AttackPoint
        {
            public float radius;
            public Vector3 offset;
            public Transform attackRoot;

#if UNITY_EDITOR
            //editor only as it's only used in editor to display the path of the attack that is used by the raycast
            [NonSerialized] public List<Vector3> previousPositions = new List<Vector3>();
#endif
        }

        [SerializeField] private GameObject _onHitEffectPrefab;
        [SerializeField] private float _onHitTimeScale = 1f;
        [SerializeField] private float _onHitScaleDuration = 0f;
        public AttackPoint[] attackPoints = new AttackPoint[0];

        protected static RaycastHit[] s_RaycastHitCache = new RaycastHit[32];
        protected static Collider[] s_ColliderCache = new Collider[32];

        private HashSet<Collider> _alreadyHitColliders;

        private Action<Entity> _hitCallback;
        private Vector3[] m_PreviousPos;

        /// <summary>
        /// Sets the <paramref name="ownerEntity"/> the weapon and a optional <paramref name="hitCallback"/> for custom actions apart from damage
        /// </summary>
        /// <param name="ownerEntity"></param>
        /// <param name="hitCallback"></param>
        public override void Init(Action<Entity> hitCallback = null)
        {
            _hitCallback = null;
            if (hitCallback != null)
                _hitCallback += hitCallback;
            _alreadyHitColliders = new HashSet<Collider>();
        }

        public override void BeginAttack()
        {
            if (_alreadyHitColliders == null)
                _alreadyHitColliders = new HashSet<Collider>();
            _alreadyHitColliders.Clear();
            IsInAttack = true;

            m_PreviousPos = new Vector3[attackPoints.Length];

            for (int i = 0; i < attackPoints.Length; ++i)
            {
                Vector3 worldPos = attackPoints[i].attackRoot.position +
                                   attackPoints[i].attackRoot.TransformVector(attackPoints[i].offset);
                m_PreviousPos[i] = worldPos;
            }
        }

        public override void EndAttack()
        {
            IsInAttack = false;

#if UNITY_EDITOR
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                attackPoints[i].previousPositions.Clear();
            }
#endif
        }

        public override void ResetAlreadyHit()
        {
            _alreadyHitColliders.Clear();
        }

        private void FixedUpdate()
        {
            if (IsInAttack)
            {
                for (int i = 0; i < attackPoints.Length; ++i)
                {
                    AttackPoint pts = attackPoints[i];

                    Vector3 worldPos = pts.attackRoot.position + pts.attackRoot.TransformVector(pts.offset);
                    Vector3 attackVector = worldPos - m_PreviousPos[i];

                    if (attackVector.magnitude < 0.001f)
                    {
                        // A zero vector for the sphere cast don't yield any result, even if a collider overlap the "sphere" created by radius. 
                        // so we set a very tiny microscopic forward cast to be sure it will catch anything overlaping that "stationary" sphere cast
                        attackVector = Vector3.forward * 0.0001f;
                    }

                    Ray r = new Ray(worldPos, attackVector.normalized);

                    int contacts = Physics.SphereCastNonAlloc(r, pts.radius, s_RaycastHitCache, attackVector.magnitude,
                        targetLayers,
                        QueryTriggerInteraction.Ignore);

                    for (int k = 0; k < contacts; ++k)
                    {
                        Collider col = s_RaycastHitCache[k].collider;

                        if (col != null)
                        {
                            if (!_alreadyHitColliders.Contains(col))
                                if (CheckCollider(col))
                                {
                                    if (_onHitEffectPrefab != null)
                                        Instantiate(_onHitEffectPrefab, s_RaycastHitCache[k].point, Quaternion.identity);
                                    if (_onHitScaleDuration > 0)
                                        Breaker.Instance.AddSlow(_onHitTimeScale, _onHitScaleDuration, true);
                                }
                            //todo: return true or false if hit was invoked
                            //if hit was invoked Spawn hit effect on s_RaycastHitCache[k].point
                        }
                    }

                    m_PreviousPos[i] = worldPos;

#if UNITY_EDITOR
                    pts.previousPositions.Add(m_PreviousPos[i]);
#endif
                }
            }
        }

        private bool CheckCollider(Collider col)
        {
            Entity entity = col.GetComponent<Entity>();
            if (entity == null)
                entity = col.GetComponentInParent<Entity>();
            if (entity == null || entity == ownerEntity)
            {
                return false;
            }

            _alreadyHitColliders.Add(col);

            _hitCallback?.Invoke(entity);
            return true;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                AttackPoint pts = attackPoints[i];

                if (pts.attackRoot != null)
                {
                    Vector3 worldPos = pts.attackRoot.TransformVector(pts.offset);
                    Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
                    Gizmos.DrawSphere(pts.attackRoot.position + worldPos, pts.radius);
                }

                if (pts.previousPositions.Count > 1)
                {
                    UnityEditor.Handles.DrawAAPolyLine(10, pts.previousPositions.ToArray());
                }
            }
        }

#endif
    }
}