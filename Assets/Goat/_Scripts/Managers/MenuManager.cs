using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    // Start is called before the first frame update
    void Start()
    {
        MainMenu = GameObject.Find("MainMenu");
        OptionsMenu = GameObject.Find("OptionsMenu");
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }

    // Update is called once per frame
    public void OptionsMenuOpen()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }
    public void BackButton()
    {
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }
    public void QuitButton()
    {
         Application.Quit();
    }
}
