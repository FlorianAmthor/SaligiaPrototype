using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using static PixelCrushers.DialogueSystem.QuestLogWindow;

namespace SuspiciousGames.Saligia.UI
{
    public class QuestInfoUI : MonoBehaviour, ISelectHandler
    {
        [Header("Prefab")]
        [SerializeField] private QuestEntryUI _questEntryUIPrefab;

        [Space(10), Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _succesDescriptionText;
        [SerializeField] private GameObject _questEntryContainer;
        [SerializeField] private UIFoldout _uiFoldout;

        private QuestInfo _questInfo;
        public QuestInfo QuestInfo => _questInfo;
        private List<QuestEntryUI> _questEntries;
        private LocalizedString _localizedQuestState;
        private UIQuestWindow _questWindowManager;

        public void Init(UIQuestWindow questWindowManager)
        {
            _questWindowManager = questWindowManager;
            _uiFoldout.onValueChanged.AddListener(OnFoldOutValueChange);
            Debug.Log(_uiFoldout.onValueChanged.GetPersistentEventCount());
        }

        public void UpdateQuestInfo(QuestInfo questInfo)
        {
            _questInfo = questInfo;
            UpdateUI();
        }

        private void UpdateUI()
        {
            var questState = QuestLog.GetQuestState(_questInfo.Title);
            if (questState.HasFlag(QuestState.Active) || questState.HasFlag(QuestState.Success))
            {
                if (_localizedQuestState == null)
                    _localizedQuestState = new LocalizedString();
                _localizedQuestState.TableReference = "QuestStates";
                _localizedQuestState.TableEntryReference = QuestLog.GetQuestState(_questInfo.Title).ToString();
                string localizedQuestState = _localizedQuestState.GetLocalizedString();
                _titleText.text = QuestLog.GetQuestTitle(_questInfo.Title) + " - " + localizedQuestState;
            }
            else
                _titleText.text = QuestLog.GetQuestTitle(_questInfo.Title);
            _descriptionText.text = QuestLog.GetQuestDescription(_questInfo.Title, QuestState.Active);
            _succesDescriptionText.text = QuestLog.GetQuestDescription(_questInfo.Title, QuestState.Success);

            if (_questEntries == null)
            {
                _questEntries = new List<QuestEntryUI>();
                for (int i = 0; i < _questInfo.Entries.Length; i++)
                    _questEntries.Add(Instantiate(_questEntryUIPrefab, _questEntryContainer.transform));
            }

            //TODO what to do when quest entry is finished
            for (int i = 0; i < _questInfo.Entries.Length; i++)
                _questEntries[i].UpdateQuestEntry(_questInfo.Title, i);
        }

        private IEnumerator SnapToQuestRoutine()
        {
            yield return new WaitForEndOfFrame();
            _questWindowManager.SnapToQuest(GetComponent<RectTransform>());
        }

        private void OnFoldOutValueChange(bool value)
        {
            if (!value)
                return;
            StartCoroutine(SnapToQuestRoutine());
        }

        public void OnSelect(BaseEventData eventData)
        {
            StartCoroutine(SnapToQuestRoutine());
        }

        public void SetVerticalDownSelectable(Selectable selectableOnDown)
        {
            var selectable = GetComponent<Selectable>();
            var navigation = selectable.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnDown = selectableOnDown;
            navigation.wrapAround = false;
            selectable.navigation = navigation;
        }

        public void SetVerticalUpSelectable(Selectable selectableOnUp)
        {
            var selectable = GetComponent<Selectable>();
            var navigation = selectable.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = selectableOnUp;
            navigation.wrapAround = false;
            selectable.navigation = navigation;
        }
    }
}