using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MinesweeperOptionsBehaviour : MonoBehaviour
{
    [SerializeField]
    private Slider widthSlider,
                   heightSlider,
                   mineSlider;

    [SerializeField]
    private TextMeshProUGUI widthNumber,
                            heightNumber,
                            mineNumber;

    private void Start()
    {
        widthNumber.SetText(widthSlider.value.ToString());
        heightNumber.SetText(heightSlider.value.ToString());
        mineNumber.SetText(mineSlider.value.ToString());
    }

    public void UpdateWidth(float sliderValue)
    {
        mineSlider.maxValue = widthSlider.value * heightSlider.value - 1;
        widthNumber.SetText(sliderValue.ToString());
    }

    public void UpdateHeight(float sliderValue)
    {
        mineSlider.maxValue = widthSlider.value * heightSlider.value - 1;
        heightNumber.SetText(sliderValue.ToString());
    }

    public void UpdateMine(float sliderValue)
    {
        mineNumber.SetText(sliderValue.ToString());
    }

    public void PlayGame() {
        MinesweeperBoardVars.Width = Mathf.FloorToInt(widthSlider.value);
        MinesweeperBoardVars.Height = Mathf.FloorToInt(heightSlider.value);
        MinesweeperBoardVars.NMines = Mathf.FloorToInt(mineSlider.value);
        SceneManager.LoadScene("Minesweeper Game Scene");
    }

}
