using System;
using UnityEditor;
using UnityEngine;

//------------------------------------------------------\\
//  Custom editor to enhance the inspector interface    \\
//  for creating a gun.                                 \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//      and modified/optimised for prototype phase      \\
//------------------------------------------------------\\

[CustomEditor(typeof(GunWeaponTemplate))]
public class GunWeaponTemplateEditor : WeaponTemplateEditor
{
    //Indexes for the dropdown menus used to select sound effects
    int meleeSoundIndex;
    int reloadSoundIndex;
    int objectHitSoundIndex;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GunWeaponTemplate targetTemplate = (GunWeaponTemplate)target;

        //Draw the default weapon editor GUI 
        base.OnInspectorGUI();

        if (soundEffectOptions == null || soundEffectOptions.Length == 0)
            return;

        //Set the sound array index to the index of the chosen sound (prevents them from resetting)
        if (!string.IsNullOrEmpty(targetTemplate.m_meleeSound))
            meleeSoundIndex = Array.IndexOf(soundEffectOptions, targetTemplate.m_meleeSound);
        if (!string.IsNullOrEmpty(targetTemplate.m_reloadSound))
            reloadSoundIndex = Array.IndexOf(soundEffectOptions, targetTemplate.m_reloadSound);
        if (!string.IsNullOrEmpty(targetTemplate.m_objectHitSound))
            objectHitSoundIndex = Array.IndexOf(soundEffectOptions, targetTemplate.m_objectHitSound);

        //Melee attack sound selection
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Melee sound: ", EditorStyles.label, GUILayout.MaxWidth(100f));
        meleeSoundIndex = EditorGUILayout.Popup(meleeSoundIndex, soundEffectOptions);
        targetTemplate.m_meleeSoundVolume = EditorGUILayout.FloatField(targetTemplate.m_meleeSoundVolume);
        GUILayout.EndHorizontal();
        targetTemplate.m_meleeSound = soundEffectOptions[meleeSoundIndex];

        //Reload sound selection
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Reload sound: ", EditorStyles.label, GUILayout.MaxWidth(100f));
        reloadSoundIndex = EditorGUILayout.Popup(reloadSoundIndex, soundEffectOptions);
        targetTemplate.m_reloadSoundVolume = EditorGUILayout.FloatField(targetTemplate.m_reloadSoundVolume);
        GUILayout.EndHorizontal();
        targetTemplate.m_reloadSound = soundEffectOptions[reloadSoundIndex];

        //Object hit sound selection
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Object hit sound: ", EditorStyles.label, GUILayout.MaxWidth(100f));
        objectHitSoundIndex = EditorGUILayout.Popup(objectHitSoundIndex, soundEffectOptions);
        targetTemplate.m_objectHitSoundVolume = EditorGUILayout.FloatField(targetTemplate.m_objectHitSoundVolume);
        GUILayout.EndHorizontal();
        targetTemplate.m_objectHitSound = soundEffectOptions[objectHitSoundIndex];

        //Apply any properties that have been changed
        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}
