using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.UI;
using Mono.Cecil.Cil;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

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
            if (GUILayout.Button(" + Add Move + "))
            {
                m_thisCombatItem.AddMove();
            }
            if (GUILayout.Button(" - Remove Move - "))
            {
                m_thisCombatItem.RemoveMove();
            }
            EditorGUILayout.Space(5);

            for (int i = 0; i < m_moves.arraySize; i++)
            {
                bool foldoutHeaderMoves = true;
                foldoutHeaderMoves = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutHeaderMoves, "Move: " + (i + 1));

                //EditorGUILayout.LabelField("Move: " + (i + 1), EditorStyles.boldLabel);
                if (foldoutHeaderMoves)
                {
                    SerializedProperty currentMoveAnimation = m_moves.GetArrayElementAtIndex(i).FindPropertyRelative("m_moveAnimation");
                    EditorGUILayout.PropertyField(currentMoveAnimation);

                    if (currentMoveAnimation.objectReferenceValue != null)
                    {
                        EditorGUILayout.Space(10);
                        SerializedProperty moveHitBoxesProperties = m_moves.GetArrayElementAtIndex(i).FindPropertyRelative("m_moveHitBoxes");
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(" + Add Hit Box + ", EditorStyles.miniButtonLeft))
                        {
                            m_thisCombatItem.AddHitBoxToMove(i);
                        }
                        if (GUILayout.Button(" - Remove Hit Box - ", EditorStyles.miniButtonRight))
                        {
                            m_thisCombatItem.RemoveHitBoxFromMove(i);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.PropertyField(moveHitBoxesProperties, false);
                        for (int j = 0; j < moveHitBoxesProperties.arraySize; j++)
                        {
                            EditorGUILayout.LabelField("Hit Box: " + (j + 1), EditorStyles.boldLabel);

                            EditorGUILayout.PropertyField(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_parentTransform"));

                            //shape stuff
                            EditorGUILayout.Space(5);
                            EditorGUILayout.PropertyField(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_shape"));
                            EditorGUI.indentLevel++;

                            switch ((moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_shape").enumValueIndex))
                            {

                                case 0:
                                    EditorGUILayout.PropertyField(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_width"));
                                    EditorGUILayout.PropertyField(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_height"));
                                    EditorGUILayout.PropertyField(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_depth"));
                                    break;

                                case 1:
                                    EditorGUILayout.PropertyField(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_radius"));
                                    break;

                                case 2:
                                    EditorGUILayout.PropertyField(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_radius"));
                                    EditorGUILayout.PropertyField(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_height"));
                                    break;
                            }

                            EditorGUI.indentLevel--;
                            EditorGUILayout.Space(10);

                            //no longer shape stuff

                            //animation frame stuff
                            AnimationClip curAnimation = (AnimationClip)currentMoveAnimation.objectReferenceValue;
                            EditorGUILayout.IntSlider(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_startFrame"), 0, (int)(curAnimation.length * curAnimation.frameRate));
                            EditorGUILayout.IntSlider(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_endFrame"), 0, (int)(curAnimation.length * curAnimation.frameRate));
                            Assert.IsTrue(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_startFrame").intValue < moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_endFrame").intValue,
                                $"Start Frame on Move: {i + 1} Hit Box: {j + 1} cannot be later than the end frame!");



                            SerializedProperty autoDamage = moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_automaticDamageCalculation");
                            EditorGUILayout.PropertyField(autoDamage);
                            if (!autoDamage.boolValue)
                            {
                                EditorGUILayout.PropertyField(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_damage"));
                            }

                            SerializedProperty autoKnockback = moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_automaticKnockbackAngle");
                            EditorGUILayout.PropertyField(autoKnockback);
                            if (!autoKnockback.boolValue)
                            {
                                EditorGUILayout.PropertyField(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_knockbackAngle"));
                            }
                            EditorGUILayout.PropertyField(moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_knockbackDistance"));
                            EditorGUILayout.Space(5);
                        }
                    }
                    EditorGUILayout.Space(10);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
