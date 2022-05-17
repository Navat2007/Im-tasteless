/*
using UnityEditor;

[CustomEditor(typeof(Wave))]
public class WaveInspector : UnityEditor.Editor
{
    public bool waveDetails;
    public bool enemyVariants;
    public bool enemyParamsDetails;

    private Wave _wave;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        _wave = (Wave) target;
        serializedObject.Update();

        #region Wave Details

        EditorGUILayout.Separator();

        waveDetails = EditorGUILayout.Toggle("Wave Details", waveDetails);

        if (waveDetails)
        {
            serializedObject.FindProperty("infinite").boolValue = EditorGUILayout.Toggle("Infinite :", _wave.infinite);
            serializedObject.FindProperty("enemyCount").intValue = EditorGUILayout.IntField("Enemy Count :", _wave.enemyCount);
            serializedObject.FindProperty("timeBetweenSpawn").floatValue = EditorGUILayout.FloatField("Time Between Spawn :", _wave.timeBetweenSpawn);
        }

        #endregion

        #region Enemy variants
        
        EditorGUILayout.Separator();

        enemyVariants = EditorGUILayout.Toggle("Enemy Variants", enemyVariants);

        if (enemyVariants)
        {
            serializedObject.FindProperty("enemyList").tree = EditorGUILayout.Tree("Infinite :", _wave.infinite);
            serializedObject.FindProperty("enemyCount").intValue = EditorGUILayout.IntField("Enemy Count :", _wave.enemyCount);
            serializedObject.FindProperty("timeBetweenSpawn").floatValue = EditorGUILayout.FloatField("Time Between Spawn :", _wave.timeBetweenSpawn);
        }

        #endregion
        
        
        
        
        serializedObject.ApplyModifiedProperties();
        
    }
}
*/