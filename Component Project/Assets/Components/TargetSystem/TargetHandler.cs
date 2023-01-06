using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZenoxZX.TargetSystem
{
    public class TargetHandler : MonoBehaviour
    {
        public TargetSystem<DummyEnemy> TargetSystem { get; private set; }

        public DummyEnemy currentTarget { get; private set; }
        private DummyEnemy[] allocTargets = new DummyEnemy[5];

        public float sightDistance, sightAngle;
        [SerializeField] LayerMask layerMask;
        [SerializeField] int skipFrameCount;

        [field: SerializeField] public bool UseCustomDirection { get; private set; }
        [SerializeField] Transform customLookTransform;
        public Vector3? LookDirection => UseCustomDirection ? (customLookTransform.position.ToX0Z() - transform.position).normalized : (Vector3?)null;

        private int tick;

        private void Awake()
        {
            TargetSystem = new TargetSystem<DummyEnemy>(gameObject, sightDistance, sightAngle, layerMask);
        }

        private void Update()
        {
            tick++;

            if (tick >= skipFrameCount)
            {
                tick -= skipFrameCount;
                SkipUpdate();
            }
        }

        private void LateUpdate()
        {
            
        }

        private void SkipUpdate()
        {
            if (currentTarget == null)
            {
                if (!TargetSystem.AnyTargetInSight) return;
                Debug.Log(TargetSystem.GetTargetsNonAlloc(transform.position));
                allocTargets = TargetSystem.GetTargetsNonAlloc(transform.position);
                DummyEnemy target = TargetSystem.GetClosestEnemy(transform, allocTargets, TargetSystem.IsOnSightAngle, LookDirection);
                currentTarget = TargetSystem.IsInSight(target) ? target : null;
            }

            else if (!currentTarget.CanTargetable)
            {
                currentTarget = null;
            }
        }
    }
}