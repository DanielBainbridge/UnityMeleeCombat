using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeleeCombatTool
{
    [RequireComponent(typeof(Animator))]
    public class CombatItem : MonoBehaviour
    {
        //the combat items stuff itself
        [SerializeField] private Animator m_animator;
        [SerializeField] public List<Move> m_moves = new List<Move>();
        [SerializeField] public List<HurtBox> m_hurtBoxes = new List<HurtBox>();
        [SerializeField] public bool m_debugHurtBoxes;
        [SerializeField] public bool m_debugHitBoxes;
        [SerializeField] private bool m_debugHurtBoxesLastFrame;
        [SerializeField] private bool m_debugHitBoxesLastFrame;
        private bool m_moveBeingUsed = false;

        private bool m_hitStopDone = false;
        private bool m_knockbackDone = false;


        private void OnEnable()
        {
            m_animator = GetComponent<Animator>();
            if (m_moves == null)
            {
                m_moves = new List<Move>();
            }
            if (m_hurtBoxes == null)
            {
                m_hurtBoxes = new List<HurtBox>();
            }
            m_debugHitBoxesLastFrame = m_debugHitBoxes;
            m_debugHurtBoxesLastFrame = m_debugHurtBoxes;
            for (int i = 0; i < 31; i++)
            {
                if (LayerMask.NameToLayer("Melee") != i)
                {
                    Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Melee"), i);
                }
            }
        }

        private void FixedUpdate()
        {
            ResolveCollisions();
        }

        //get animator
        public void UpdateAnimator()
        {
            m_animator = GetComponent<Animator>();
        }

        //check for collisions in a collision list
        public List<HitBox> CheckAllCollisions()
        {
            List<HitBox> collidingList = new List<HitBox>();
            foreach (Move m in m_moves)
            {
                foreach (HitBox hB in m.m_moveHitBoxes)
                {
                    if (hB.m_isColliding)
                    {
                        collidingList.Add(hB);
                    }
                }
            }
            return collidingList;
        }
        //stop the timeScale for seconds based on the hit boxes values, or automatic 
        public void DoHitStop(HitBox hitBox)
        {
            if (hitBox.m_automaticHitStop)
            {
                float stopTime = (float)((hitBox.m_endFrame - hitBox.m_startFrame) / hitBox.m_endFrame) * hitBox.m_hitStopMultiplier;
                StartCoroutine(HitStopForSeconds(stopTime));
            }
            else
            {
                StartCoroutine(HitStopForSeconds(hitBox.m_hitStopLength));
            }
        }
        //stop time scale for an amount of seconds
        private IEnumerator HitStopForSeconds(float time)
        {
            m_hitStopDone = false;
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(time); //need to use real time otherwise won't complete
            Time.timeScale = 1;
            m_hitStopDone = true;
        }

        //do knockback, using raw or calculated values
        public void DoKnockBack(HitBox hitBox)
        {
            HurtBox receivingHurtBox = hitBox.m_collidingHurtBox;

            //conditional displacement angle based on if it should be automatically calculated
            Vector3 displacementAngle = hitBox.m_automaticKnockbackAngle ?
                Vector3.Normalize(receivingHurtBox.m_hurtBoxObject.transform.position - hitBox.m_hitBoxObject.transform.position) :
                new Vector3(Mathf.Sin(hitBox.m_knockbackAngle.x), Mathf.Sin(hitBox.m_knockbackAngle.y), Mathf.Sin(hitBox.m_knockbackAngle.z));

            Vector3 goal = displacementAngle * hitBox.m_knockbackDistance;

            if (receivingHurtBox.m_owner.GetComponent<Rigidbody>() != null)
            {
                Rigidbody receivingBody = receivingHurtBox.m_owner.GetComponent<Rigidbody>();
                // physics knockback
                StartCoroutine(KnockBackWithForce(receivingBody, hitBox.m_knockbackTime, goal));
            }
            else
            {
                // raw translate
                StartCoroutine(KnockBackRaw(receivingHurtBox.m_owner.transform, hitBox.m_knockbackTime, goal, hitBox));
            }

        }

        //do knockback if there is a rigidbody on the combat item
        private IEnumerator KnockBackWithForce(Rigidbody rB, float totalTime, Vector3 goal)
        {
            m_knockbackDone = false;
            if (totalTime == 0)
            {
                rB.transform.Translate(goal);
                m_knockbackDone = true;
                yield break;
            }

            float timeIncremement = 0;
            rB.AddForce(2 * goal / totalTime, ForceMode.VelocityChange);
            while (timeIncremement < totalTime)
            {
                rB.AddForce(-2 * goal / totalTime / totalTime, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
                timeIncremement += Time.fixedDeltaTime;
            }
            m_knockbackDone = true;
        }

        //do knockback if there isn't a rigidbody on the combat item
        private IEnumerator KnockBackRaw(Transform obj, float totalTime, Vector3 goal, HitBox hitBox)
        {
            m_knockbackDone = false;
            float timeIncremement = 0;
            Vector3 objOriginalPos = obj.position;
            if (totalTime == 0)
            {
                obj.Translate(goal);
                yield return null;
            }
            while (timeIncremement < totalTime)
            {
                obj.position = Vector3.Lerp(objOriginalPos, objOriginalPos + goal, timeIncremement / totalTime);
                yield return 0;
                timeIncremement += Time.deltaTime;
            }
            m_knockbackDone = true;
        }
        //gets the damage from a hit box, this is an etension function to be used later
        public float GetDamage(HitBox hitBox)
        {
            if (hitBox.m_automaticDamageCalculation)
            {
                float invFrameLength = 1 / (float)(hitBox.m_endFrame - hitBox.m_startFrame);
                invFrameLength += 1;
                return hitBox.m_endFrame * invFrameLength;
            }
            return hitBox.m_damage;
        }
        //standard coroutine 
        public IEnumerator HitBoxStandardSolve(HitBox hB)
        {
            //stops hitbox from being solved twice
            OnHitReaction(hB);
            hB.m_isColliding = false;
            DoHitStop(hB);
            yield return new WaitWhile(GetHitStopDone);
            DoKnockBack(hB);
            yield return new WaitWhile(GetKnockBackDone);
        }
        //flags for if a move has had hit stop completed
        private bool GetHitStopDone()
        {
            return !m_hitStopDone;
        }
        //flags for if a move has had knock back completed
        private bool GetKnockBackDone()
        {
            return !m_knockbackDone;
        }

        //resolve hit box collisions
        virtual public void ResolveCollisions()
        {
            List<HitBox> collidingBoxes = CheckAllCollisions();
            foreach (HitBox hB in collidingBoxes)
            {
                StartCoroutine(HitBoxStandardSolve(hB));
            }
        }

        //Virtual function, not pure virtual in case you don't want to do anything in particular in reaction to being hit
        virtual public void OnHitReaction(HitBox hB)
        {
            return;
        }

        //Add in a hurt box onto the combat item
        public void AddHurtBoxBox()
        {
            HurtBox newHurtBox = new HurtBox(HurtBox.Shape.Box, transform);
            newHurtBox.NameHurtBoxObject($"Hurt Box {m_hurtBoxes.Count + 1}: Box");

            m_hurtBoxes.Add(newHurtBox);
        }
        //Add in a hurt box onto the combat item
        public void AddHurtBoxSphere()
        {
            HurtBox newHurtBox = new HurtBox(HurtBox.Shape.Sphere, transform);
            newHurtBox.NameHurtBoxObject($"Hurt Box {m_hurtBoxes.Count + 1}: Sphere");

            m_hurtBoxes.Add(newHurtBox);
        }
        //Add in a hurt box onto the combat item
        public void AddHurtBoxCapsule()
        {
            HurtBox newHurtBox = new HurtBox(HurtBox.Shape.Capsule, transform);
            newHurtBox.NameHurtBoxObject($"Hurt Box {m_hurtBoxes.Count + 1}: Capsule");

            m_hurtBoxes.Add(newHurtBox);
        }
        // remove one hurt box
        public void RemoveHurtBox()
        {
            if (m_hurtBoxes.Count != 0)
            {
                m_hurtBoxes[m_hurtBoxes.Count - 1].DestroyHurtBox();
                m_hurtBoxes.RemoveAt(m_hurtBoxes.Count - 1);
            }
        }
        // remove all hurt boxes
        public void ClearHurtBoxes()
        {
            foreach (HurtBox hB in m_hurtBoxes)
            {
                hB.DestroyHurtBox();
            }
            m_hurtBoxes.Clear();
        }
        //find the move to use and then begin using the move
        public void UseMove(int moveLocation)
        {
            if (!m_moveBeingUsed)
            {
                StartCoroutine(UseMoveCoroutine(moveLocation));
            }
        }

        //feels like it should be inside of move but coroutines can only be called on monobehaviours gross
        private IEnumerator UseMoveCoroutine(int moveLocation)
        {
            m_moveBeingUsed = true;
            float secondsToWait = 1 / m_moves[moveLocation].GetMoveFrameRate();
            if (m_moves[moveLocation].GetCurrentAnimationFrame() == 0)
            {
                m_moves[moveLocation].StartMove();
            }

            while (m_moves[moveLocation].GetCurrentAnimationFrame() <= m_moves[moveLocation].GetTotalAnimationFrames())
            {
                m_moves[moveLocation].ConstructHitBoxesPerFrame();
                m_moves[moveLocation].DestroyHitBoxesPerFrame();

                //waits 1 animation frame, not 1 in game frame
                yield return new WaitForSeconds(secondsToWait);
                //move forward one frame for next loop
                m_moves[moveLocation].IncrementAnimationFrame();
            }

            yield return new WaitForSeconds(secondsToWait * m_moves[moveLocation].GetMoveFrameRate());
            m_moves[moveLocation].ResetCurrentFrame();
            m_moves[moveLocation].StopMove();
            m_moveBeingUsed = false;
        }

        //toggle debug if it was changed last frame
        public void DebugBoxCheck()
        {
            if (m_debugHurtBoxes != m_debugHurtBoxesLastFrame)
            {
                foreach (HurtBox hB in m_hurtBoxes)
                {
                    hB.DebugHurtBoxesSwitch(m_debugHurtBoxes);
                }
            }
            if (m_debugHitBoxes != m_debugHitBoxesLastFrame)
            {
                foreach (Move move in m_moves)
                {
                    foreach (HitBox hB in move.m_moveHitBoxes)
                    {
                        hB.DebugHitBoxesSwitch(m_debugHitBoxes);
                    }
                }
            }
            m_debugHitBoxesLastFrame = m_debugHitBoxes;
            m_debugHurtBoxesLastFrame = m_debugHurtBoxes;
        }
        //toggle the debug value
        public void ToggleDebug()
        {
            m_debugHitBoxes = !m_debugHitBoxes;
            m_debugHurtBoxes = !m_debugHurtBoxes;
        }

        //add move to a combat item
        public void AddMove()
        {
            Move newMove = new Move();
            newMove.m_animator = m_animator;
            m_moves.Add(newMove);
        }

        //remove a move from a combat item
        public void RemoveMove()
        {
            if (m_moves.Count != 0)
                m_moves.RemoveAt(m_moves.Count - 1);
        }
        //add hitboxes to a move
        public void AddHitBoxToMove(int moveNumber)
        {
            m_moves[moveNumber].m_moveHitBoxes.Add(new HitBox());
        }
        //remove a hitbox from a move
        public void RemoveHitBoxFromMove(int moveNumber)
        {
            if (m_moves.Count != 0 && m_moves[moveNumber].m_moveHitBoxes.Count != 0)
                m_moves[moveNumber].m_moveHitBoxes.RemoveAt(m_moves[moveNumber].m_moveHitBoxes.Count - 1);
        }
        //update the data of an move based on the animator plugged in
        public void UpdateMove()
        {
            foreach (Move m in m_moves)
            {
                if (m.m_moveAnimation != null)
                {
                    m.m_moveName = m.m_moveAnimation.name;
                    m.SetTotalFrames();
                    if (!m.IsMoveInAnimator())
                    {
                        m.AddMoveToAnimator();
                    }
                }
            }
        }
    }
}

namespace MeleeCombatTool.Editors
{
#if UNITY_EDITOR
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using Unity.VisualScripting;
    using UnityEngine.Assertions;
#endif
    [CustomEditor(typeof(CombatItem))]
    public class CombatItemEditor : Editor
    {
        //properties
        SerializedProperty m_animator;
        SerializedProperty m_moves;
        SerializedProperty m_hurtBoxes;

        //actual items
        CombatItem m_thisCombatItem;
        List<HurtBox> m_thisCombatItemHurtBoxes;
        List<Move> m_thisCombatItemMoves;
        public void OnEnable()
        {
            //finds and defines serialized variables from the target object
            m_animator = serializedObject.FindProperty("m_animator");
            m_moves = serializedObject.FindProperty("m_moves");
            m_hurtBoxes = serializedObject.FindProperty("m_hurtBoxes");
            //defines variable as a Combat Item
            m_thisCombatItem = target.GetComponent<CombatItem>();
            m_thisCombatItem.UpdateAnimator();
            m_thisCombatItemHurtBoxes = m_thisCombatItem.m_hurtBoxes;
            m_thisCombatItemMoves = m_thisCombatItem.m_moves;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //Label field is a way to label without having fieldds that have their own names
            EditorGUILayout.LabelField("Base Animator", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_animator);
            //space puts vertical space between editor layouts
            EditorGUILayout.Space(10);

            if (m_animator.objectReferenceValue == null)
            {
                return;
            }

            // Hurt Boxes
            EditorGUILayout.LabelField("Hurt Box Editing", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            //creates a button in style that aligns button to a side
            EditorGUILayout.LabelField("Hurt Box Adding");

            //creates a horizontal group
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Add Hurt Box: Box +", EditorStyles.miniButton))
            {
                m_thisCombatItem.AddHurtBoxBox();
            }
            if (GUILayout.Button("+ Add Hurt Box: Sphere +", EditorStyles.miniButton))
            {
                m_thisCombatItem.AddHurtBoxSphere();
            }
            if (GUILayout.Button("+ Add Hurt Box: Capsule +", EditorStyles.miniButton))
            {
                m_thisCombatItem.AddHurtBoxCapsule();
            }
            //ends horizontal group for buttons

            GUILayout.EndHorizontal();
            GUILayout.Space(7);


            EditorGUILayout.LabelField("Hurt Box Removal");
            GUILayout.BeginHorizontal();
            //creates a button in style that aligns button to a side
            if (GUILayout.Button("- Remove Hurt Box -", EditorStyles.miniButton))
            {
                m_thisCombatItem.RemoveHurtBox();
                m_thisCombatItemHurtBoxes = m_thisCombatItem.m_hurtBoxes;
            }
            if (GUILayout.Button("- Clear Hurt Boxes -", EditorStyles.miniButton))
            {
                m_thisCombatItem.ClearHurtBoxes();
                m_thisCombatItemHurtBoxes = m_thisCombatItem.m_hurtBoxes;
            }
            //ends horizontal group for buttons
            GUILayout.EndHorizontal();

            for (int i = 0; i < m_hurtBoxes.arraySize; i++)
            {
                //get current hurt box
                SerializedProperty currentHurtBox = m_hurtBoxes.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(currentHurtBox, new GUIContent($"Hurt Box: {i + 1} {currentHurtBox.FindPropertyRelative("m_shape").enumNames[currentHurtBox.FindPropertyRelative("m_shape").enumValueIndex]}"), false);
                if (!currentHurtBox.isExpanded)
                {
                    continue;
                }
                EditorGUI.indentLevel++;
                switch ((currentHurtBox.FindPropertyRelative("m_shape").enumValueIndex))
                {
                    case 0:
                        EditorGUILayout.PropertyField(currentHurtBox.FindPropertyRelative("m_width"));
                        EditorGUILayout.PropertyField(currentHurtBox.FindPropertyRelative("m_height"));
                        EditorGUILayout.PropertyField(currentHurtBox.FindPropertyRelative("m_depth"));
                        EditorGUILayout.PropertyField(currentHurtBox.FindPropertyRelative("m_center"));
                        break;
                    case 1:
                        EditorGUILayout.PropertyField(currentHurtBox.FindPropertyRelative("m_radius"));
                        EditorGUILayout.PropertyField(currentHurtBox.FindPropertyRelative("m_center"));
                        break;

                    case 2:
                        EditorGUILayout.PropertyField(currentHurtBox.FindPropertyRelative("m_radius"));
                        EditorGUILayout.PropertyField(currentHurtBox.FindPropertyRelative("m_height"));
                        EditorGUILayout.PropertyField(currentHurtBox.FindPropertyRelative("m_center"));
                        break;
                }
                EditorGUILayout.PropertyField(currentHurtBox.FindPropertyRelative("m_hurtBoxParentObject"));

                EditorGUI.indentLevel--;
            }
            for (int i = 0; i < m_thisCombatItemHurtBoxes.Count; i++)
            {
                m_thisCombatItemHurtBoxes[i].UpdateHurtBoxObject();
            }


            // Hit Boxes

            //more spacing
            EditorGUILayout.Space(10);
            //more header
            EditorGUILayout.LabelField("Hit Box Editing", EditorStyles.boldLabel);
            if (GUILayout.Button(" + Add Move + "))
            {
                m_thisCombatItem.AddMove();
                m_thisCombatItemMoves = m_thisCombatItem.m_moves;
            }
            if (GUILayout.Button(" - Remove Move - "))
            {
                m_thisCombatItem.RemoveMove();
                m_thisCombatItemMoves = m_thisCombatItem.m_moves;
            }
            EditorGUILayout.Space(5);

            for (int i = 0; i < m_thisCombatItemMoves.Count - 1; i++)
            {
                SerializedProperty currentMove = m_moves.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(currentMove, new GUIContent($"Move: {i + 1}"), false);
                if (!currentMove.isExpanded)
                {
                    continue;
                }
                SerializedProperty currentMoveAnimation = currentMove.FindPropertyRelative("m_moveAnimation");
                EditorGUILayout.PropertyField(currentMoveAnimation);
                m_thisCombatItem.UpdateMove();
                if (currentMoveAnimation.objectReferenceValue == null)
                {
                    continue;
                }

                EditorGUILayout.Space(10);
                SerializedProperty moveHitBoxesProperties = currentMove.FindPropertyRelative("m_moveHitBoxes");
                //horizontal button group
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(" + Add Hit Box + ", EditorStyles.miniButton))
                {
                    m_thisCombatItem.AddHitBoxToMove(i);
                }
                if (GUILayout.Button(" - Remove Hit Box - ", EditorStyles.miniButton))
                {
                    m_thisCombatItem.RemoveHitBoxFromMove(i);
                }
                AnimationClip curAnimation = (AnimationClip)currentMoveAnimation.objectReferenceValue;

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField($" \"{curAnimation.name}\" Hit Boxes:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                for (int j = 0; j < moveHitBoxesProperties.arraySize; j++)
                {
                    SerializedProperty currentHitbox = moveHitBoxesProperties.GetArrayElementAtIndex(j);
                    EditorGUILayout.PropertyField(currentHitbox, new GUIContent($"Hit Box: {j + 1}"), false);
                    if (!moveHitBoxesProperties.GetArrayElementAtIndex(j).isExpanded)
                    {
                        continue;
                    }
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_parentTransform"));
                    //shape stuff
                    EditorGUILayout.Space(5);
                    EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_shape"));
                    EditorGUI.indentLevel++;

                    switch ((moveHitBoxesProperties.GetArrayElementAtIndex(j).FindPropertyRelative("m_shape").enumValueIndex))
                    {
                        case 0:
                            EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_width"));
                            EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_height"));
                            EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_depth"));
                            break;
                        case 1:
                            EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_radius"));
                            break;

                        case 2:
                            EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_radius"));
                            EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_height"));
                            break;
                    }
                    EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_offset"));
                    EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_rotationOffset"));
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space(10);

                    //no longer shape stuff

                    //animation frame stuff
                    EditorGUILayout.IntSlider(currentHitbox.FindPropertyRelative("m_startFrame"), 0, (int)(curAnimation.length * curAnimation.frameRate));
                    EditorGUILayout.IntSlider(currentHitbox.FindPropertyRelative("m_endFrame"), 0, (int)(curAnimation.length * curAnimation.frameRate));

                    //error if the start frame is before the end frame in the inspector to stop people breaking code
                    Assert.IsTrue(currentHitbox.FindPropertyRelative("m_startFrame").intValue < currentHitbox.FindPropertyRelative("m_endFrame").intValue,
                        $"Start Frame on Move: {i + 1} Hit Box: {j + 1} cannot be later than the end frame!");

                    SerializedProperty autoDamage = currentHitbox.FindPropertyRelative("m_automaticDamageCalculation");
                    EditorGUILayout.PropertyField(autoDamage, new GUIContent("Auto Damage"));
                    if (!autoDamage.boolValue)
                    {
                        EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_damage"));
                    }

                    SerializedProperty autoKnockback = currentHitbox.FindPropertyRelative("m_automaticKnockbackAngle");
                    EditorGUILayout.PropertyField(autoKnockback, new GUIContent("Auto Knockback"));
                    if (!autoKnockback.boolValue)
                    {
                        EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_knockbackAngle"));
                    }
                    EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_knockbackDistance"));
                    EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_knockbackTime"));
                    EditorGUILayout.Space(5);

                    SerializedProperty autoHitStop = currentHitbox.FindPropertyRelative("m_automaticHitStop");
                    EditorGUILayout.PropertyField(autoHitStop, new GUIContent("Auto Hit Stop"));
                    if (!autoHitStop.boolValue)
                    {
                        EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_hitStopLength"));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(currentHitbox.FindPropertyRelative("m_hitStopMultiplier"));
                    }


                    EditorGUI.indentLevel--;
                }


                if (GUILayout.Button($"Use Move: {m_thisCombatItemMoves[i].m_moveName}"))
                {
                    m_thisCombatItem.UseMove(i);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space(10);


            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_debugHurtBoxes"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_debugHitBoxes"));


            m_thisCombatItem.DebugBoxCheck();

            serializedObject.ApplyModifiedProperties();
        }
    }
}