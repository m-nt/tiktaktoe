using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Edgegap;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WebSocketManager : MonoBehaviour
{
    public static WebSocketManager self;
    public ClientWebSocket wsc;
    public ClientWebSocketOptions options;

    public string access_token;
    public User user;
    public string uri_address;
    public Button button;
    public Match match;
    public UnityEvent<Match> OnSocketReceiveMatch;
    public CancellationToken CT;
    private void Awake() {
        if (self == null) {
            self = this;
            DontDestroyOnLoad(this);
        } else {
            // Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start() {
        OnSocketReceiveMatch.AddListener(OnMessageMatch);
        user.uuid = !String.IsNullOrEmpty(user.uuid) ? user.uuid : Guid.NewGuid().ToString();
        Uri uri = new(uri_address);
        wsc = new();
        wsc.Options.SetRequestHeader("token", access_token);
        wsc.Options.SetRequestHeader("name", user.name);
        wsc.Options.SetRequestHeader("uuid", user.uuid);
        wsc.Options.SetRequestHeader("rank", user.rank.ToString());
        UniTask.Void(async () => {
            await wsc.ConnectAsync(uri,CT);
            await Recive();
        });
        Debug.LogError(wsc.State);
    }
    private void OnApplicationQuit() {
        UniTask.Void(async () => {
            await wsc.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None);
        });
    }
    async Task Recive() {
        var buffer = new byte[4096 * 20];
        while (wsc.State == WebSocketState.Open) {
            await Task.Delay(10);
            var response = await wsc.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (response.MessageType == WebSocketMessageType.Close) {
                await
                    wsc.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close response received",
                        CancellationToken.None);
            } else if (response.EndOfMessage) {
                var result = Encoding.UTF8.GetString(buffer);
                Match res_match = new();
                try {
                    Debug.LogError(result);
                    res_match = JsonConvert.DeserializeObject<Match>(result);
                    OnSocketReceiveMatch.Invoke(res_match);
                } catch (System.Exception) {
                    // string res = result.ToString();
                    // Debug.LogError($"Response data is not right:");
                    // Debug.LogError(res);
                }

                buffer = new byte[4096 * 20];
            }
        }
    }
    // Update is called once per frame
    void OnMessageMatch(Match _match) {
        // match.data.state
        match = _match;
        // Debug.LogError(match.ToJson);
        SceneManager.LoadScene(1);
    }
    void OnMessageUser(WSDataUser user) {
        Debug.LogError(user);
    }
    public void OnSendConnectionMessage() {
        if (wsc.State != WebSocketState.Open) {
            Debug.LogError($"WebSocket is not connected! ${wsc.State}");
        }
        button.interactable = false;
        WSDataUser wsdata = new() {
            action = "ready",
            data = user
        };
        ArraySegment<byte> buffer = new(Encoding.ASCII.GetBytes(wsdata.ToJson));
        UniTask.Void(async () => {
            await wsc.SendAsync(buffer,WebSocketMessageType.Binary,true, CancellationToken.None);
        });
    }
}
public class BaseModel {
    public string ToJson {
        get { return JsonUtility.ToJson(this); }
    }

    public string ToParams {
        get {
            string param = "?";
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.ToJson);
            foreach (KeyValuePair<string, string> item in dict) {
                if (!string.IsNullOrEmpty(item.Value))
                    param += $"{item.Key}={item.Value}&";
            }
            return param;
        }
    }

}
[Serializable]
public class WSDataMatch: BaseModel{
    public string action;
    public Match data;
}
[Serializable]
public class WSDataUser: BaseModel{
    public string action;
    public User data;
}

[Serializable]
public class UserState {
    public static readonly string LOBY = "loby";
    public static readonly string READY = "ready";
    public static readonly string PRELOAD = "preload";
    public static readonly string INGAME = "ingame";
    public static readonly string OFFLINE = "offline";
}
[Serializable]
public class MatchState {
    public static readonly string PRELOAD = "preload";
    public static readonly string INGAME = "ingame";
    public static readonly string ENDING = "ending";
    public static readonly string ENDED = "ended";
    public static readonly string TERMINATION = "termination";
}
[Serializable]
public class User: BaseModel{
    public string uuid;
    public string name;
    public int rank;
    public UserState state;
    public string[] companions;
}
[Serializable]
public class Match: BaseModel{
    public string muid;
    public string state;
    public ushort port;
    public string[] opponents;
}

