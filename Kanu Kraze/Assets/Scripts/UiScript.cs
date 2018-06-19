using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiScript : MonoBehaviour
{
    public GameObject CitizenFreedPopup;
    public static CitizenScript ActiveCitizen;
    public GameObject LoadScreen;
    public Sprite[] Planets;
    public Image LoadingPlanet;

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(1);
    }

    internal void ShowCitizen(CitizenScript citizenScript)
    {
        CitizenFreedPopup.SetActive(true);
        var renderers = CitizenFreedPopup.GetComponentsInChildren<Image>();
        renderers[3].sprite = citizenScript.Sprites.FirstOrDefault();
        renderers[4].sprite = citizenScript.Reward;

        var txts = CitizenFreedPopup.GetComponentsInChildren<Text>();
        txts[2].text = citizenScript.Name;
        var gender = citizenScript.Male ? "he" : "she";
        txts[3].text = "As a reward for saving " + citizenScript.Name + ", " + gender + " has given you " + citizenScript.GiftName + "!";
        ActiveCitizen = citizenScript;
    }

    public void ResumeAfterPlayer()
    {
        if (ActiveCitizen)
        {
            CitizenFreedPopup.SetActive(false);
            ActiveCitizen.ShipPickup();
        }
        ActiveCitizen = null;
    }

    public void LoadLevel(int i)
    {
        const int offset = 2;

        LoadingPlanet.sprite = Planets[i];
        LoadScreen.SetActive(true);
        SceneManager.LoadScene(offset + i);
    }
}
