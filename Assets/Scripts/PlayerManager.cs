using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;

    [SerializeField] GameObject robotEye;
    [SerializeField] private int health;

    private GameObject bullet;
    private bool isDead = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            if (isDead) return;
            bullet = collision.gameObject;
            IsHit(bullet.GetComponent<BulletManager>().damage);
        }
    }

    private void IsHit(int damage)
    {
        Destroy(bullet.gameObject);
        health -= damage;
        if (health <= 0)
        {
            IsDead();
        }
    }


    private void IsDead()
    {
        isDead = true;
        robotEye.SetActive(false);
        anim.SetTrigger("dead");
        gameObject.layer = LayerMask.NameToLayer("Default");
        playerMovement.enabled = false;
        playerShooting.enabled = false;
        this.enabled = false;
    }





}
