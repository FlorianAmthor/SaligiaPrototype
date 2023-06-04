using SuspiciousGames.Saligia.Core.Entities.Buffs;
using SuspiciousGames.Saligia.Core.Entities.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static SuspiciousGames.Saligia.Core.Entities.Buffs.CrowdControlBuffData;

namespace SuspiciousGames.Saligia.Core.Entities
{
    public class BuffComponent : EntityComponent
    {
        public UnityEvent<BuffData> BuffAdded;
        public UnityEvent<BuffData> BuffRemoved;
        public UnityEvent<BuffData> DebuffAdded;
        public UnityEvent<BuffData> DebuffRemoved;

        //TODO implement diminishing returns later
        private readonly Dictionary<CrowdControlType, Multiplier> _diminishingReturnMultipliers = new Dictionary<CrowdControlType, Multiplier>();

        private readonly Dictionary<BuffData, Buff> _buffs = new Dictionary<BuffData, Buff>();

        void Update()
        {
            //OPTIONAL, return before updating each buff if game is paused
            if (Time.timeScale == 0)
                return;

            foreach (var buff in new List<Buff>(_buffs.Values))
            {
                buff.Tick(Time.deltaTime);
                if (buff.IsFinished)
                {
                    _buffs.Remove(buff.BuffData);
                    if (buff.BuffData.IsPositiveBuff)
                        BuffRemoved.Invoke(buff.BuffData);
                    else
                        DebuffRemoved.Invoke(buff.BuffData);
                }
            }
        }

        public void AddBuff(BuffData buffData, Entity buffSource = null)
        {
            if (_buffs.ContainsKey(buffData))
            {
                // if buffdata is typeof ccbuffdata
                _buffs[buffData].Activate();
            }
            else
            {
                var buff = buffData.InitializeBuff(Owner, buffSource);
                _buffs.Add(buffData, buff);
                buff.Activate();
            }

            if (buffData.IsPositiveBuff)
                BuffAdded.Invoke(buffData);
            else
                DebuffAdded.Invoke(buffData);
        }

        /// <summary>
        /// Tries to remove an existing buff with <paramref name="buffdata"/>
        /// </summary>
        /// <param name="buffdata"></param>
        /// <returns>Returns if the buff was removed successfully</returns>
        public bool RemoveBuff(BuffData buffdata)
        {
            if (_buffs.ContainsKey(buffdata))
            {
                _buffs.Remove(buffdata);

                if (buffdata.IsPositiveBuff)
                    BuffRemoved.Invoke(buffdata);
                else
                    DebuffRemoved.Invoke(buffdata);

                return true;
            }
            return false;
        }
    }
}
