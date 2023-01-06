using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZenoxZX.TargetSystem;

[CustomEditor(typeof(FieldOfViewDrawer))]
public class FieldOfViewDrawerEditor : Editor
{
    private FieldOfViewDrawer fov;
    private Hero hero;

    private void OnSceneGUI()
    {
        fov = (FieldOfViewDrawer)target;
        hero = fov.hero;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, hero.targetDistance);
        Vector3 viewAngleA = fov.DirFromAngle(-hero.targetAngle * .5f, false);
        Vector3 viewAngleB = fov.DirFromAngle(hero.targetAngle * .5f, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * hero.targetDistance);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * hero.targetDistance);
    }
}
