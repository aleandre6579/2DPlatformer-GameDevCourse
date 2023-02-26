using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{


    private Animator anim;
    private bool isPressed = false;
    [SerializeField] private int collisionCnt = 0;
    [SerializeField] private int shotCnt = 0;

    [SerializeField] private float delayTime = 2;
    [SerializeField] private GameObject door;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void ButtonPressed()
    {
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
            collisionCnt++;
            if (!isPressed)
                ButtonPressed();
        }

        else if(collision.CompareTag("PlayerBullet") || collision.CompareTag("EnemyBullet"))
        {
            ButtonShot();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            collisionCnt--;
            if (collisionCnt + shotCnt == 0)
                ButtonUnpressed();
        }
    }
}
