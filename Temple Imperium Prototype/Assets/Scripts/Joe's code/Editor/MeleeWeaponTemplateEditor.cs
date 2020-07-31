using UnityEditor;

//------------------------------------------------------\\
//  This script currently does not add any functionality\\
//  It exists for expandability purposes in case any    \\
//  extra editor features are needed in the future      \\
//  for melee weapons.                                  \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

[CustomEditor(typeof(MeleeWeaponTemplate))]
public class MeleeWeaponTemplateEditor : WeaponTemplateEditor
{
    public override void OnInspectorGUI()
    {
        //Draw the default weapon editor GUI
        base.OnInspectorGUI();
    }
}