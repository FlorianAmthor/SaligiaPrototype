using SuspiciousGames.Saligia.Core.Entities.Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SuspiciousGames.Saligia.Core.Entities
{
    public class EnemyManager : MonoBehaviour
    {
        #region Singleton

        private static EnemyManager _instance;

        public static EnemyManager Instance { get { return _instance; } }

        public UnityEvent<EnemyEntity> enemyDeathEvent;
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                //DontDestroyOnLoad(gameObject);
                _enemies = new List<EnemyEntity>();
                _closestEnemies = new List<Tuple<float, EnemyEntity>>();
                foreach (var obj in GameObject.FindGameObjectsWithTag("Enemy"))
                    _enemies.Add(obj.GetComponent<EnemyEntity>());
            }
        }
        #endregion

        private List<EnemyEntity> _enemies;

        private List<Tuple<float, EnemyEntity>> _closestEnemies;

        public void SpawnEnemy(GameObject enemyPrefab, Vector3 spawnPosition, bool forcePlayerTarget = false, UnityAction<Entity> onEnemyDeathCallback = null)
        {
            EnemyEntity enemy = null;
            if (forcePlayerTarget)
                enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.LookRotation(PlayerEntity.Instance.transform.position - spawnPosition)).GetComponent<EnemyEntity>();
            else
                enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity).GetComponent<EnemyEntity>();

            enemy.gameObject.SendMessage("Start");
            if (onEnemyDeathCallback != null)
                enemy.HealthComponent.OnDeath.AddListener(onEnemyDeathCallback);
            if (forcePlayerTarget)
                enemy.ForceTarget(PlayerEntity.Instance);

            AddEnemy(enemy);
        }

        public void AddEnemy(EnemyEntity enemyEntity)
        {
            if (_enemies == null)
                _enemies = new();
            _enemies.Add(enemyEntity);
        }

        public void RemoveEnemy(EnemyEntity enemyEntity)
        {
            if (_enemies == null)
                _enemies = new();
            _enemies.Remove(enemyEntity);
        }

        public void OnEnemyDeath(EnemyEntity enemyEntity)
        {
            _enemies.Remove(enemyEntity);
            enemyDeathEvent?.Invoke(enemyEntity);
        }

        public List<Tuple<float, EnemyEntity>> GetClosestEnemies(Vector3 position, float range)
        {
            _closestEnemies.Clear();
            float rangeToEnemy;

            foreach (var enemy in _enemies)
            {
                if (enemy == null)
                    continue;
                rangeToEnemy = Vector3.Distance(position, enemy.transform.position);
                if (rangeToEnemy <= range)
                    _closestEnemies.Add(new Tuple<float, EnemyEntity>(rangeToEnemy, enemy));
            }

            _enemies.RemoveAll((EnemyEntity e) => e == null);

            _closestEnemies.Sort((Tuple<float, EnemyEntity> t1, Tuple<float, EnemyEntity> t2) =>
            {
                return t1.Item1.CompareTo(t2.Item1);
            });

            return _closestEnemies;
        }

        public List<Tuple<float, EnemyEntity>> GetClosestEnemies(Vector3 position, Vector3 forward, float range, float angle)
        {
            _closestEnemies.Clear();
            float angleToEnemy, rangeToEnemy;

            foreach (var enemy in _enemies)
            {
                if (enemy == null)
                    continue;
                angleToEnemy = Vector3.Angle(forward, (enemy.transform.position - position).normalized);
                rangeToEnemy = Vector3.Distance(position, enemy.transform.position);
                if (rangeToEnemy <= range && angleToEnemy <= angle)
                    _closestEnemies.Add(new Tuple<float, EnemyEntity>(rangeToEnemy, enemy));
            }

            _enemies.RemoveAll((EnemyEntity e) => e == null);

            _closestEnemies.Sort((Tuple<float, EnemyEntity> t1, Tuple<float, EnemyEntity> t2) =>
            {
                return t1.Item1.CompareTo(t2.Item1);
            });

            return _closestEnemies;
        }
    }
}
