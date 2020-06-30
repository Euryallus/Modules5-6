using UnityEditor;

[CustomEditor(typeof(MeleeWeaponTemplate))]
public class MeleeWeaponTemplateEditor : WeaponTemplateEditor
{
    public override void OnInspectorGUI()
    {
        //Draw the default weapon editor GUI first
        base.OnInspectorGUI();
    }
}
