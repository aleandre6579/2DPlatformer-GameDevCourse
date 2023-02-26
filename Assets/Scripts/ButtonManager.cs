using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    private AudioSource aSource;
    [SerializeField] private AudioClip press;

    [SerializeField] private bool isOnePress;
    private Animator anim;
    private bool isPressed = false;
    [SerializeField] private int collisionCnt = 0;
    [SerializeField] private int shotCnt = 0;

    [SerializeField] private float delayTime = 2;
    [SerializeField] private GameObject door;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        aSource = GetComponent<AudioSource>();
    }

    private void ButtonPressed()
    {
        aSource.PlayOneShot(press, 1f);
        anim.SetBool("isPressed", true);
        isPressed = true;
        door.SetActive(false);
    }
    private void ButtonUnpressed()
    {
        anim.SetBool("isPressed", false);
        isPressed = false;
        door.SetActive(true);
    }

    private void ButtonShot()
    {
        shotCnt++;
        ButtonPressed();
        StartCoroutine("PressedDelay");
    }

    private IEnumerator PressedDelay()
    {
        yield return new WaitForSeconds(delayTime);
        shotCnt--;
        if (collisionCnt + shotCnt == 0)
            ButtonUnpressed();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            if(!isOnePress)
            {
                collisionCnt++;
                if (!isPressed)
                    ButtonPressed();
            }
            else if(!isPressed)
            {
                ButtonPressed();
            }
        }

        else if(collision.CompareTag("PlayerBullet") || collision.CompareTag("EnemyBullet"))
        {
            if(!isOnePress)
                ButtonShot();
            else if(!isPressed)
                ButtonPressed();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            if(!isOnePress)
            {
                collisionCnt--;
                if (collisionCnt + shotCnt == 0)
                    ButtonUnpressed();
            }
        }
    }
}
