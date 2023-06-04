using DuloGames.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.Saligia.UI
{
    [System.Serializable]
    public class TabManager
    {
        [SerializeField] private List<UITab> _tabList;
        [SerializeField] private UITab _defaultUITab;
        private int _currentTabIndex = 0;

        public void ActivateNextTab()
        {
            _currentTabIndex++;
            _currentTabIndex %= _tabList.Count;
            ActivateTab();
        }

        public void ActivatePreviousTab()
        {
            _currentTabIndex--;
            if (_currentTabIndex == -1)
                _currentTabIndex = _tabList.Count - 1;
            ActivateTab();
        }

        public void ResetActiveTabIndex()
        {
            _currentTabIndex = 0;
        }

        public void ActivateDefaultTab()
        {
            _defaultUITab.Activate();
        }

        private void ActivateTab()
        {
            _tabList[_currentTabIndex].Activate();
        }
    }
}