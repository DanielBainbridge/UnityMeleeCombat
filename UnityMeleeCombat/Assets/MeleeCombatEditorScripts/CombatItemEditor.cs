using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor(typeof(CombatItem))]
public class CombatItemEditor : Editor
{
    SerializedProperty m_animator;
    SerializedProperty m_moves;
    SerializedProperty m_hurtBoxes;
    CombatItem m_thisCombatItem;
    public void OnEnable()
    {
        //finds and defines serialized variables from the target object
        m_animator = serializedObject.FindProperty("m_animator");
        m_moves = serializedObject.FindProperty("m_moves");
        m_hurtBoxes = serializedObject.FindProperty("m_hurtBoxes");
        //defines variable as a Combat Item
        m_thisCombatItem = (CombatItem)target;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //Label field is a way to label without having fieldds that have their own names
        EditorGUILayout.LabelField("Base Animator", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_animator);
        //space puts vertical space between editor layouts
        EditorGUILayout.Space(10);

        if (m_animator.objectReferenceValue != null)
        {
            //heading label
            EditorGUILayout.LabelField("Hurt Box Editing", EditorStyles.boldLabel);
            //creates a horizontal group for the next two buttons
            GUILayout.BeginHorizontal();
            //creates a button in style that aligns button to a side
            if (GUILayout.Button("Auto-Generate Hurt Boxes", EditorStyles.miniButtonLeft))
            {
                m_thisCombatItem.GenerateHurtBoxes();
            }
            if (GUILayout.Button("Add Hurt Box", EditorStyles.miniButtonRight))
            {
                m_thisCombatItem.AddHurtBox();
            }
            //ends horizontal group for buttons
            GUILayout.EndHorizontal();
            
            if(m_hurtBoxes.arraySize != 0)
            {
                EditorGUILayout.PropertyField(m_hurtBoxes);
            }
            //more spacing
            EditorGUILayout.Space(10);
            //more header
            EditorGUILayout.LabelField("Hit Box Editing", EditorStyles.boldLabel);
            if(GUILayout.Button("Add Move To Combat Item", EditorStyles.miniButton))
            {
                m_thisCombatItem.AddMove();
            }

            if(m_moves.arraySize != 0)
            {
                EditorGUILayout.PropertyField(m_moves);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
