using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZenoxZX.TargetSystem
{
    public class TargetSystem<Target> where Target : MonoBehaviour, ITargetable
    {
        private readonly LayerMask layerMask;
        private readonly GameObject gameObject;
        private readonly Transform transform;

        public float SightDistance { get; private set; }
        public float SightAngle { get; private set; }

        // Non Alloc Collections
        private readonly List<Target> targetBufferList = new List<Target>();
        private Collider[] colliderBuffer = new Collider[20];

        // Const
        private const float HALF = .5f;

        public TargetSystem(GameObject gameObject, float sightDistance, float sightAngle, LayerMask layerMask)
        {
            this.gameObject = gameObject;
            this.layerMask = layerMask;
            SightDistance = sightDistance;
            SightAngle = sightAngle;

            transform = this.gameObject.transform;
        }

        public void SetSightAngle(float angle) => SightAngle = angle;
        public void SetSightDistance(float distance) => SightDistance = distance;

        public bool AnyTargetInSight => Physics.CheckSphere(transform.position, SightDistance, layerMask);
        public bool IsOnSightAngle(Vector3 direction, Vector3 targetDirection, float angle) => Vector3.Angle(direction, targetDirection) <= angle * HALF;
        public bool IsOnSightAngle(Vector3 direction, Target target) => IsOnSightAngle(direction, target.transform.forward, SightAngle);
        public bool IsOnSightAngle(Target target) => IsOnSightAngle(transform.forward, target);
        public bool IsInSight(Target target)
        {
            float sightDistanceSqr = SightDistance * SightDistance;
            float distance = (target.transform.position - transform.position).sqrMagnitude;
            return sightDistanceSqr <= distance;
        }

        public Target GetClosestEnemy(Transform originTransform, Target[] targets, Func<Vector3,Target,bool> checkAngle = null, Vector3? forward = null)
        {
            Target bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            float distanceSqr;
            forward ??= transform.forward;

            for (int i = 0; i < targets.Length; i++)
            {
                var foo = targets[i];
                if (!foo.CanTargetable) continue;
                if (checkAngle != null)
                    if (!checkAngle(forward.Value.ToX0Z(), foo)) continue;
                distanceSqr = (foo.transform.position - originTransform.position).sqrMagnitude;

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    bestTarget = foo;
                }
            }

            return bestTarget;
        }

        public bool TryGetClosestEnemy(Transform originTransform, Target[] targets, out Target target, Func<Vector3, Target, bool> checkAngle = null, Vector3? forward = null)
        {
            Target bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            float distanceSqr;
            forward ??= transform.forward;

            for (int i = 0; i < targets.Length; i++)
            {
                var foo = targets[i];
                if (!foo.CanTargetable) continue;
                if (checkAngle != null)
                    if (!checkAngle(forward.Value.ToX0Z(), foo)) continue;
                distanceSqr = (foo.transform.position - originTransform.position).sqrMagnitude;

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    bestTarget = foo;
                }
            }

            target = bestTarget;
            return bestTarget != null;
        }

        public Target[] GetTargetsNonAlloc(Vector3 position)
        {
            targetBufferList.Clear();
            colliderBuffer = Physics.OverlapSphere(position, SightDistance, layerMask);

            for (int i = 0; i < colliderBuffer.Length; i++)
            {
                if (colliderBuffer[i].TryGetComponent(out Target target))
                {
                    targetBufferList.Add(target);
                }
            }

            return targetBufferList.ToArray();
        }
    }
}
