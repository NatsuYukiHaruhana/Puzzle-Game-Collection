using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class _2048OptionsBehaviour : MonoBehaviour
{
    [SerializeField]
    private Slider limitSlider;

    [SerializeField]
    private Toggle endlessToggle;

    [SerializeField]
    private TextMeshProUGUI limitNumber;

    private void Start() {
        limitNumber.SetText(limitSlider.value.ToString());
    }

    public void UpdateLimit(float sliderValue) {
        if (Mathf.IsPowerOfTwo(Mathf.FloorToInt(limitSlider.value))) {
            limitNumber.SetText(sliderValue.ToString());
        } else {
            limitSlider.value = Mathf.NextPowerOfTwo(Mathf.FloorToInt(limitSlider.value));
        }
    }

    public void ChangeEndless() {
        limitSlider.interactable = !endlessToggle.isOn;
    }

    public void PlayGame() {
        _2048BoardVars.Limit = Mathf.FloorToInt(limitSlider.value);
        _2048BoardVars.Endless = endlessToggle.isOn;
        SceneManager.LoadScene("2048 Game Scene");
    }
}
