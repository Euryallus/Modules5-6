using System;
using UnityEditor;
using UnityEngine;

//------------------------------------------------------\\
//  Custom editor to enhance the inspector interface    \\
//  for creating a weapon. Acts as a base class for     \\
//  other template editors.                             \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//      and modified/optimised for prototype phase      \\
//------------------------------------------------------\\

[CustomEditor(typeof(WeaponTemplate))]
public class WeaponTemplateEditor : Editor
{
    int attackSoundIndex;               //Index for the dropdown menu used to select the attack sound
    public string[] soundEffectOptions; //All possible sound effect names that can be selected

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        WeaponTemplate targetTemplate = (WeaponTemplate)target;

        //Draw the default ScriptableObject GUI first
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Sound Effect Properties", EditorStyles.boldLabel);

        GameObject goAudioManager = GameObject.Find("_Audio Manager");

        //Ensure that an audio manager is present in the current scene,
        //  if not audio dropdowns cannot be displayed, so show some error text instead
        if (goAudioManager != null)
        {
            SoundEffect[] availableSounds = goAudioManager.GetComponent<AudioManager>().GetSoundEffects();
            soundEffectOptions = new string[availableSounds.Length];
            for (int i = 0; i < availableSounds.Length; i++)
            {
                soundEffectOptions[i] = availableSounds[i].name;
            }

            //Set the sound array indexes to the index of the chosen sound (prevents them from resetting)
            if (!string.IsNullOrEmpty(targetTemplate.m_attackSound))
                attackSoundIndex = Array.IndexOf(soundEffectOptions, targetTemplate.m_attackSound);

            //Attack sound selection
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Attack sound: ", EditorStyles.label, GUILayout.MaxWidth(100f));
            attackSoundIndex = EditorGUILayout.Popup(attackSoundIndex, soundEffectOptions);
            targetTemplate.m_attackSoundVolume = EditorGUILayout.FloatField(targetTemplate.m_attackSoundVolume);
            GUILayout.EndHorizontal();
            targetTemplate.m_attackSound = soundEffectOptions[attackSoundIndex];
        }
        else
        {
            //Error text when no audio manager is found
            GUIStyle styleError = new GUIStyle();
            styleError.normal.textColor = Color.red;
            EditorGUILayout.LabelField("No _Audio Manager found in scene.", styleError);
            EditorGUILayout.LabelField("Please add the prefab to edit sounds.", styleError);
        }

        //Apply any properties that have been changed
        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}
