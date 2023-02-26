using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Animator panelAnim;
    private PlayerInputActions playerActions;
    private InputAction escape;
    private bool isOpen = false;

    private void Awake()
    {
        playerActions = new PlayerInputActions();
    }
    private void Start()
    {

        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        escape = playerActions.Player.Escape;
        escape.Enable();
        escape.started += ToggleSettings;
    }

    private void OnDisable()
    {
        escape.Disable();
    }

    public void Restart()
    {
        StartCoroutine(StartRestart());
    }

    private IEnumerator StartRestart()
    {
        panelAnim.SetBool("fadeIn", true);
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ToggleSettings(InputAction.CallbackContext context)
    {
        isOpen = !isOpen;
        transform.GetChild(0).gameObject.SetActive(isOpen);
    }
}
