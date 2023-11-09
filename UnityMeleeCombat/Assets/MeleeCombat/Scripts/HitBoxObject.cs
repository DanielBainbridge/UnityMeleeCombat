using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeleeCombatTool
{
    //The Monobehaviour call of a Hitbox, this is to make it so that a gameObject actually appears in our scene.
    public class HitBoxObject : MonoBehaviour
    {
        [HideInInspector] public HitBox m_hitbox;
        public List<HurtBoxObject> m_alreadyCollidedWithThisHurtBox = new List<HurtBoxObject>();
        private void Update()
        {
            if (m_hitbox.m_owner.m_debugHurtBoxes)
            {
                m_hitbox.UpdateDebugMeshSize();
            }
        }
        //hitboxes are triggers so as to not have rigid bodies move against them, this could have been done with manual collision checks to avoid rigidbodies entirely
        private void OnTriggerEnter(Collider collision)
        {
            //check if we collided with a hurt box if not early out
            if (collision.gameObject.GetComponent<HurtBoxObject>() == null)
            {
                m_hitbox.m_isColliding = false;
                return;
            }
            //get hurtbox reference
            HurtBoxObject collidingHurtBox = collision.gameObject.GetComponent<HurtBoxObject>();

            //check if we have already collided with this hurt box, if we have early out
            //for multi hit moves, more hitboxes need to be added to the move, rather than hitting the same one again.
            foreach (HurtBoxObject hBO in m_alreadyCollidedWithThisHurtBox)
            {
                if (hBO == collidingHurtBox)
                {
                    m_hitbox.m_isColliding = false;
                    return;
                }
            }

            //check we haven't hit our own hurt box, if we have early out
            if (collidingHurtBox.m_hurtbox.m_owner == m_hitbox.m_owner)
            {
                m_hitbox.m_isColliding = false;
                return;
            }

            //add collision to collision list and flag for resolution
            m_hitbox.m_collidingHurtBox = collidingHurtBox.m_hurtbox;
            m_hitbox.m_isColliding = true;
            m_alreadyCollidedWithThisHurtBox.Add(collidingHurtBox);
        }

    }
}