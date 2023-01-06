using UnityEngine;

namespace ZenoxZX.TargetSystem
{
    public class DummyEnemy : MonoBehaviour, ITargetable
    {
        public bool CanTargetable => true;
    }
}