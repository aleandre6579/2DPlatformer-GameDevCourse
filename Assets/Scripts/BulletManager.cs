using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [HideInInspector]
    public Vector3 direction;
    [SerializeField] float bulletSpeed;
    [SerializeField] float bulletLifespan;

    private void Start()
    {
        Invoke("KillBullet", bulletLifespan);
    }

    private void Update()
    {

        transform.position += direction.normalized * bulletSpeed;
    }

    private void KillBullet()
    {
        Destroy(gameObject);
    }

}
