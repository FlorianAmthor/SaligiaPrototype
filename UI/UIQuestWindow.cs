using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PixelCrushers.DialogueSystem.QuestLogWindow;

namespace SuspiciousGames.Saligia.UI
{
    public class UIQuestWindow : UIWindow
    {
        [Header("Prefab"), SerializeField]
        private QuestInfoUI _questInfoUIPrefab;

        [Header("Quest Info Containers")]
        [SerializeField] private ScrollRect _questScrollRect;
        [SerializeField] private GameObject _activeQuestContainer;
        [SerializeField] private GameObject _completedQuestContainer;
        private List<QuestInfo> _completedQuests;
        private List<QuestInfo> _activeQuests;

        private Dictionary<string, QuestInfoUI> _questInfoUIDictionary;

        public ScrollRect QuestScrollRect => _questScrollRect;

        public void OnQuestGroupSelectionChange(Selectable questGroupSelectionSelectable)
        {
            if (_questInfoUIDictionary == null)
                _questInfoUIDictionary = new Dictionary<string, QuestInfoUI>();
            UpdateQuests();

            foreach (var questInfoUI in _questInfoUIDictionary.Values)
            {
                var navigation = questInfoUI.GetComponent<Selectable>().navigation;
                navigation.selectOnLeft = questGroupSelectionSelectable;
                questInfoUI.GetComponent<Selectable>().navigation = navigation;
            }
        }

        public void UpdateQuests()
        {
            if (_questInfoUIDictionary == null)
                _questInfoUIDictionary = new Dictionary<string, QuestInfoUI>();

            _completedQuests = GetQuests(QuestState.Success);
            _activeQuests = GetQuests(QuestState.Active);

            LinkedList<QuestInfoUI> questUIs = new LinkedList<QuestInfoUI>();

            //TODO: add navigation for active quest header

            foreach (var activeQuest in _activeQuests)
            {
                if (_questInfoUIDictionary.TryGetValue(activeQuest.Title, out QuestInfoUI questInfoUI))
                {
                    questInfoUI.UpdateQuestInfo(activeQuest);
                }
                else
                {
                    questInfoUI = Instantiate(_questInfoUIPrefab, _completedQuestContainer.transform);
                    questInfoUI.Init(this);
                    questInfoUI.UpdateQuestInfo(activeQuest);
                    _questInfoUIDictionary.Add(activeQuest.Title, questInfoUI);
                }
                questUIs.AddLast(new LinkedListNode<QuestInfoUI>(questInfoUI));
            }

            //TODO: add navigation for completed quest header

            foreach (var completedQuest in _completedQuests)
            {
                if (_questInfoUIDictionary.TryGetValue(completedQuest.Title, out QuestInfoUI questInfoUI))
                {
                    questInfoUI.UpdateQuestInfo(completedQuest);
                }
                else
                {
                    questInfoUI = Instantiate(_questInfoUIPrefab, _completedQuestContainer.transform);
                    questInfoUI.Init(this);
                    questInfoUI.UpdateQuestInfo(completedQuest);
                    _questInfoUIDictionary.Add(completedQuest.Title, questInfoUI);
                }
                questUIs.AddLast(new LinkedListNode<QuestInfoUI>(questInfoUI));
            }

            foreach (var questInfoUI in _questInfoUIDictionary.Values)
                if (QuestLog.GetQuestState(questInfoUI.QuestInfo.Title) == QuestState.Success)
                    questInfoUI.transform.SetParent(_completedQuestContainer.transform);

            var currentQuestNode = questUIs.First;

            while (currentQuestNode != null)
            {
                currentQuestNode.Value.SetVerticalUpSelectable(currentQuestNode.Previous?.Value.GetComponent<Selectable>());
                currentQuestNode.Value.SetVerticalDownSelectable(currentQuestNode.Next?.Value.GetComponent<Selectable>());
                currentQuestNode = currentQuestNode.Next;
            }

            //questUIs.First.Value.GetComponent<Selectable>().Select();
        }

        private List<QuestInfo> GetQuests(QuestState questStateMask, bool sortByName = false)
        {
            string[] questTitles = QuestLog.GetAllQuests(questStateMask, sortByName);
            List<QuestInfo> result = new List<QuestInfo>();
            foreach (var title in questTitles)
                result.Add(GetQuestInfo(string.Empty, title));
            return result;
        }

        internal void SnapToQuest(RectTransform scrollRectChild)
        {
            //var normalizedPosition = (float)rectTransform.GetSiblingIndex() / (float)_questScrollRect.content.transform.childCount;
            //_questScrollRect.verticalNormalizedPosition = 1 - normalizedPosition;
            //Canvas.ForceUpdateCanvases();
            float normalizePosition = _questScrollRect.GetComponent<RectTransform>().anchorMin.y - scrollRectChild.anchoredPosition.y;
            normalizePosition += (float)scrollRectChild.transform.GetSiblingIndex() / (float)_questScrollRect.content.transform.childCount;
            normalizePosition /= 1000f;
            normalizePosition = Mathf.Clamp01(1 - normalizePosition);
            _questScrollRect.verticalNormalizedPosition = normalizePosition;
        }

        private QuestInfo GetQuestInfo(string group, string title)
        {
            FormattedText description = FormattedText.Parse(QuestLog.GetQuestDescription(title), DialogueManager.masterDatabase.emphasisSettings);
            FormattedText localizedTitle = FormattedText.Parse(QuestLog.GetQuestTitle(title), DialogueManager.masterDatabase.emphasisSettings);
            FormattedText heading = localizedTitle;
            string localizedGroup = string.IsNullOrEmpty(group) ? string.Empty : QuestLog.GetQuestGroup(title);
            bool abandonable = QuestLog.IsQuestAbandonable(title);
            bool trackable = QuestLog.IsQuestTrackingAvailable(title);
            bool track = QuestLog.IsQuestTrackingEnabled(title);
            int entryCount = QuestLog.GetQuestEntryCount(title);
            FormattedText[] entries = new FormattedText[entryCount];
            QuestState[] entryStates = new QuestState[entryCount];
            for (int i = 0; i < entryCount; i++)
            {
                entries[i] = FormattedText.Parse(QuestLog.GetQuestEntry(title, i + 1), DialogueManager.masterDatabase.emphasisSettings);
                entryStates[i] = QuestLog.GetQuestEntryState(title, i + 1);
            }
            return new QuestInfo(localizedGroup, title, heading, description, entries, entryStates, trackable, track, abandonable);
        }
    }
}