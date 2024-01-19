using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using UnityEngine;

public class WebSocketManager : MonoBehaviour
{
    public ClientWebSocket wsc;
    public ClientWebSocketOptions options;

    public string access_token;
    public string player_name, player_rank;
    public Guid player_uuid = Guid.NewGuid();
    public string uri_address;
    public CancellationToken CT;
    // Start is called before the first frame update
    async void Start() {
        player_uuid = Guid.NewGuid();
        Uri uri = new(uri_address);
        wsc = new();
        wsc.Options.SetRequestHeader("token", access_token);
        wsc.Options.SetRequestHeader("name", player_name);
        wsc.Options.SetRequestHeader("uuid", player_uuid.ToString());
        wsc.Options.SetRequestHeader("rank", player_rank);
        await wsc.ConnectAsync(uri,CT);
        Debug.LogError(wsc.State);
    }
    IEnumerator Recive() {
        var buffer = new byte[4096 * 20];
        while (wsc.State == WebSocketState.Open) {
            yield return new WaitForEndOfFrame();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
