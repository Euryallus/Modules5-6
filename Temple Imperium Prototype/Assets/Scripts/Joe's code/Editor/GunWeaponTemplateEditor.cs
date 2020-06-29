using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GunWeaponTemplate))]
public class GunWeaponTemplateEditor : WeaponTemplateEditor
{
    int meleeSoundIndex;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GunWeaponTemplate targetTemplate = (GunWeaponTemplate)target;

        base.OnInspectorGUI();

        //Set the sound array index to the index of the chosen sound (prevents them from resetting)
        if (!string.IsNullOrEmpty(targetTemplate.m_meleeSound))
            meleeSoundIndex = Array.IndexOf(soundEffectOptions, targetTemplate.m_meleeSound);

        //Melee attack Sound
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Melee sound: ", EditorStyles.label, GUILayout.MaxWidth(100f));
        meleeSoundIndex = EditorGUILayout.Popup(meleeSoundIndex, soundEffectOptions);
        targetTemplate.m_meleeSoundVolume = EditorGUILayout.FloatField(targetTemplate.m_meleeSoundVolume);
        GUILayout.EndHorizontal();
        targetTemplate.m_meleeSound = soundEffectOptions[meleeSoundIndex];

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}
