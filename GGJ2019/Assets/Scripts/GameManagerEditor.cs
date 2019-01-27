﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(GameManager))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager gm = (GameManager)target;
        if (GUILayout.Button("Upgrade Mush-Home"))
        {
            gm.UpgradeHome();
        }
    }
}