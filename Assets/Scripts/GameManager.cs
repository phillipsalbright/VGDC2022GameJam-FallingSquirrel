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
    [SerializeField] private Transform levelLocation;
    [SerializeField] private GameObject level;
    [SerializeField] private TMP_Text joiningText;
    private float distanceBetweenLevels = 40;
    private PlayerInputManager pm;
    private bool gameOver = false;
    [SerializeField] private GameObject[] obstaclesToSpawn;
    [SerializeField] private GameObject powerUp;


    private void Awake()
    {
        joiningText.text = "Press a button on player 1's device\nor\nPress escape to play two player using one keyboard";
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
        if (pm != null)
        {
            player.DeactivateInput();
            players.Add(player.gameObject);
            player.gameObject.GetComponentInChildren<PlayerMovement>().SetPlayerNum(players.Count);
            joiningText.text = "Press a button on player " + (players.Count + 1) + "'s device\nor\nPress escape to play two player using one keyboard";
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
                if (numPlayers > 1)
                {
                    for (int i = 0; i < numPlayers; i++)
                    {
                        StartCoroutine(PowerupSpawner(i));
                    }

                }

            }

        }

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 0)
        {
            int currentPlayers = players.Count;
            for (int i = 0; i < players.Count; i++)
            {
                Destroy(players[i]);
            }
            players.Clear();
            players = new List<GameObject>();
            pm.DisableJoining();
            GameObject p = pm.playerPrefab;
            pm = null;
            GameObject p1 = Instantiate(p);
            //p.GetComponent<PlayerInput>().defaultActionMap = "Player2Keyboard";
            GameObject p2 = Instantiate(p);
            p2.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player2Keyboard");
            Debug.Log(players.Count);
            players.Add(p1);
            Debug.Log(players.Count);
            p1.gameObject.GetComponentInChildren<PlayerMovement>().SetPlayerNum(players.Count);
            players.Add(p2);
            p2.gameObject.GetComponentInChildren<PlayerMovement>().SetPlayerNum(players.Count);
            gameCam.gameObject.SetActive(false);
            float distance = 0;
            for (int i = 0; i < players.Count; i++)
            {
                Instantiate(level, levelLocation.position + new Vector3(distance, 0, 0), Quaternion.identity);
                players[i].transform.position = new Vector3(distance, 0, 0);
                players[i].GetComponent<PlayerInput>().ActivateInput();
                distance += distanceBetweenLevels;
            }
          //  players[1].GetComponent<PlayerInput>().SwitchCurrentActionMap("Player2Keyboard");
            Time.timeScale = 1;
            if (numPlayers > 1)
            {
                for (int i = 0; i < numPlayers; i++)
                {
                    StartCoroutine(PowerupSpawner(i));
                }

            }
        }
    }

    public void CupCollected(int playerNum)
    {
        if (!gameOver)
        {
            gameOver = true;
            GameObject[] cups = GameObject.FindGameObjectsWithTag("cup");
            for (int i = 0; i < cups.Length; i++)
            {
                Destroy(cups[i]);
            }
            StartCoroutine(GameOver(playerNum));
        }
    }

    IEnumerator GameOver(int playerNum)
    {
        yield return new WaitForSeconds(5f);
        gameCam.gameObject.SetActive(true);
        joiningText.SetText("Player " + playerNum + " wins!");
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0);
    }

    IEnumerator PowerupSpawner(int playerIndex)
    {
        yield return new WaitForSeconds(4f);
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5, 9));
            float AverageOtherPlayers = 0;
            for (int i = 0; i < numPlayers; i++)
            {
                if (i != playerIndex)
                {
                    AverageOtherPlayers += players[i].transform.position.y;
                }
            }
            AverageOtherPlayers = AverageOtherPlayers / (numPlayers - 1);
            float probability = 1;
            if (players[playerIndex].transform.position.y < AverageOtherPlayers)
            {
                probability = 1.6f;
            }
            else
            {
                probability = .4f;
            }
            float randomNum = Random.Range(0, 2);
            if (randomNum < probability)
            {
                float xrange = Random.Range(players[playerIndex].transform.position.x - 3, players[playerIndex].transform.position.x + 3);
                Instantiate(powerUp, new Vector3(xrange, players[playerIndex].GetComponentInChildren<PlayerMovement>().transform.position.y - 20), Quaternion.identity);
            }
        }
    }

    public void SpawnObstacle(int playerIndex)
    {
        int otherIndex = Random.Range(0, numPlayers);
        if (otherIndex == playerIndex && otherIndex > 0)
        {
            otherIndex--;
        }
        else if (otherIndex == playerIndex && otherIndex < numPlayers - 1)
        {
            otherIndex++;
        }
      //  Debug.Log("spawning obstacle" + playerIndex + " " + otherIndex);
        int obstacleToSpawn = Random.Range(0, obstaclesToSpawn.Length);
        float xloc = players[otherIndex].transform.position.x + Random.Range(-3, 3);
        Instantiate(obstaclesToSpawn[obstacleToSpawn], new Vector3(xloc, players[otherIndex].GetComponentInChildren<PlayerMovement>().transform.position.y - 20), Quaternion.identity);
    }
}
