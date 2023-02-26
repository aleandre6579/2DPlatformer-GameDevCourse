using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private AudioSource aSource;
    [SerializeField] private AudioClip buttonClick1;
    [SerializeField] private AudioClip buttonClick2;

    [SerializeField] private GameObject page1;
    [SerializeField] private GameObject page2;


    private void Awake()
    {
        aSource= GetComponent<AudioSource>();
    }
    public void PlayGame()
    {
        aSource.PlayOneShot(buttonClick2, 0.2f);
        page1.SetActive(false);
        page2.SetActive(true);
    }

    public void Back()
    {
        aSource.PlayOneShot(buttonClick1, 0.5f);
        page1.SetActive(true);
        page2.SetActive(false);
    }

    public void PlayLevel(int lvl)
    {
        SceneManager.LoadScene(lvl);
    }

    public void ExitGame()
    {
        Application.Quit();
    }




}
