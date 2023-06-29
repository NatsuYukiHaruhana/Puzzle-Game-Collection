using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighscoreTable : MonoBehaviour
{
    [SerializeField]
    private string gameName;

    [SerializeField]
    private Transform entryContainer, entryTemplate;

    private List<HighscoreEntry> highscoreEntryList;
    private List<Transform> highscoreEntryTransformList;

    private void Awake() {
        entryTemplate.gameObject.SetActive(false);

        if (PlayerPrefs.HasKey(gameName + " highscore table")) {
            string jsonString = PlayerPrefs.GetString(gameName + " highscore table");
            Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

            highscoreEntryList = highscores.highscoreEntryList;
        } else {
            highscoreEntryList = new List<HighscoreEntry>();
        }
        this.gameObject.SetActive(false);
    }

    public void AddHighscoreEntry(int score, string name, string gameName) {
        highscoreEntryList.Add(new HighscoreEntry{ score = score, name = name });

        for (int i = 0; i < highscoreEntryList.Count - 1; i++) {
            for (int j = i + 1; j < highscoreEntryList.Count; j++) {
                if (highscoreEntryList[i].score < highscoreEntryList[j].score) {
                    HighscoreEntry aux = highscoreEntryList[i];
                    highscoreEntryList[i] = highscoreEntryList[j];
                    highscoreEntryList[j] = aux;
                }
            }
        }

        if (highscoreEntryList.Count > 10) {
            highscoreEntryList.RemoveAt(highscoreEntryList.Count - 1);
        }

        Highscores highscores = new Highscores { highscoreEntryList = highscoreEntryList };
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString(gameName + " highscore table", json);
        PlayerPrefs.Save();

        this.gameObject.SetActive(true);

        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry entry in highscoreEntryList) {
            CreateHighscoreEntryTransform(entry, entryContainer, highscoreEntryTransformList);
        }

        this.gameObject.SetActive(false);
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList) {
        const float templateHeight = 38.5f, initialHeight = 179f;

        Transform entryTransform = Instantiate(entryTemplate, container);
        entryTransform.gameObject.SetActive(true);

        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0f, initialHeight - templateHeight * transformList.Count);

        entryTransform.Find("Score Text").GetComponent<TextMeshProUGUI>().SetText(highscoreEntry.score.ToString());
        entryTransform.Find("Name Text").GetComponent<TextMeshProUGUI>().SetText(highscoreEntry.name);

        transformList.Add(entryTransform);
    }

    private class Highscores {
        public List<HighscoreEntry> highscoreEntryList;
    }

    [System.Serializable]
    private class HighscoreEntry {
        public int score;
        public string name;
    }
}
