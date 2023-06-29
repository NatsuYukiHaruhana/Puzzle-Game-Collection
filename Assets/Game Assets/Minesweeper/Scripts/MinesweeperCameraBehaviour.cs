using UnityEngine;

public class MinesweeperCameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private float xMovementSpeed, yMovementSpeed, zoomSpeed;

    private float maxZoom, minX, minY, maxX, maxY;

    private Camera cam;
    private void Start() {
        cam = Camera.main;
        MinesweeperBoard board = GetComponent<MinesweeperBoard>();

        maxZoom = cam.orthographicSize = (board.GetHeight() > board.GetWidth() ? board.GetHeight() : board.GetWidth()) / 2f + 1f;
        cam.transform.position = new Vector3(board.GetWidth() / 2f, board.GetHeight() / 2f + 0.5f, -0.3f);

        minX = 5;
        maxX = board.GetWidth() - 5;

        minY = 3;
        maxY = board.GetHeight() - 3;
    }

    private void Update() {
        float xAxis = Input.GetAxis("Horizontal"), yAxis = Input.GetAxis("Vertical");
        if (yAxis > 0f) { // go up
            if (cam.transform.position.y < maxY) { 
                cam.transform.position += new Vector3(0f, yMovementSpeed * yAxis * Time.deltaTime, 0f);
            }
        } else if (yAxis < 0f) { // go down
            if (cam.transform.position.y > minY) {
                cam.transform.position += new Vector3(0f, yMovementSpeed * yAxis * Time.deltaTime, 0f);
            }
        }
        
        if (xAxis > 0f) { // go right
            if (cam.transform.position.x < maxX) {
                cam.transform.position += new Vector3(xMovementSpeed * xAxis * Time.deltaTime, 0f, 0f);
            }
        } else if (xAxis < 0f) { // go left
            if (cam.transform.position.x > minX) {
                cam.transform.position += new Vector3(xMovementSpeed * xAxis * Time.deltaTime, 0f, 0f);
            }
        }

        float zoomAxis = Input.GetAxis("Mouse ScrollWheel");
        if (zoomAxis > 0f) { // zoom 
            if (cam.orthographicSize > 3f) { 
                cam.orthographicSize -= zoomSpeed * zoomAxis * Time.deltaTime;
            }
        } else if (zoomAxis < 0f) { // unzoom
            if (cam.orthographicSize < maxZoom) { 
                cam.orthographicSize -= zoomSpeed * zoomAxis * Time.deltaTime;
            }
        }
    }
}
