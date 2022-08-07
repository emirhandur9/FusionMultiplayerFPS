using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkInGameMessages : NetworkBehaviour
{
    InGameMessagesUIHandler inGameMessagesUIHandler;
    ScoreboardUIHandler scoreboardUIHandler;

    public void SendInGameMessages(string userNickname, string message)
    {
        RPC_InGameMessage($"<b>{userNickname}</b> {message}");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_InGameMessage(string message, RpcInfo info = default)
    {
        if(inGameMessagesUIHandler == null)
        {
            inGameMessagesUIHandler = NetworkPlayer.Local.localCameraHandler.GetComponentInChildren<InGameMessagesUIHandler>();
        }

        if(inGameMessagesUIHandler != null)
        {
            inGameMessagesUIHandler.OnGameMessageReceived(message);
        }
    }


    public void AddPlayerScoreboard(string username, int currentDeathCount)
    {
        if (scoreboardUIHandler == null)
        {
            //Debug.Log(NetworkPlayer.Local);
            scoreboardUIHandler = NetworkPlayer.Local.localCameraHandler.GetComponentInChildren<ScoreboardUIHandler>();
        }

        if (scoreboardUIHandler != null)
        {
            scoreboardUIHandler.AddPlayerScoreboard(username);
        }
    }

    public void RemovePlayerScoreboard(string username)
    {
        RPC_RemovePlayerScoreBoard(username);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_RemovePlayerScoreBoard(string username)
    {
        if (scoreboardUIHandler == null)
        {
            scoreboardUIHandler = NetworkPlayer.Local.localCameraHandler.GetComponentInChildren<ScoreboardUIHandler>();
        }

        if (scoreboardUIHandler != null)
        {
            scoreboardUIHandler.RemovePlayerScoreboard(username);
        }
    }

    public void Killed(string killer, string dead)
    {
        RPC_UpdateScoreBoard(killer, dead);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateScoreBoard(string killer, string dead)
    {
        if (scoreboardUIHandler == null)
        {
            scoreboardUIHandler = NetworkPlayer.Local.localCameraHandler.GetComponentInChildren<ScoreboardUIHandler>();
        }

        if (scoreboardUIHandler != null)
        {
            scoreboardUIHandler.UpdateScoreBoard(killer, dead);
        }
    }
}
