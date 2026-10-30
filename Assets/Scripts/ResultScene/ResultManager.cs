using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text score;
    [SerializeField]
    private TMP_Text maxScore;
    [SerializeField]
    private TMP_Text maxCombo;
    [SerializeField]
    private TMP_Text noNotes;
    void Start(){
        score.SetText($"Score {GameManager.result.score}");
        maxScore.SetText($"/ {GameManager.level.maxScore}");
        maxCombo.SetText($"Max Combo {GameManager.result.maxCombo}");
        noNotes.SetText($"/ {GameManager.level.noNotes}");
    }

    public void Back() => GameManager.Back();
    public void Replay() => GameManager.Replay();
}
