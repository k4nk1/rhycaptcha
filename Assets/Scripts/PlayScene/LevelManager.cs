using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    // Inspector
    [SerializeField]
    public float perfectSpan;
    [SerializeField]
    public float goodSpan;
    [SerializeField]
    public float laneLength;
    [SerializeField]
    public float laneWidth;
    [SerializeField]
    public int noLane;
    [SerializeField]
    public float noteSpeed;
    [SerializeField]
    public float destroyDelay;
    [SerializeField]
    public float touchingScoreSpan;

    //Prefab
    [SerializeField]
    private GameObject notePrefab;
    [SerializeField]
    private GameObject longNotePrefab;

    //Connect
    [SerializeField]
    private Transform noteParent;
    [SerializeField]
    private Transform lineParent;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private TMP_Text debugText;


    public Queue<Note> spawnQueue;
    public List<Note> notes;
    public List<Renderer> lineRenderers;
    public float spawnAdvance;
    public float spb;
    public int combo = 0;
    public int maxCombo;
    public int score = 0;
    public float count = 0;
    public bool pausing;
    public double startTime;
    public double pausedTime;
    private float songOffset;
    public float bpm;
    public float duration;

    void Start()
    {
        //name = GameManager.level.name
        if (GameManager.level == null){
            Debug.LogError("Level is not set.");
            return;
        }
        bpm = GameManager.level.bpm;
        duration = GameManager.level.duration;
        songOffset = GameManager.level.songOffset;
        audioSource.clip = GameManager.level.audio;
        spawnQueue = LevelUtility.DecodePattern(GameManager.level.pattern, bpm, GameManager.startOffset);
        notes = new List<Note>();
        spawnAdvance = laneLength / noteSpeed;
        spb = 60 / bpm;

        if(spawnAdvance > GameManager.deviceOffset + songOffset){
            startTime = AudioSettings.dspTime + spawnAdvance;
            audioSource.PlayScheduled(startTime - GameManager.deviceOffset - songOffset);
        }else{
            startTime = AudioSettings.dspTime + GameManager.deviceOffset + songOffset;
            audioSource.PlayScheduled(AudioSettings.dspTime);
        }

        foreach(Transform child in lineParent){
            lineRenderers.Add(child.GetComponent<Renderer>());
        }
    }

    void Update()
    {
        if (pausing) return;
        if (1 < count)
        {
            if (count > Mathf.Ceil(count - Time.deltaTime))
                uiManager.SetCD(Mathf.FloorToInt(count)-1);
            count -= Time.deltaTime;
            return;
        }
        float playTime = (float)(AudioSettings.dspTime - startTime);
        debugText.SetText(playTime.ToString());

        while (spawnQueue.Count > 0 && spawnQueue.Peek().time - spawnAdvance <= playTime)
            notes.Add(SpawnNote(spawnQueue.Dequeue()));

        for (int i = notes.Count - 1; i >= 0; i--)
        {
            Note note = notes[i];

            note.Move(noteSpeed);

            if (note.state == NoteState.UnTouched && note.endTime + goodSpan < playTime)
            {
                uiManager.SetCombo(combo = 0);
                uiManager.ShowMiss();
                note.Miss();
            }

            if (note.endTime + destroyDelay < playTime)
            {
                note.Destroy();
                notes.Remove(note);
                continue;
            }

            if (note.state == NoteState.Touching)
            {
                if ((playTime-(note.endTime-note.time) / touchingScoreSpan < Mathf.Floor((playTime-(note.endTime-note.time)+Time.deltaTime) / touchingScoreSpan))){
                    uiManager.SetScore(score += 10);
                }
                if (!InputManager.Touching(note.lane + (note.endLane - note.lane) * (playTime - note.time) / (note.endTime - note.time) + 0.5f, noLane))
                {
                    uiManager.SetCombo(combo = 0);
                    uiManager.ShowMiss();
                    note.Miss();
                }
                if (note.endTime < playTime)
                {
                    note.state = NoteState.Touched;
                }
            }
        }


        foreach (Touch touch in Input.touches){
            float touchedLane = LevelUtility.Screen2Lane(touch.position, noLane);
            if (touch.phase == TouchPhase.Began)
            {
                Note nextNote = GetNextNote(touchedLane);
                if (nextNote != null)
                {
                    if (nextNote.time - perfectSpan <= playTime && playTime <= nextNote.time + perfectSpan)
                    {
                        uiManager.SetCombo(++combo);
                        maxCombo = Math.Max(maxCombo, combo);
                        uiManager.SetScore(score += 100);
                        uiManager.ShowPerfect();
                        nextNote.Perfect();
                    }
                    else if (nextNote.time - goodSpan <= playTime && playTime <= nextNote.time + goodSpan)
                    {
                        uiManager.SetCombo(++combo);
                        maxCombo = Math.Max(maxCombo, combo);
                        uiManager.SetScore(score += 50);
                        uiManager.ShowGood();
                        nextNote.Good();
                    }
                }
            }
        }

        for (int i = 0; i < noLane; i++)
        {
            bool touching = InputManager.Touching(i + 0.5f, noLane);
            if (lineRenderers[i].enabled != touching)
            {
                lineRenderers[i].enabled = touching;
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) GameManager.Replay();

        //Tick
        playTime += Time.deltaTime;
        if (playTime > duration + 2f) GameManager.ShowResult(new Result { score = score, maxCombo = maxCombo });
    }

    public Note GetNextNote(float lane)
    {
        foreach (Note note in notes)
        {
            if (note.lane <= lane && lane <= note.lane + 1 && note.state == NoteState.UnTouched) return note;
        }
        return null;
    }

    public Note SpawnNote(Note note)
    {
        if (note.time == note.endTime)
        {
            note.obj = Instantiate(notePrefab, LevelUtility.SpawnPoint(note.lane, laneWidth, noLane, laneLength), Quaternion.identity, noteParent);
        }
        else
        {
            note.obj = Instantiate(longNotePrefab, LevelUtility.SpawnPoint(note.lane, laneWidth, noLane, laneLength), Quaternion.identity, noteParent);
            Transform body = note.obj.transform.Find("Body");
            Transform tail = note.obj.transform.Find("Tail");
            Vector3 relative = new Vector3((note.endLane - note.lane) * laneWidth, 0, (note.endTime - note.time) * noteSpeed);
            body.localScale = new Vector3(laneWidth * 0.8f, relative.magnitude, 1);
            body.eulerAngles = new Vector3(90, 0, Mathf.Atan2(relative.z, relative.x) * Mathf.Rad2Deg - 90);
            body.localPosition = relative / 2;
            tail.localPosition = relative;
        }
        return note;
    }

    public void Pause()
    {
        uiManager.SetPauseDialog(true);
        pausing = true;
    }

    public void Resume()
    {
        uiManager.SetPauseDialog(false);
        pausing = false;
        count = 4.1f;
    }

    public void Replay()
    {
        GameManager.Replay();
    }

    public void Back(){
        GameManager.Back();
    }
}
