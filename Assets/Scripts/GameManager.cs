using UnityEngine;
using UnityEngine.SceneManagement;

public class Level
{
    public string name;
    public string url;
    public float bpm;
    public float duration;
    public int maxScore;
    public int noNotes;
    public float songOffset;
    public AudioClip audio;
    public string pattern;
}

public class Result
{
    public int score;
    public int maxCombo;
}

public static class GameManager
{
    public static Level level = null;
    public static float startOffset = 0;
    public static float deviceOffset = 0.1f;
    public static string lastScene;
    public static Result result;

    public static void Play(Level _level, float _offset = 0)
    {
        level = _level;
        startOffset = _offset;
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("PlayScene");
    }

    public static void Replay()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public static void Back()
    {
        SceneManager.LoadScene(lastScene);
    }

    public static void Edit(Level _level)
    {
        level = _level;
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("EditorScene");
    }

    public static void ShowResult(Result _result){
        result = _result;
        SceneManager.LoadScene("ResultScene");
    }
}
