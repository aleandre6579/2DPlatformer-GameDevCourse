using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] GameObject robotEye;
    [SerializeField] Transform shootingPoint;
    [SerializeField] GameObject bulletPref;

    // Eye Movement
    private Vector3 mousePos;
    private Vector3 mouseToEyeDir;
    [SerializeField] Vector2 xEyeBounds;
    [SerializeField] Vector2 yEyeBounds;

    // Inputs
    private PlayerInputActions playerActions;
    private InputAction shoot;

    private void Awake()
    {
        playerActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        shoot = playerActions.Player.Fire;
        shoot.Enable();
        shoot.performed += Shoot;
    }

    private void OnDisable()
    {
        shoot.Disable();
    }

    private void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 mouseDirection = mousePos - shootingPoint.position;

        float xScaledPos = Mathf.Clamp(shootingPoint.localPosition.x + mouseDirection.x / 50, xEyeBounds.x, xEyeBounds.y);
        float yScaledPos = Mathf.Clamp(shootingPoint.localPosition.y + mouseDirection.y / 50, yEyeBounds.x, yEyeBounds.y);

        robotEye.transform.localPosition = new Vector3(xScaledPos, 
                                                       yScaledPos, 
                                                       0);
    }


    private void Shoot(InputAction.CallbackContext context)
    {
        GameObject bulletInst = Instantiate(bulletPref, robotEye.transform.position, Quaternion.identity);

        Vector3 direction = new Vector3(mousePos.x - robotEye.transform.position.x, mousePos.y - robotEye.transform.position.y, 0);
        bulletInst.GetComponent<BulletManager>().direction = direction;


    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(robotEye.transform.position, mousePos);
    }

}
