using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DroneManager : Enemy
{
    private Transform bullet;

    // Components
    private DroneMovement droneMovement;
    private Animator anim;
    private Rigidbody2D rb;

    [SerializeField] float deathForce = 5;

    // Shooting
    [SerializeField] private GameObject bulletPref;
    [SerializeField] private Transform shootpoint;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float reloadTime;
    private Coroutine shootRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        droneMovement = GetComponent<DroneMovement>();
        anim = GetComponent<Animator>();
    }


    // Shooting
    public void StartShooting()
    {
        shootRoutine = StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(reloadTime);
        GameObject bulletInst = Instantiate(bulletPref, shootpoint.transform.position, Quaternion.identity);
        bulletInst.GetComponent<BulletManager>().direction = Vector3.down;
        shootRoutine = StartCoroutine(Shoot());
    }

    public void StopShooting()
    {
        StopCoroutine(shootRoutine);
    }


    // Collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("PlayerBullet"))
        {
            bullet = collision.transform;
            IsHit(bullet.GetComponent<BulletManager>().damage);
        }
    }

    private void IsHit(int damage)
    {
        Destroy(bullet.gameObject);
        health -= damage;
        if(health <= 0)
        {
            IsDead();
        }
    }


    private void IsDead()
    {
        anim.SetBool("isDead", true);
        droneMovement.IsDead();
        rb.bodyType = RigidbodyType2D.Dynamic;
        Vector3 bulletDir = bullet.position - transform.position;
        rb.AddForce(-bulletDir * deathForce, ForceMode2D.Impulse);
        droneMovement.enabled = false;
        this.enabled = false;
    }




}
