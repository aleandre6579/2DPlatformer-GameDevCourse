using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateManager : MonoBehaviour
{

    private Animator anim;
    private bool isUp = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    private void SlideUp()
    {
        isUp = true;
        anim.SetBool("slideUp", true);
    }
    private void SlideDown()
    {
        isUp = false;
        anim.SetBool("slideUp", false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isUp && collision.CompareTag("Player"))
        {
            SlideUp();
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isUp && collision.CompareTag("Player"))
        {
            SlideDown();
        }
    }
}
