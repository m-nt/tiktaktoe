using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum PlayerType {
    X,
    O,
    S
}

public class Game : NetworkBehaviour
{
    public static Game self;
    public int[] game_matrix = new int[9];
    public PlayerType turn = PlayerType.X;
    public int x_score, o_score;
    private void Awake() {
        if (self == null) {
            self = this;
        }
        if (!isServer) return;
    }
    // Start is called before the first frame update
    void OnEnable() {
        Reset_game_hard(true);
        if (isServer) Debug.LogError("Im Server");
    }
    public void Add_Player (NetworkConnectionToClient target, Player _player) {
        if (NetworkServer.connections.Count > 2) {
            target.Disconnect();
            return;
        }
        Menu.self.Start_new_game();
        if (NetworkServer.connections.Count == 1) {
            _player.type = PlayerType.X;
            Menu.self.Set_up_player(target, PlayerType.X);
        } else if (NetworkServer.connections.Count == 2){
            _player.type = PlayerType.O;
            Menu.self.Set_up_player(target, PlayerType.O);
        }
        if (NetworkServer.connections.Count >= 2) Menu.self.Start_game_delay();
    }

    [ServerCallback]
    public void Reset_game_soft() {
        int i = 0;
        while (i < game_matrix.Length) {
            game_matrix[i] = -1;
            i++;
        }
        Menu.self.Reset_game_soft();
    }
    [ServerCallback]
    public void Reset_game_hard(bool new_match) {
        if (new_match) {
            x_score = 0;
            o_score = 0;
        }
        turn = PlayerType.X;
        Reset_game_soft();
        Menu.self.Reset_Game();
    }

    // Update is called once per frame
    [ServerCallback]
    void Update() {
        Menu.self.x_score_value = x_score.ToString();
        Menu.self.o_score_value = o_score.ToString();
        if (!isServer) return;
        int result = Check_game_state();
        if (result == -1) return;
        if (result == 1 || result == 0) WinResult(result);
    }
    [ServerCallback]
    void WinResult(int result) {
        if (result == 1) o_score++;
        if (result == 0) x_score++;
        Reset_game_soft();
        Menu.self.WinResult(result);
    }
    [ServerCallback]
    public int Check_game_state() {
        if (game_matrix[0] == 1 && game_matrix[1] == 1 && game_matrix[2] == 1) return 1;
        if (game_matrix[0] == 1 && game_matrix[4] == 1 && game_matrix[8] == 1) return 1;
        if (game_matrix[0] == 1 && game_matrix[3] == 1 && game_matrix[6] == 1) return 1;
        if (game_matrix[1] == 1 && game_matrix[4] == 1 && game_matrix[7] == 1) return 1;
        if (game_matrix[2] == 1 && game_matrix[5] == 1 && game_matrix[8] == 1) return 1;
        if (game_matrix[2] == 1 && game_matrix[4] == 1 && game_matrix[6] == 1) return 1;
        if (game_matrix[3] == 1 && game_matrix[4] == 1 && game_matrix[5] == 1) return 1;
        if (game_matrix[6] == 1 && game_matrix[7] == 1 && game_matrix[8] == 1) return 1;
        if (game_matrix[0] == 0 && game_matrix[1] == 0 && game_matrix[2] == 0) return 0;
        if (game_matrix[0] == 0 && game_matrix[4] == 0 && game_matrix[8] == 0) return 0;
        if (game_matrix[0] == 0 && game_matrix[3] == 0 && game_matrix[6] == 0) return 0;
        if (game_matrix[1] == 0 && game_matrix[4] == 0 && game_matrix[7] == 0) return 0;
        if (game_matrix[2] == 0 && game_matrix[5] == 0 && game_matrix[8] == 0) return 0;
        if (game_matrix[2] == 0 && game_matrix[4] == 0 && game_matrix[6] == 0) return 0;
        if (game_matrix[3] == 0 && game_matrix[4] == 0 && game_matrix[5] == 0) return 0;
        if (game_matrix[6] == 0 && game_matrix[7] == 0 && game_matrix[8] == 0) return 0;
        return -1;
    }
}
