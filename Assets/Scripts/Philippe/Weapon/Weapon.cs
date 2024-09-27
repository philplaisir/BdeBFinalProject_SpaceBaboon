using UnityEngine;

namespace SpaceBaboon.WeaponSystem
{
    public class Weapon : BaseStats<MonoBehaviour>, IStatsEditable
    {
        public override ScriptableObject GetData()
        {
            //throw new System.NotImplementedException();
            return null;
        }

        protected virtual void Attack() { }
    }
}
