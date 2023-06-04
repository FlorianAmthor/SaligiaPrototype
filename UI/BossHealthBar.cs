using SuspiciousGames.Saligia.Core.Entities.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace SuspiciousGames.Saligia.UI
{
    public class BossHealthBar : MonoBehaviour
    {
        [SerializeField] private UIDocument _healthBar;
        [SerializeField] private string _healthBarName;

        private Label _healthBarLabel;
        private VisualElement _healthBarVisualElement;
        private HealthComponent _healthComponent;

        private void Start()
        {
            _healthBarLabel = _healthBar.rootVisualElement.Q<Label>("BossHealthLabel");
            _healthBarVisualElement = _healthBar.rootVisualElement.Q("BossHealthFill");
            _healthComponent = GetComponent<HealthComponent>();

            _healthBar.rootVisualElement.style.flexGrow = 1;

            _healthBarLabel.text = _healthBarName;
            UpdateHealthBar(_healthComponent.HealthPercentage);
            _healthComponent.OnHealthChanged.AddListener(UpdateHealthBar);
        }

        private void UpdateHealthBar(float healthPercentage)
        {
            _healthBarVisualElement.style.width = Length.Percent(healthPercentage * 100);
        }
    }
}

