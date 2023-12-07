using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Die : MonoBehaviour
{

    public Animator anim;
    public CharacterController coll;
    public Rigidbody rig;
    private bool isDie = false;

    void Start()
    {
        anim = transform.GetChild(1).GetComponent<Animator>();
        coll = transform.GetComponent<CharacterController>();
        rig = transform.GetComponent<Rigidbody>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Attack") && !isDie)
        {
            Debug.Log("¸ÂÀ½ Ä®");
            anim.SetTrigger("isDie");
            isDie = true;


            coll.enabled = false;
            rig.isKinematic = true;

           // transform.gameObject.SetActive(false);

        }
    }


}
