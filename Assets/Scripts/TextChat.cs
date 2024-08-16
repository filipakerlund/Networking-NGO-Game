using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TextChat : NetworkBehaviour
{
    [SerializeField] InputReader inputReader;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TMP_InputField inputField;

    // To hide it and reveal it with 'Enter' key
    [SerializeField] GameObject chatText;
    private bool isChatVisible;

    private FixedString128Bytes message;
    private List<string> messageList = new List<string>();
    
    private void Start()
    {
        if (inputReader != null)
        {
            inputReader.SendEvent += OnSend;
        }
        ToggleChat(false);  // Hide the chat and unlock input.
    }

    private void OnSend()
    {
        if (!isChatVisible)
        {
            ToggleChat(true);
            inputField.Select();
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(inputField.text))
            {
                // Send message.
                message = new FixedString128Bytes(inputField.text);
                SubmitMessageServerRpc(message);
                inputField.text = string.Empty;
            }
            ToggleChat(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitMessageServerRpc(FixedString128Bytes message)
    {

        // Update server message list.
        UpdateMessageList(message);

        // Broadcast the updated message to all clients.
        SubmitMessageClientRpc(message);
    }

    [ClientRpc]
    private void SubmitMessageClientRpc(FixedString128Bytes message)
    {
        // Update the local message list and UI.
        if (!IsServer) // Since it can be a host.
        {
            UpdateMessageList(message);
        }
    }

    public void UpdateMessageList(FixedString128Bytes message)
    {
        messageList.Insert(0, message.ToString());
        
        if (messageList.Count > 1)
        {
            text.text = string.Join("\n", messageList);
        }
        else
        {
            text.text = message.ToString();
        }
    }

    private void ToggleChat(bool isVisible)
    {
        inputReader.ToggleInput(!isVisible);
        chatText.SetActive(isVisible);
        isChatVisible = isVisible;  
    }
}