using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndManager : MonoBehaviour
{

    private DontDestroy dontDestroy;
    [SerializeField] private TextMeshProUGUI timer;

    private void Start()
    {
        dontDestroy = GameObject.Find("DontDestroy").GetComponent<DontDestroy>();
        timer.text = Mathf.Round(dontDestroy.time).ToString();
    }



    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }



}
