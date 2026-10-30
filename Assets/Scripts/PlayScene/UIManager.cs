using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //Judgement
    [SerializeField]
    private TMP_Text judgementText;
    private float sinceJudgementUpdate;

    [SerializeField]
    private TMP_Text comboText;

    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private TMP_Text cdText;

    [SerializeField]
    private GameObject pauseDialog;


    void Start()
    {

    }

    void Update()
    {
        //Judgement
        if(sinceJudgementUpdate < 1f){
            judgementText.rectTransform.anchoredPosition = new Vector3(0, 50 * Mathf.Sin(sinceJudgementUpdate * 1.57f), 0);
            sinceJudgementUpdate += Time.deltaTime;
            if (sinceJudgementUpdate > 1f)
                judgementText.SetText("");
        }
    }

    public void ShowPerfect()
    {
        sinceJudgementUpdate = 0;
        judgementText.SetText("Perfect");
    }

    public void ShowGood()
    {
        sinceJudgementUpdate = 0;
        judgementText.SetText("Good");
    }

    public void ShowMiss()
    {
        sinceJudgementUpdate = 0;
        judgementText.SetText("Miss");
    }

    public void SetCombo(int combo)
    {
        if (combo > 4)
            comboText.SetText($"{combo} Combo");
        else
            comboText.SetText("");
    }

    public void SetScore(int score)
    {
        scoreText.SetText(score.ToString());
    }

    public void SetCD(int count)
    {
        cdText.SetText(count == 0 ? "" : count.ToString());
    }

    public void SetPauseDialog(bool value){
        pauseDialog.SetActive(value);
    }
}
