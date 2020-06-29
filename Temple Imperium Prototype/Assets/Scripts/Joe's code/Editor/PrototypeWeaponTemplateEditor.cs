﻿using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrototypeWeaponTemplate))]
public class PrototypeWeaponTemplateEditor : WeaponTemplateEditor
{
    int firingSoundIndex;
    int disableSoundIndex;
    int powerSoundIndex;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        PrototypeWeaponTemplate targetTemplate = (PrototypeWeaponTemplate)target;

        base.OnInspectorGUI();

        //Set the sound array indexes to the index of the chosen sound (prevents them from resetting)
        if(!string.IsNullOrEmpty( targetTemplate.m_firingSound ))
            firingSoundIndex = Array.IndexOf(soundEffectOptions, targetTemplate.m_firingSound);
        if (!string.IsNullOrEmpty(targetTemplate.m_disableSound))
            disableSoundIndex = Array.IndexOf(soundEffectOptions, targetTemplate.m_disableSound);
        if (!string.IsNullOrEmpty(targetTemplate.m_powerSound))
            powerSoundIndex = Array.IndexOf(soundEffectOptions, targetTemplate.m_powerSound);

        //Firing Sound
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Firing sound: ", EditorStyles.label, GUILayout.MaxWidth(100f));
        firingSoundIndex = EditorGUILayout.Popup(firingSoundIndex, soundEffectOptions);
        targetTemplate.m_firingSoundVolume = EditorGUILayout.FloatField(targetTemplate.m_firingSoundVolume);
        GUILayout.EndHorizontal();
        targetTemplate.m_firingSound = soundEffectOptions[firingSoundIndex];

        //Disable Sound
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Disable sound: ", EditorStyles.label, GUILayout.MaxWidth(100f));
        disableSoundIndex = EditorGUILayout.Popup(disableSoundIndex, soundEffectOptions);
        targetTemplate.m_disableSoundVolume = EditorGUILayout.FloatField(targetTemplate.m_disableSoundVolume);
        GUILayout.EndHorizontal();
        targetTemplate.m_disableSound = soundEffectOptions[disableSoundIndex];

        //Power Sound
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Power sound: ", EditorStyles.label, GUILayout.MaxWidth(100f));
        powerSoundIndex = EditorGUILayout.Popup(powerSoundIndex, soundEffectOptions);
        targetTemplate.m_powerSoundVolume = EditorGUILayout.FloatField(targetTemplate.m_powerSoundVolume);
        GUILayout.EndHorizontal();
        targetTemplate.m_powerSound = soundEffectOptions[powerSoundIndex];

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}
