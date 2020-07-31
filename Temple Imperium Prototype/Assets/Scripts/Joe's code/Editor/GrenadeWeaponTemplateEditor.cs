using System;
using UnityEditor;
using UnityEngine;

//------------------------------------------------------\\
//  Custom editor to enhance the inspector interface    \\
//  for creating a grenade.                             \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//      and modified/optimised for prototype phase      \\
//------------------------------------------------------\\

[CustomEditor(typeof(GrenadeWeaponTemplate))]
public class GrenadeWeaponTemplateEditor : WeaponTemplateEditor
{
    //Index for the dropdown menu used to select the throw sound
    int throwSoundIndex;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GrenadeWeaponTemplate targetTemplate = (GrenadeWeaponTemplate)target;

        //Draw the default weapon editor GUI first
        base.OnInspectorGUI();

        if (soundEffectOptions == null || soundEffectOptions.Length == 0)
            return;

        //Set the sound array indexes to the index of the chosen sound (prevents them from resetting)
        if (!string.IsNullOrEmpty(targetTemplate.m_throwSound))
            throwSoundIndex = Array.IndexOf(soundEffectOptions, targetTemplate.m_throwSound);

        //Throw sound selection
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Throw sound: ", EditorStyles.label, GUILayout.MaxWidth(100f));
        throwSoundIndex = EditorGUILayout.Popup(throwSoundIndex, soundEffectOptions);
        targetTemplate.m_throwSoundVolume = EditorGUILayout.FloatField(targetTemplate.m_throwSoundVolume);
        GUILayout.EndHorizontal();
        targetTemplate.m_throwSound = soundEffectOptions[throwSoundIndex];

        //Apply any properties that have been changed
        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}
