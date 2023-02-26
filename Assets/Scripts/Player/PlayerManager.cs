using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    // Audio
    private AudioSource aSource;
    [SerializeField] private AudioClip death;

    private Animator anim;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;
    private DontDestroy dontDestroy;

    [SerializeField] GameObject robotEye;
    public int health;

    private GameObject bullet;
    private bool isDead = false;

    [SerializeField] private GameObject[] hearts;
    public float currTime;
    [SerializeField] private TextMeshProUGUI timer;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
        aSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        dontDestroy = GameObject.Find("DontDestroy").GetComponent<DontDestroy>();
        Debug.Log(dontDestroy.health);
        currTime = dontDestroy.time;
        health = dontDestroy.health;
        for (int i = 0; i < 3; i++)
        {
            hearts[i].SetActive(false);
        }
        for (int i = 0; i < health; i++)
        {
            hearts[i].SetActive(true);
        }
    }

    private void Update()
    {
        currTime += Time.deltaTime;
        timer.text = Mathf.Round(currTime).ToString();
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
        dontDestroy.health -= damage;
        hearts[health].SetActive(false);
        if (health <= 0)
        {
            IsDead();
        }
    }

    private void IsDead()
    {
        aSource.PlayOneShot(death, 0.5f);
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
        dontDestroy.time = currTime;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
