using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxObject : MonoBehaviour
{
    [HideInInspector] public HitBox m_hitbox;

    private void OnTriggerEnter(Collider collision)
    {
        //check if we collided with a hurt box
        if(collision.gameObject.GetComponent<HurtBoxObject>() == null)
        {
            return;
        }

        HurtBoxObject collidingHurtBox = collision.gameObject.GetComponent<HurtBoxObject>();

        //check we haven't hit our own hurt box
        if(collidingHurtBox.m_hurtbox.m_owner == m_hitbox.m_owner)
        {
            return;
        }

        // TO DO resolve collisions we actually care about


    }

}
