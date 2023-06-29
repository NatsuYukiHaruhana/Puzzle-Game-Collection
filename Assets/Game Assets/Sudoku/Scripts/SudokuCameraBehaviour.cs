using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuCameraBehaviour : MonoBehaviour
{
    private void Start() {
        Camera cam = Camera.main;
        SudokuBoard board = GetComponent<SudokuBoard>();

        cam.orthographicSize = (board.GetHeight() > board.GetWidth() ? board.GetHeight() : board.GetWidth()) / 2f + 1f;
        cam.transform.position = new Vector3(board.GetWidth() / 2f, board.GetHeight() / 2f + 0.5f, -0.3f);
    }
}
