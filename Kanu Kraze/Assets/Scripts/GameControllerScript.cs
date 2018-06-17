using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerScript : MonoBehaviour
{
    public CameraScript CameraScript;

    public List<PlayerScript> Players = new List<PlayerScript>();
    public int SelectedPlayer = 0;
    public GameObject LevelCompletePopup;
    public GameObject LevelFailedPopup;

    // Use this for initialization
    void Start()
    {
        if (Players.Count > 0)
        {
            Players[0].Activate();
            CameraScript.ChangePlayer(Players[0]);
            // set all players to follow startup
            for(var i = 1; i < Players.Count; i++)
            {
                Players[i].Follow(Players[0]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!CameraScript.IsTransitioning())
        {
            if (Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.LeftControl))
            {
                var player = NextPlayer();

                if (!Players.Any(p => p.Alive))
                {
                    // UH OH! Game over
                    Debug.Log("GAME OVER");
                }

                while (!player.Alive)
                {
                    player = NextPlayer();
                }
                player.Activate();
                CameraScript.ChangePlayer(Players[SelectedPlayer]);
            }
        }
    }

    public PlayerScript NextPlayer()
    {
         var original = Players[SelectedPlayer];

        if (Input.GetKey(KeyCode.LeftShift))
            SelectedPlayer--;
        else
            SelectedPlayer++;

        if (SelectedPlayer >= Players.Count)
            SelectedPlayer = 0;
        if (SelectedPlayer < 0)
            SelectedPlayer = Players.Count - 1;

        original.Follow(Players[SelectedPlayer]);

        return Players[SelectedPlayer];
    }

    internal void LevelCompleted()
    {
        LevelCompletePopup.SetActive(true);
        var faces = LevelCompletePopup.GetComponentsInChildren<Image>();

        var offset = 3;

        for (var i = 0; i < Players.Count; i++)
        {
            faces[offset + i].sprite = Players[i].Complete ? Players[i].StatusImages[3] : Players[i].StatusImages[2];
        }
    }
}
