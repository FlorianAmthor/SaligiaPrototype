using SuspiciousGames.Saligia.Core.Entities.Player;
using SuspiciousGames.Saligia.Core.Skills;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

namespace SuspiciousGames.Saligia.Core.Entities.Components
{
    [RequireComponent(typeof(CastComponent))]
    public class AimingComponent : EntityComponent
    {
        [SerializeField] private string _cancelAimingTrigger;
        [SerializeField] private UnityEvent<bool> _aimingStart;
        [SerializeField] private UnityEvent _aimingEnd;

        private Action<CallbackContext> _onMove;
        private bool _isAiming = false;
        public bool IsAiming => _isAiming;
        private bool _isAborted = false;

        private AimingSO _currentAimingSO;

        public void StartAiming(AimingSO aiming)
        {
            _currentAimingSO = aiming;
            Owner.Animator.ResetTrigger(_cancelAimingTrigger);
            _isAiming = true;
            _isAborted = false;
            Owner.BlockStaggering(true);
            aiming.onAbort.AddListener(AbortAiming);
            _aimingStart.Invoke(aiming.StartAiming((PlayerEntity)Owner));
            _onMove += aiming.OnMove;
            StartCoroutine("Aiming", aiming);
        }

        public void EndAiming()
        {
            _isAiming = false;
        }

        public void AbortAiming()
        {
            if (_isAiming)
            {
                _isAborted = true;
                _isAiming = false;
                Destroy(Owner.CastComponent.ActiveSkill);
                Owner.Animator.SetTrigger(_cancelAimingTrigger);
            }
        }

        private IEnumerator Aiming(AimingSO aiming)
        {
            if (Owner.WeaponComponent.GetActiveWeapon(out var activeWeapon))
                activeWeapon.gameObject.SetActive(false);
            while (_isAiming)
            {
                aiming.UpdateAiming((PlayerEntity)Owner);
                yield return new WaitForEndOfFrame();
            }
            OnAimingEnd(aiming);
        }


        private void OnAimingEnd(AimingSO aiming)
        {
            if (Owner.WeaponComponent.GetActiveWeapon(out var activeWeapon))
                activeWeapon.gameObject.SetActive(true);
            _onMove -= aiming.OnMove;
            aiming.EndAiming((PlayerEntity)Owner);
            _aimingEnd.Invoke();
            aiming.onAbort.RemoveListener(AbortAiming);
            _currentAimingSO = null;
            Owner.TargetingComponent.ResetTarget();
            if (!_isAborted)
                Owner.CastComponent.OnAimingEnd();
            else
                Owner.BlockStaggering(false);
        }

        public void OnFreeAimMove(CallbackContext context)
        {
            if (_onMove != null && _currentAimingSO && !(_currentAimingSO is ObjectLockedSO))
                _onMove.Invoke(context);
        }

        public void OnLockedAimingMove(CallbackContext context)
        {
            if (_onMove != null && _currentAimingSO && _currentAimingSO is ObjectLockedSO)
                _onMove.Invoke(context);
        }

        public void OnAbortAiming(CallbackContext context)
        {
            if (context.started)
                AbortAiming();
        }
    }
}
