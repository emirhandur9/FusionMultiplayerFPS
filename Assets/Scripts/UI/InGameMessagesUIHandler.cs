using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameMessagesUIHandler : MonoBehaviour
{
    public TextMeshProUGUI[] messages;
    Queue messageQue = new Queue();
    private void Start()
    {
        
    }

    public void OnGameMessageReceived(string message)
    {
        messageQue.Enqueue(message);

        if (messageQue.Count > 3)
            messageQue.Dequeue();

        int queIndex = 0;
        foreach (var msg in messageQue)
        {
            messages[queIndex].text = msg.ToString();
            queIndex++;
        }
    }
}
