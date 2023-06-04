using SuspiciousGames.Saligia.Core.Entities.Player;
using SuspiciousGames.Saligia.Core.Skills;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace SuspiciousGames.Saligia.UI
{
    public enum PageTurn { left, right }
    public class LoadoutMenuManager : MonoBehaviour
    {
        private class SpellLoadoutSkillInfo
        {
            public UnityEvent<SpellLoadoutSkillInfo> GluttonyEvent;
            public UnityEvent<SpellLoadoutSkillInfo> EnvyEvent;

            private BaseSkill _baseSkill;
            public BaseSkill BaseSkill => _baseSkill;
            private VisualElement _spellRuneSocketGluttony;
            private VisualElement _spellRuneSocketEnvy;
            private Texture2D _gluttonyStoneIcon;
            private Texture2D _envyStoneIcon;
            private VisualElement _bindingScoket;
            private Label _gluttonyDesc;
            private Label _envyDesc;


            public SpellLoadoutSkillInfo(BaseSkill baseSkill,
                VisualElement spellIcon,
                Label spellDescription,
                VisualElement spellRuneSocketGluttony,
                VisualElement spellRuneSocketEnvy,
                Texture2D gluttonyStoneIcon,
                Texture2D envyStoneIcon,
                VisualElement bindingSocket,
                Label gluttonyDesc,
                Label envyDesc)
            {
                _baseSkill = baseSkill;
                _spellRuneSocketGluttony = spellRuneSocketGluttony;
                _spellRuneSocketEnvy = spellRuneSocketEnvy;

                _gluttonyStoneIcon = gluttonyStoneIcon;
                _envyStoneIcon = envyStoneIcon;
                _bindingScoket = bindingSocket;

                _gluttonyDesc = gluttonyDesc;
                _envyDesc = envyDesc;

                spellIcon.style.backgroundImage = baseSkill.Sprite.texture;
                //spellIcon.RegisterCallback<FocusEvent>(e => e.);
                spellDescription.text = baseSkill.SkillDescription;
                gluttonyDesc.text = baseSkill.SkillDescriptionGluttony;
                envyDesc.text = baseSkill.SkillDescriptionEnvy;

                GluttonyEvent = new UnityEvent<SpellLoadoutSkillInfo>();
                EnvyEvent = new UnityEvent<SpellLoadoutSkillInfo>();

                spellRuneSocketGluttony.RegisterCallback<NavigationSubmitEvent>(GluttonySocketSubmited);
                spellRuneSocketEnvy.RegisterCallback<NavigationSubmitEvent>(EnvySocketSubmited);
            }

            private void GluttonySocketSubmited(NavigationSubmitEvent e)
            {
                GluttonyEvent.Invoke(this);
                _spellRuneSocketGluttony.style.backgroundImage = _gluttonyStoneIcon;
                _baseSkill.AddRune(Rune.Gluttony);
                _gluttonyDesc.AddToClassList("gluttony_active");
            }
            private void EnvySocketSubmited(NavigationSubmitEvent e)
            {
                EnvyEvent.Invoke(this);
                _spellRuneSocketEnvy.style.backgroundImage = _envyStoneIcon;
                _baseSkill.AddRune(Rune.Envy);
                _envyDesc.AddToClassList("envy_active");
            }

            public void RemoveGluttonyRune()
            {
                _spellRuneSocketGluttony.style.backgroundImage = new StyleBackground();
                _baseSkill.RemoveRune(Rune.Gluttony);
                _gluttonyDesc.RemoveFromClassList("gluttony_active");
            }
            public void RemoveEnvyRune()
            {
                _spellRuneSocketEnvy.style.backgroundImage = new StyleBackground();
                _baseSkill.RemoveRune(Rune.Envy);
                _envyDesc.RemoveFromClassList("envy_active");
            }

            public void BindVisual(StyleBackground button)
            {
                _bindingScoket.style.backgroundImage = button;
            }
        }

        [SerializeField] private UIDocument _document;
        [SerializeField] private List<VisualTreeAsset> _pagesVTA;
        [SerializeField] private Texture2D _glutonnyRuneIcon;
        [SerializeField] private Texture2D _envyRuneIcon;
        [SerializeField] private Texture2D _buttonSecondary1;
        [SerializeField] private Texture2D _buttonSecondary2;
        [SerializeField] private Texture2D _buttonSecondary3;
        [SerializeField] private PlayerEntity _player;
        [SerializeField] private BaseSkill _skill1;
        [SerializeField] private BaseSkill _skill2;
        [SerializeField] private BaseSkill _skill3;
        [SerializeField] private BaseSkill _skill4;
        [SerializeField] private UnityEvent OnCloseMenu;



        private List<VisualElement> _pagesVE;
        private List<SpellLoadoutSkillInfo> _spellInfos;

        private SpellLoadoutSkillInfo _boundToSecondary1;
        private SpellLoadoutSkillInfo _boundToSecondary2;
        private SpellLoadoutSkillInfo _boundToSecondary3;
        private SpellLoadoutSkillInfo _listeningForRebind;

        private VisualElement _gluttonyNoSocket;
        private VisualElement _envyNoSocket;

        private VisualElement _scythe;
        private VisualElement _grimoire;

        private int _currentIndex = 0;
        private bool _active = false;

        private void Awake()
        {
            _document.rootVisualElement.style.display = DisplayStyle.None;
            _pagesVE = new List<VisualElement>();
            foreach (var page in _pagesVTA)
            {
                var ve = page.Instantiate();
                ve.style.flexGrow = 1;
                _document.rootVisualElement.Add(ve);
                _pagesVE.Add(ve);
            }
        }

        public void Init()
        {
            _skill1.Runes.Clear();
            _skill1.Runes.Add(Rune.Base);

            _skill2.Runes.Clear();
            _skill2.Runes.Add(Rune.Base);

            _skill3.Runes.Clear();
            _skill3.Runes.Add(Rune.Base);

            _skill4.Runes.Clear();
            _skill4.Runes.Add(Rune.Base);

            _scythe = _document.rootVisualElement.Q("ScytheIcon");
            _scythe.RegisterCallback<NavigationSubmitEvent>(e => SwitchToScythe());
            _grimoire = _document.rootVisualElement.Q("GrimoireIcon");
            _grimoire.RegisterCallback<NavigationSubmitEvent>(e => SwitchToGrimoire());
            SwitchToScythe();

            _spellInfos = new List<SpellLoadoutSkillInfo>();

            _gluttonyNoSocket = _document.rootVisualElement.Q("GluttonyNoSocket");
            _gluttonyNoSocket.RegisterCallback<NavigationSubmitEvent>(e => RemoveOtherRunes(Rune.Gluttony));
            RemoveOtherRunes(Rune.Gluttony);

            _envyNoSocket = _document.rootVisualElement.Q("EnvyNoSocket");
            _envyNoSocket.RegisterCallback<NavigationSubmitEvent>(e => RemoveOtherRunes(Rune.Envy));
            RemoveOtherRunes(Rune.Envy);

            var spellIcon1 = _document.rootVisualElement.Q("FlyingOrbIcon");
            var spellDesc1 = _document.rootVisualElement.Q<Label>("FlyingOrbBasicDescription");
            var spellGlutSocket1 = _document.rootVisualElement.Q("GluttonyFlyingOrbSocket");
            var spellEnvySocket1 = _document.rootVisualElement.Q("EnvyFlyingOrbSocket");
            var bindingSocket1 = _document.rootVisualElement.Q("FlyingOrbCurrentButton");
            var spellDesc1Gluttony = _document.rootVisualElement.Q<Label>("FlyingOrbGluttonyDescription");
            var spellDesc1Envy = _document.rootVisualElement.Q<Label>("FlyingOrbEnvyDescription");


            var spellinfo1 = new SpellLoadoutSkillInfo
                (_skill1,
                spellIcon1,
                spellDesc1,
                spellGlutSocket1,
                spellEnvySocket1,
                _glutonnyRuneIcon,
                _envyRuneIcon,
                bindingSocket1,
                spellDesc1Gluttony,
                spellDesc1Envy
                );
            spellIcon1.RegisterCallback<FocusEvent>(e => _listeningForRebind = spellinfo1);
            spellIcon1.RegisterCallback<FocusOutEvent>(e => _listeningForRebind = null);
            spellinfo1.GluttonyEvent.AddListener(e => RemoveOtherRunes(Rune.Gluttony, e));
            spellinfo1.EnvyEvent.AddListener(e => RemoveOtherRunes(Rune.Envy, e));
            _spellInfos.Add(spellinfo1);

            var spellIcon2 = _document.rootVisualElement.Q("BarrageIcon");
            var spellDesc2 = _document.rootVisualElement.Q<Label>("BarrageBasicDescription");
            var spellGlutSocket2 = _document.rootVisualElement.Q("GluttonyBarrageSocket");
            var spellEnvySocket2 = _document.rootVisualElement.Q("EnvyBarrageSocket");
            var bindingSocket2 = _document.rootVisualElement.Q("BarrageCurrentButton");
            var spellDesc2Gluttony = _document.rootVisualElement.Q<Label>("BarrageGluttonyDescription");
            var spellDesc2Envy = _document.rootVisualElement.Q<Label>("BarrageEnvyDescription");

            var spellinfo2 = new SpellLoadoutSkillInfo
                (_skill2,
                spellIcon2,
                spellDesc2,
                spellGlutSocket2,
                spellEnvySocket2,
                _glutonnyRuneIcon,
                _envyRuneIcon,
                bindingSocket2,
                spellDesc2Gluttony,
                spellDesc2Envy
                );
            spellIcon2.RegisterCallback<FocusEvent>(e => _listeningForRebind = spellinfo2);
            spellIcon2.RegisterCallback<FocusOutEvent>(e => _listeningForRebind = null);
            spellinfo2.GluttonyEvent.AddListener(e => RemoveOtherRunes(Rune.Gluttony, e));
            spellinfo2.EnvyEvent.AddListener(e => RemoveOtherRunes(Rune.Envy, e));
            _spellInfos.Add(spellinfo2);

            var spellIcon3 = _document.rootVisualElement.Q("ScourgeIcon");
            var spellDesc3 = _document.rootVisualElement.Q<Label>("ScourgeBasicDescription");
            var spellGlutSocket3 = _document.rootVisualElement.Q("GluttonyScourgeSocket");
            var spellEnvySocket3 = _document.rootVisualElement.Q("EnvyScourgeSocket");
            var bindingSocket3 = _document.rootVisualElement.Q("ScourgeCurrentButton");
            var spellDesc3Gluttony = _document.rootVisualElement.Q<Label>("ScourgeGluttonyDescription");
            var spellDesc3Envy = _document.rootVisualElement.Q<Label>("ScourgeEnvyDescription");

            var spellinfo3 = new SpellLoadoutSkillInfo
                (_skill3,
                spellIcon3,
                spellDesc3,
                spellGlutSocket3,
                spellEnvySocket3,
                _glutonnyRuneIcon,
                _envyRuneIcon,
                bindingSocket3,
                spellDesc3Gluttony,
                spellDesc3Envy
                );
            spellIcon3.RegisterCallback<FocusEvent>(e => _listeningForRebind = spellinfo3);
            spellIcon3.RegisterCallback<FocusOutEvent>(e => _listeningForRebind = null);
            spellinfo3.GluttonyEvent.AddListener(e => RemoveOtherRunes(Rune.Gluttony, e));
            spellinfo3.EnvyEvent.AddListener(e => RemoveOtherRunes(Rune.Envy, e));
            _spellInfos.Add(spellinfo3);

            var spellIcon4 = _document.rootVisualElement.Q("WimsidIcon");
            var spellDesc4 = _document.rootVisualElement.Q<Label>("WimsidBasicDescription");
            var spellGlutSocket4 = _document.rootVisualElement.Q("GluttonyWimsidSocket");
            var spellEnvySocket4 = _document.rootVisualElement.Q("EnvyWimsidSocket");
            var bindingSocket4 = _document.rootVisualElement.Q("WimsidCurrentButton");
            var spellDesc4Gluttony = _document.rootVisualElement.Q<Label>("WimsidGluttonyDescription");
            var spellDesc4Envy = _document.rootVisualElement.Q<Label>("WimsidEnvyDescription");

            var spellinfo4 = new SpellLoadoutSkillInfo
                (_skill4,
                spellIcon4,
                spellDesc4,
                spellGlutSocket4,
                spellEnvySocket4,
                _glutonnyRuneIcon,
                _envyRuneIcon,
                bindingSocket4,
                spellDesc4Gluttony,
                spellDesc4Envy
                );
            spellIcon4.RegisterCallback<FocusEvent>(e => _listeningForRebind = spellinfo4);
            spellIcon4.RegisterCallback<FocusOutEvent>(e => _listeningForRebind = null);
            spellinfo4.GluttonyEvent.AddListener(e => RemoveOtherRunes(Rune.Gluttony, e));
            spellinfo4.EnvyEvent.AddListener(e => RemoveOtherRunes(Rune.Envy, e));
            _spellInfos.Add(spellinfo4);

            BindSpell(spellinfo1, SkillSlot.SecondaryOne);
            BindSpell(spellinfo2, SkillSlot.SecondaryTwo);
            BindSpell(spellinfo3, SkillSlot.SecondaryThree);
        }

        public void OpenIngameMenu()
        {
            if (!_active)
            {
                Time.timeScale = 0;
                AudioListener.pause = true;
                _document.rootVisualElement.style.display = DisplayStyle.Flex;
                UpdatePages();
                _active = true;
            }
        }

        public void CloseIngameMenu()
        {
            if (_active)
            {
                _document.rootVisualElement.style.display = DisplayStyle.None;
                OnCloseMenu.Invoke();
                _active = false;
                AudioListener.pause = false;
                Time.timeScale = 1;
            }
        }

        public void SwitchForward()
        {
            SwitchPage(PageTurn.right);
        }

        public void SwitchBackwards()
        {
            SwitchPage(PageTurn.left);
        }

        public void BindSecondaryOne()
        {
            if (_listeningForRebind != null)
                BindSpell(_listeningForRebind, SkillSlot.SecondaryOne);
        }
        public void BindSecondaryTwo()
        {
            if (_listeningForRebind != null)
                BindSpell(_listeningForRebind, SkillSlot.SecondaryTwo);
        }
        public void BindSecondaryThree()
        {
            if (_listeningForRebind != null)
                BindSpell(_listeningForRebind, SkillSlot.SecondaryThree);
        }

        private void SwitchPage(PageTurn pageTurn)
        {
            if (pageTurn == PageTurn.left)
                _currentIndex--;
            else
                _currentIndex++;

            if (_currentIndex < 0)
                _currentIndex = _pagesVE.Count - 1;

            if (_currentIndex > _pagesVE.Count - 1)
                _currentIndex = 0;

            UpdatePages();
        }

        private void UpdatePages()
        {
            for (int i = 0; i < _pagesVE.Count; i++)
            {
                if (_currentIndex == i)
                {
                    _pagesVE[i].style.display = DisplayStyle.Flex;
                }
                else
                {
                    _pagesVE[i].style.display = DisplayStyle.None;
                }
            }

            var ve = _pagesVE[_currentIndex].Q(className: "selectable");
            ve?.Focus();
        }

        private void RemoveOtherRunes(Rune rune, SpellLoadoutSkillInfo filter = null)
        {
            if (filter == null)
            {
                if (rune == Rune.Gluttony)
                    _gluttonyNoSocket.style.backgroundImage = _glutonnyRuneIcon;
                else
                    _envyNoSocket.style.backgroundImage = _envyRuneIcon;
            }
            else
            {
                if (rune == Rune.Gluttony)
                    _gluttonyNoSocket.style.backgroundImage = new StyleBackground();
                else
                    _envyNoSocket.style.backgroundImage = new StyleBackground();
            }

            foreach (var info in _spellInfos)
            {
                if (info == filter)
                    continue;

                if (rune == Rune.Gluttony)
                    info.RemoveGluttonyRune();
                else
                    info.RemoveEnvyRune();
            }
        }

        private void SwitchToScythe()
        {
            _grimoire.RemoveFromClassList("selected");
            _scythe.AddToClassList("selected");
            _player.SwitchWeapon(Core.Player.PlayerWeaponType.Scythe);
        }

        private void SwitchToGrimoire()
        {
            _scythe.RemoveFromClassList("selected");
            _grimoire.AddToClassList("selected");
            _player.SwitchWeapon(Core.Player.PlayerWeaponType.Grimoire);
        }

        private void BindSpell(SpellLoadoutSkillInfo info, SkillSlot skillSlot)
        {
            switch (skillSlot)
            {
                case SkillSlot.SecondaryOne:
                    RebindSpell(info, _buttonSecondary1, _boundToSecondary1);
                    _boundToSecondary1 = info;
                    break;
                case SkillSlot.SecondaryTwo:
                    RebindSpell(info, _buttonSecondary2, _boundToSecondary2);
                    _boundToSecondary2 = info;
                    break;
                case SkillSlot.SecondaryThree:
                    RebindSpell(info, _buttonSecondary3, _boundToSecondary3);
                    _boundToSecondary3 = info;
                    break;
                default:
                    break;
            }

            _player.ChangeSecondaryAbilityOld(info.BaseSkill, skillSlot);
        }

        private void RebindSpell(SpellLoadoutSkillInfo newSpell, Texture2D button, SpellLoadoutSkillInfo oldSpell)
        {
            if (newSpell == oldSpell)
            {
                return;
            }

            if (newSpell == _boundToSecondary1)
            {
                _boundToSecondary1 = oldSpell;
                newSpell.BindVisual(button);
                _boundToSecondary1.BindVisual(_buttonSecondary1);
                _player.ChangeSecondaryAbilityOld(oldSpell.BaseSkill, SkillSlot.SecondaryOne);
                return;
            }
            if (newSpell == _boundToSecondary2)
            {
                _boundToSecondary2 = oldSpell;
                newSpell.BindVisual(button);
                _boundToSecondary2.BindVisual(_buttonSecondary2);
                _player.ChangeSecondaryAbilityOld(oldSpell.BaseSkill, SkillSlot.SecondaryTwo);
                return;
            }
            if (newSpell == _boundToSecondary3)
            {
                _boundToSecondary3 = oldSpell;
                newSpell.BindVisual(button);
                _boundToSecondary3.BindVisual(_buttonSecondary3);
                _player.ChangeSecondaryAbilityOld(oldSpell.BaseSkill, SkillSlot.SecondaryThree);
                return;
            }

            if (oldSpell != null)
                oldSpell.BindVisual(new StyleBackground());
            newSpell.BindVisual(button);
        }

    }

}

