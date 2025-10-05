using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardConfig))] 
public class CardConfigEditor : Editor
{
    private const int SpriteWidth = 100;
    private const int SpriteHeight = 150;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Get the target config
        CardConfig config = (CardConfig)target;

        // Add a small preview if a sprite is assigned
        if (config.CardSprite != null)  // Assuming your field is named 'cardSprite'
        {
            GUILayout.Space(10);  // Add some padding
            GUILayout.Label("Preview:", EditorStyles.boldLabel);
            Rect previewRect = GUILayoutUtility.GetRect(SpriteWidth, SpriteHeight, GUILayout.ExpandWidth(false));
            EditorGUI.ObjectField(previewRect, config.CardSprite, typeof(Sprite), false);
            // Alternative: For a simple texture preview without the object field
            // GUI.DrawTexture(previewRect, config.cardSprite.texture, ScaleMode.ScaleToFit, true);
        }
        else
        {
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Assign a Sprite to see a preview here.", MessageType.Info);
        }
    }
}