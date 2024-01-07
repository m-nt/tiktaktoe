using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum PlayerType {
    X,
    O,
    S
}

public class Game : MonoBehaviour
{
    public int[] game_matrix = new int[9];
    public Transform[] blocks = new Transform[9];
    public PlayerType type = PlayerType.X;
    public GameObject start_new_game_menu;
    public GameObject loading_new_game_menu;
    public GameObject game_result_menu;
    public TextMeshProUGUI x_score_text, o_score_text;
    public int x_score, o_score;

    // Start is called before the first frame update
    void Start() {
        Application.targetFrameRate = 20;
        Reset_game_hard(true);
    }
    public void Reset_game_soft() {
        int i = 0;
        while (i < game_matrix.Length) {
            game_matrix[i] = -1;
            i++;
        }
        foreach (Transform item in blocks) {
            item.GetChild(0).gameObject.SetActive(false);
            item.GetChild(1).gameObject.SetActive(false);
        }
    }
    public void Reset_game_hard(bool new_match) {
        if (new_match) {
            x_score = 0;
            o_score = 0;
        }
        Reset_game_soft();
        game_result_menu.transform.GetChild(0).gameObject.SetActive(false);
        game_result_menu.transform.GetChild(1).gameObject.SetActive(false);
        game_result_menu.gameObject.SetActive(false);
        start_new_game_menu.SetActive(true);
        loading_new_game_menu.SetActive(false);
    }
    public void Start_new_game() {
        loading_new_game_menu.SetActive(true);
        StartCoroutine(Start_game_delay());
    }
    IEnumerator Start_game_delay() {
        yield return new WaitForSeconds(2);
        start_new_game_menu.SetActive(false);
    }
    public void Add_state(int index) {
        blocks[index].GetChild((int)type).gameObject.SetActive(true);
        game_matrix[index] = (int)type;
    }

    // Update is called once per frame
    void Update() {
        x_score_text.text = x_score.ToString();
        o_score_text.text = o_score.ToString();
        int result = Check_game_state();
        if (result == -1) return;
        if (result == 1 || result == 0) WinResult(result);
    }
    void WinResult(int result) {
        if (result == 1) o_score++;
        if (result == 0) x_score++;
        Reset_game_soft();
        game_result_menu.SetActive(true);
        game_result_menu.transform.GetChild(result).gameObject.SetActive(true);
    }
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
