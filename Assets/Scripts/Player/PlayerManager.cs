using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        else if(collision.CompareTag("Gate"))
        {
            playerMovement.atGate = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Gate"))
        {
            playerMovement.atGate = false;
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
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
