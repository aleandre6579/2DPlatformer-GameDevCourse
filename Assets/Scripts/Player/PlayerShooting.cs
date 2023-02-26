using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    // Audio
    private AudioSource aSource;
    [SerializeField] private AudioClip shootSound;

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
        aSource= gameObject.AddComponent<AudioSource>();
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

        if (robotEye.transform.parent.rotation.y == 0) // If player rotation is towards right
        { 
            robotEye.transform.localPosition = new Vector3(xScaledPos,
                                                           yScaledPos,
                                                           0);
        }
        else // If player rotation is towards left
        { 
            robotEye.transform.localPosition = new Vector3(-xScaledPos,
                                                           yScaledPos,
                                                           0);
        }
    }


    private void Shoot(InputAction.CallbackContext context)
    {
        GameObject bulletInst = Instantiate(bulletPref, robotEye.transform.position, Quaternion.identity);

        Vector3 direction = new Vector3(mousePos.x - robotEye.transform.position.x, mousePos.y - robotEye.transform.position.y, 0);
        bulletInst.GetComponent<BulletManager>().direction = direction;
        
        aSource.PlayOneShot(shootSound, 0.8f);
    }
    

}
