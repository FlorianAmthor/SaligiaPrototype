using SuspiciousGames.Saligia.Core.Entities.Buffs;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.Saligia.UI
{
    public class BuffUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _buffPrefab;
        [Space(10), SerializeField] private GameObject _buffContainer;

        private Queue<BuffUI> _unusedBuffUiQueue;
        private Dictionary<BuffData, BuffUI> _inUseBuffUiDictionary;

        private void Start()
        {
            _unusedBuffUiQueue = new();
            _inUseBuffUiDictionary = new();

            for (int i = 0; i < 15; i++)
            {
                var buffUI = Instantiate(_buffPrefab, _buffContainer.transform).GetComponent<BuffUI>();
                buffUI.gameObject.SetActive(false);
                _unusedBuffUiQueue.Enqueue(buffUI);
            }
        }

        public void OnBuffAdded(BuffData buffData)
        {
            BuffUI buffUI;
            if (_inUseBuffUiDictionary.TryGetValue(buffData, out buffUI))
            {
                buffUI.Init(buffData);
            }
            else
            {
                buffUI = _unusedBuffUiQueue.Dequeue();
                buffUI.gameObject.SetActive(true);
                buffUI.Init(buffData);
                _inUseBuffUiDictionary.Add(buffData, buffUI);
            }
        }

        public void OnBuffRemoved(BuffData buffData)
        {
            if (_inUseBuffUiDictionary.TryGetValue(buffData, out var buffUI))
            {
                buffUI.gameObject.SetActive(false);
                _inUseBuffUiDictionary.Remove(buffData);
                _unusedBuffUiQueue.Enqueue(buffUI);
            }
        }
    }
}