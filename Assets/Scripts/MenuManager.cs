using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject page1;
    [SerializeField] private GameObject page2;

    public void PlayGame()
    {
        page1.SetActive(false);
        page2.SetActive(true);
    }

    public void Back()
    {
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
