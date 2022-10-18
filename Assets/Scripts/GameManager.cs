using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int numPlayers = 2;
    private List<GameObject> players = new List<GameObject>();
    [SerializeField] private Camera gameCam;
    [SerializeField] private Transform initialSpawnPoint;
    [SerializeField] private Transform levelLocation;
    [SerializeField] private GameObject level;
    [SerializeField] private TMP_Text joiningText;
    private float distanceBetweenLevels = 40;
    private PlayerInputManager pm;
    private bool gameOver = false;

    private void Awake()
    {
        joiningText.text = "Press a button on player 1's device";
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(.5f);
        Time.timeScale = 0;
        pm = this.GetComponent<PlayerInputManager>();
        pm.EnableJoining();
    }
    public void OnPlayerJoined(PlayerInput player)
    {
        player.DeactivateInput();
        players.Add(player.gameObject);
        player.gameObject.GetComponentInChildren<PlayerMovement>().SetPlayerNum(players.Count);
        joiningText.text = "Press a button on player " + (players.Count + 1) + "'s device";
        if (players.Count >= numPlayers)
        {
            pm.DisableJoining();
            //Game start
            gameCam.gameObject.SetActive(false);
            float distance = 0;
            for (int i = 0; i < players.Count; i++)
            {
                Instantiate(level, levelLocation.position + new Vector3(distance, 0, 0), Quaternion.identity);
                players[i].transform.position = new Vector3(distance, 0, 0);
                players[i].GetComponent<PlayerInput>().ActivateInput();
                distance += distanceBetweenLevels;
            }
            Time.timeScale = 1;
        }

    }

    public void CupCollected(int playerNum)
    {
        if (!gameOver)
        {
            gameOver = true;
            gameCam.gameObject.SetActive(true);
            joiningText.SetText("Player " + playerNum + " wins!");
            StartCoroutine(GameOver());
        }
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }
}
