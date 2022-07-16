using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonsController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject orderMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private AudioSource clickSound;
    
    public void ChangeScene(string scene)
    {
        clickSound.Play();
        SceneManager.LoadScene(scene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    public void GoToSettings()
    {
        clickSound.Play();
        mainMenu.SetActive(false);
        orderMenu.SetActive(false);
        settingsMenu.SetActive(true);
        creditsMenu.SetActive(false);
    }

    public void ToGameOrder()
    {
        clickSound.Play();
        mainMenu.SetActive(false);
        orderMenu.SetActive(true);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }
    
    public void GoToMain()
    {
        clickSound.Play();
        mainMenu.SetActive(true);
        orderMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void GoToCredits()
    {
        clickSound.Play();
        mainMenu.SetActive(false);
        orderMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }
}
