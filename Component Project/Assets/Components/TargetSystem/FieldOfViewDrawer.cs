using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenoxZX.TargetSystem;


namespace ZenoxZX.TargetSystem
{
    public class FieldOfViewDrawer : MonoBehaviour
    {
        public Hero hero;
        public float meshResolution;
        public MeshFilter viewMeshFilter;
        private Mesh viewMesh;

        private void Start()
        {
            viewMesh = new Mesh();
            viewMesh.name = "View Mesh";
            viewMeshFilter.mesh = viewMesh;
        }

        private void LateUpdate()
        {
            DrawFOV();
        }

        public void DrawFOV()
        {
            float viewAngle = hero.targetAngle;
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
            float stepAngleSize = viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();

            for (int i = 0; i < stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle * .5f + stepAngleSize * i;
                Vector3 dir = DirFromAngle(angle, true);
                Vector3 point = transform.position + dir * hero.targetDistance;
                viewPoints.Add(point);
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;

            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            viewMesh.Clear();
            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool isGlobalAngle)
        {
            if (!isGlobalAngle) angleInDegrees += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}