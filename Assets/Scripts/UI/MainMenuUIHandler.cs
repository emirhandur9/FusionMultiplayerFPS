using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUIHandler : MonoBehaviour
{
    public TMP_InputField inputField;

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
        {
            inputField.text = PlayerPrefs.GetString("PlayerNickname");
        }
    }

    public void OnJoinGameClicked()
    {
        PlayerPrefs.SetString("PlayerNickname", inputField.text);
        PlayerPrefs.Save();

        SceneManager.LoadScene("GameScene");
    }
}
