using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.UI;

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
        m_thisCombatItem = target.GetComponent<CombatItem>();
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
            EditorGUILayout.Space(5);
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

            EditorGUILayout.PropertyField(m_hurtBoxes);

            //more spacing
            EditorGUILayout.Space(10);
            //more header
            EditorGUILayout.LabelField("Hit Box Editing", EditorStyles.boldLabel);
            if(GUILayout.Button("Add Move"))
            {
                m_thisCombatItem.AddMove();
            }
            if(GUILayout.Button("Remove Move"))
            {
                m_thisCombatItem.RemoveMove();
            }
            EditorGUILayout.Space(5);
            
            for(int i = 0; i < m_moves.arraySize; i++)
            {
                EditorGUILayout.LabelField("Move: " + (i + 1), EditorStyles.boldLabel);
                SerializedProperty currentMoveAnimation = m_moves.GetArrayElementAtIndex(i).FindPropertyRelative("m_moveAnimation");
                EditorGUILayout.PropertyField(currentMoveAnimation);
                if(currentMoveAnimation.objectReferenceValue != null)
                {
                    EditorGUILayout.PropertyField(m_moves.GetArrayElementAtIndex(i).FindPropertyRelative("m_moveHitBoxes"), false);
                }
            }

        }
        serializedObject.ApplyModifiedProperties();
    }
}
