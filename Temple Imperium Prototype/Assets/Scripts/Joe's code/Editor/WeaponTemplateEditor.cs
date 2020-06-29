using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponTemplate))]
public class WeaponTemplateEditor : Editor
{
    int attackSoundIndex;
    public string[] soundEffectOptions;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        WeaponTemplate targetTemplate = (WeaponTemplate)target;

        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Sound Effect Properties", EditorStyles.boldLabel);

        SoundEffect[] availableSounds = GameObject.Find("_Audio Manager").GetComponent<SoundEffectPlayer>().GetSoundEffects();
        soundEffectOptions = new string[availableSounds.Length];
        for (int i = 0; i < availableSounds.Length; i++)
        {
            soundEffectOptions[i] = availableSounds[i].name;
        }

        //Set the sound array indexes to the index of the chosen sound (prevents them from resetting)
        if (!string.IsNullOrEmpty(targetTemplate.m_attackSound))
            attackSoundIndex = Array.IndexOf(soundEffectOptions, targetTemplate.m_attackSound);

        //Attack Sound
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Attack sound: ", EditorStyles.label, GUILayout.MaxWidth(100f));
        attackSoundIndex = EditorGUILayout.Popup(attackSoundIndex, soundEffectOptions);
        targetTemplate.m_attackSoundVolume = EditorGUILayout.FloatField(targetTemplate.m_attackSoundVolume);
        GUILayout.EndHorizontal();
        targetTemplate.m_attackSound = soundEffectOptions[attackSoundIndex];

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}
