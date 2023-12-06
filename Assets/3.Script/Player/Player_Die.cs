using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Die : MonoBehaviour
{

    public Animator anim;
    private bool isDie = false;

    void Start()
    {
        anim = FindObjectOfType<Animator>();

    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Attack") && !isDie)
        {
            anim.SetTrigger("isDie");
            isDie = true;

            CapsuleCollider coll = transform.GetComponent<CapsuleCollider>();
            Rigidbody rig = transform.GetComponent<Rigidbody>();
            coll.isTrigger = true;
            rig.isKinematic = true;

        }
    }


}
