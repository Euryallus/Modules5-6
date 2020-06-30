﻿using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GrenadeWeaponTemplate))]
public class GrenadeWeaponTemplateEditor : WeaponTemplateEditor
{
    int throwSoundIndex;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GrenadeWeaponTemplate targetTemplate = (GrenadeWeaponTemplate)target;

        base.OnInspectorGUI();

        //Set the sound array indexes to the index of the chosen sound (prevents them from resetting)
        if (!string.IsNullOrEmpty(targetTemplate.m_throwSound))
            throwSoundIndex = Array.IndexOf(soundEffectOptions, targetTemplate.m_throwSound);

        //Throw Sound
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Throw sound: ", EditorStyles.label, GUILayout.MaxWidth(100f));
        throwSoundIndex = EditorGUILayout.Popup(throwSoundIndex, soundEffectOptions);
        targetTemplate.m_throwSoundVolume = EditorGUILayout.FloatField(targetTemplate.m_throwSoundVolume);
        GUILayout.EndHorizontal();
        targetTemplate.m_throwSound = soundEffectOptions[throwSoundIndex];

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}