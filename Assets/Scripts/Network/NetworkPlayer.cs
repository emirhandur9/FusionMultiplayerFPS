using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public TextMeshPro playerNickname;
    public static NetworkPlayer Local { get; set; }

    public Transform playerModel;

    [Networked(OnChanged = nameof(OnNicknameChanged))]
    public NetworkString<_16> nickname { get; set; }



    bool isPublicJoinMessageSent = false;

    NetworkInGameMessages networkInGameMessages;

    public LocalCameraHandler localCameraHandler;
    public GameObject localUI;

    public ScoreboardUIHandler localScoreboard;

    HPHandler hpHandler;


    private void Awake()
    {
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        hpHandler = GetComponent<HPHandler>();
    }
    public override void Spawned()
    {
        string nickname = PlayerPrefs.GetString("PlayerNickname");

        if (Object.HasInputAuthority)
        {
            Debug.Log("local player");
            Local = this;
            Camera.main.gameObject.SetActive(false);


            RPC_SetNickname(nickname);


            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));
        }
        else
        {
            Camera cam = GetComponentInChildren<Camera>();
            cam.enabled = false;

            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            localUI.SetActive(false);


            Debug.Log("other player");
        }

        Runner.SetPlayerObject(Object.InputAuthority, Object);

        transform.name = $"P_{Object.Id}";

    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Object.HasStateAuthority)
        {
            if(Runner.TryGetPlayerObject(player, out NetworkObject playerLeft))
            {
                if(playerLeft == Object)
                {
                    Local.GetComponent<NetworkInGameMessages>().SendInGameMessages(playerLeft.GetComponent<NetworkPlayer>().nickname.ToString(), "left");
                    Local.GetComponent<NetworkInGameMessages>().RemovePlayerScoreboard(playerLeft.GetComponent<NetworkPlayer>().nickname.ToString());
                }

            }

        }
        if(player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }
    public static void OnNicknameChanged(Changed<NetworkPlayer> changed)
    {
        changed.Behaviour.NicknameChanged();
    }
    private void NicknameChanged()
    {
        playerNickname.text = nickname.ToString();
        networkInGameMessages.AddPlayerScoreboard(nickname.ToString(), hpHandler.deathCount);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickname(string nickname, RpcInfo info = default)
    {
        this.nickname = nickname;

        if (!isPublicJoinMessageSent)
        {
            networkInGameMessages.SendInGameMessages(nickname, "joined");

            isPublicJoinMessageSent = true;
        }
    }



   

   

}
