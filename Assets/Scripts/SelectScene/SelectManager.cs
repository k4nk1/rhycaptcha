using UnityEngine;

public class SelectManager : MonoBehaviour
{
    public string testName;
    public float testBpm;
    public float testDuration;
    public int testMaxScore;
    public int testNoNotes;
    public float testSongOffset;
    [TextArea]
    public string testPattern;
    public AudioClip testAudio;

    void Start(){
        GameManager.level = new Level { name = testName, bpm = testBpm, duration = testDuration, maxScore = testMaxScore, noNotes = testNoNotes, songOffset = testSongOffset, pattern = testPattern, audio = testAudio };
    }

    public void Play()
    {
        GameManager.Play(GameManager.level);
    }

    public void Edit()
    {
        GameManager.Edit(GameManager.level);
    }
}
