using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreboardUIHandler : MonoBehaviour
{
    List<PlayerScore> allPlayers = new List<PlayerScore>();

    [SerializeField] Transform layout;
    [SerializeField] GameObject scorePrefab;

    public void AddPlayerScoreboard(string username)
    {
        //daha once eklendi mi diye check
        foreach (var item in allPlayers)
        {
            if (username == item.nickname)
                return;
        }

        //eklenmediyse eklioz
        PlayerScore playerScore = new PlayerScore();
        playerScore.nickname = username;
        GameObject uiText = Instantiate(scorePrefab, layout);
        playerScore.uiText = uiText.GetComponent<TextMeshProUGUI>();


        playerScore.Update();

        allPlayers.Add(playerScore);
    }
    public void RemovePlayerScoreboard(string username)
    {
        PlayerScore playerScore;
        foreach (var item in allPlayers)
        {
            if (username == item.nickname)
            {
                playerScore = item;
                Destroy(playerScore.uiText.gameObject);
                allPlayers.Remove(playerScore);
                break;
            }
        }
    }

    public PlayerScore GetPlayerByNickname(string nick)
    {
        foreach (var plyr in allPlayers)
        {
            if (nick == plyr.nickname)
                return plyr;
        }
        return null;
    }

    public void UpdateScoreBoard(string killer, string dead)
    {
        PlayerScore killerPlayer = GetPlayerByNickname(killer);
        PlayerScore deadPlayer = GetPlayerByNickname(dead);

        killerPlayer.killCount++;
        deadPlayer.deadCount++;

        killerPlayer.Update();
        deadPlayer.Update();
    }

    public void UpdateDeath(string nick)
    {
        Debug.Log("update death: " + nick);
        PlayerScore playr = GetPlayerByNickname(nick);
        playr.deadCount++;
        playr.Update();
    }

    public void UpdateKill(string nick)
    {
        PlayerScore playr = GetPlayerByNickname(nick);
        playr.killCount++;
        playr.Update();
    }
}

public class PlayerScore
{
    public string nickname { get; set; }
    public TextMeshProUGUI uiText { get; set; }

    public int killCount { get; set; }
    public int deadCount { get; set; }

    public void Update()
    {
        uiText.text = nickname + ": " + " kill: " + killCount + " death:" + deadCount;
    }
}
