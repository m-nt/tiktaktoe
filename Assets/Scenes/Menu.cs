using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : NetworkBehaviour
{
    public static Menu self;
    public GameObject start_new_game_menu;
    public GameObject loading_new_game_menu;
    public GameObject game_result_menu;
    [SyncVar]
    public string x_score_value, o_score_value;
    public TextMeshProUGUI x_score_text, o_score_text, you_text;
    public Transform[] blocks = new Transform[9];
    public PlayerType player;
    private void Awake() {
        if (self == null) {
            self = this;
        }
    }
    public void ExitMatch() {
        SceneManager.LoadScene(0);
        NewMatch(true);
    }
    [Command(requiresAuthority = false)]
    public void NewMatch(bool new_m) {
        Game.self.Reset_game_hard(new_m);
    }
    private void Update() {
        x_score_text.text = x_score_value;
        o_score_text.text = o_score_value;
    }
    [ClientRpc(includeOwner = true)]
    public void WinResult(int result){
        game_result_menu.SetActive(true);
        game_result_menu.transform.GetChild(result).gameObject.SetActive(true);
    }
    [ClientRpc(includeOwner = true)]
    public void Reset_Game() {
        game_result_menu.transform.GetChild(0).gameObject.SetActive(false);
        game_result_menu.transform.GetChild(1).gameObject.SetActive(false);
        game_result_menu.gameObject.SetActive(false);
        start_new_game_menu.SetActive(false);
        loading_new_game_menu.SetActive(false);
        Reset_game_soft();
    }
    [ClientRpc( includeOwner = true)]
    public void Start_new_game() {
        loading_new_game_menu.SetActive(true);
        // StartCoroutine(Start_game_delay());
    }
    [ClientRpc(includeOwner = true)]
    public void Change_state(int index, int p_type) {
        Debug.LogError("hhhey");
        blocks[index].GetChild(p_type).gameObject.SetActive(true);
    }
    [TargetRpc]
    public void Set_up_player(NetworkConnectionToClient target, PlayerType type) {
        player = type;
        switch (type) {
            case PlayerType.X:
                you_text.text = "you: X";
                you_text.color = Color.blue;
                break;
            case PlayerType.O:
                you_text.text = "you: O";
                you_text.color = Color.red;
                break;
        }
    }
    [ClientRpc( includeOwner = true)]
    public void Start_game_delay() {
        // yield return new WaitForSeconds(2);
        start_new_game_menu.SetActive(false);
    }
    public void Add_State_local(int index){
        Add_state(index, (int)player);
    }
    [Command(requiresAuthority = false)]
    public void Add_state(int index,int type, NetworkConnectionToClient sender = null) {
        if (Game.self.game_matrix[index] != -1) return;
        if ((int)Game.self.turn != type) return;
        Change_state(index, type);
        Game.self.game_matrix[index] = type;
        Game.self.turn = Game.self.turn == PlayerType.X ? PlayerType.O : PlayerType.X;
    }

    [ClientRpc]
    public void Reset_game_soft() {
        foreach (Transform item in blocks) {
            item.GetChild(0).gameObject.SetActive(false);
            item.GetChild(1).gameObject.SetActive(false);
        }
    }
}
