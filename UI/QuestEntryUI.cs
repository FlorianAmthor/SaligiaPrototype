using PixelCrushers.DialogueSystem;
using UnityEngine;
using TMPro;

namespace SuspiciousGames.Saligia.UI
{
    public class QuestEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _questEntryDescriptionText;
        private QuestState _questEntryState;

        public void UpdateQuestEntry(string questTitle, int questEntryIndex)
        {
            _questEntryState = QuestLog.GetQuestEntryState(questTitle, questEntryIndex);
            _questEntryDescriptionText.text = QuestLog.GetQuestEntry(questTitle, questEntryIndex);
        }
    }
}