using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform stage;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private RectTransform lineParent;
    [SerializeField]
    private RectTransform GraduationParent;
    [SerializeField]
    private RectTransform noteParent;
    [SerializeField]
    private RectTransform divisionParent;

    [SerializeField]
    private Image noteIcon;
    [SerializeField]
    private Image flickNoteIcon;
    [SerializeField]
    private Image longNoteIcon;
    [SerializeField]
    private TMP_InputField nameInput;
    [SerializeField]
    private TMP_InputField urlInput;
    [SerializeField]
    private TMP_InputField bpmInput;
    [SerializeField]
    private TMP_InputField delayInput;

    [SerializeField]
    private GameObject notePrefab;
    [SerializeField]
    private GameObject flickNotePrefab;
    [SerializeField]
    private GameObject longNotePrefab;
    [SerializeField]
    private GameObject graduationPrefab;

    [SerializeField]
    private int graduationSpan;
    [SerializeField]
    private float speedMultiplier;

    private float stageSpeed;
    private int noGraduations;
    private List<TMP_Text> graduations;
    private TMP_Text[] divisionButtons = new TMP_Text[8];
    private int lastBeat = 0;
    private float touchDistance;
    private SortedSet<EditorNote> notes;
    private string noteType = "Note";
    private EditorLongNoteTail holdingTail;

    public int division = 1;


    void Start()
    {
        nameInput.text = GameManager.level.name;
        urlInput.text = GameManager.level.url;
        bpmInput.text = GameManager.level.bpm.ToString();
        delayInput.text = GameManager.level.songOffset.ToString();
        for (int i = 2; i <= 5; i++)
        {
            lineParent.GetChild(i - 2).transform.position = new Vector3(i * Screen.width / 12, Screen.height / 2, 0);
        }
        noGraduations = Screen.height / graduationSpan * 2;
        graduations = new List<TMP_Text>();
        for (int i = -noGraduations; i <= noGraduations; i++)
        {
            GameObject graduation = Instantiate(graduationPrefab, GraduationParent);
            graduation.transform.localPosition = new Vector3(0, i * graduationSpan, 0);
            TMP_Text tmp = graduation.GetComponent<TMP_Text>();
            tmp.SetText(i.ToString());
            graduations.Add(tmp);
        }
        for (int i = 0; i < 8; i++){
            divisionButtons[i] = divisionParent.Find((i+1).ToString()).GetComponent<TMP_Text>();
        }
        notes = DecodePattern(GameManager.level.pattern);
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            float touchedBeat = ScreenToBeatSnap(touch.position.y);
            int touchedLane = ScreenToLaneSnap(touch.position.x);
            if (touch.phase == TouchPhase.Began)
            {
                touchDistance = 0;
                EditorNote tmp = new EditorNote { beat = touchedBeat, lane = touchedLane };
                if (notes.TryGetValue(tmp, out tmp) && tmp is EditorLongNoteTail t){
                    holdingTail = t;
                }else{
                    holdingTail = null;
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if(holdingTail is null){
                    stageSpeed = touch.deltaPosition.y;
                }else if (touchedBeat != holdingTail.beat || touchedLane != holdingTail.lane && touchedBeat > holdingTail.parent.beat){
                    notes.Remove(holdingTail);
                    holdingTail.beat = touchedBeat;
                    holdingTail.lane = touchedLane;
                    notes.Add(holdingTail);
                    TransformLongNote(holdingTail.parent);
                }
            }
            else if (touch.phase == TouchPhase.Ended && touchDistance < 10)
            {
                if (0 <= touchedLane && touchedLane <= 4)
                {
                    EditorNote tmp = new EditorNote { beat = touchedBeat, lane = touchedLane };
                    if (notes.TryGetValue(tmp, out tmp))
                    {
                        if (tmp is EditorLongNote t) notes.Remove(t.tail);
                        Destroy(tmp.obj);
                        notes.Remove(tmp);
                    }else{
                        switch (noteType)
                        {
                            case "Note":
                                EditorNote note = new EditorNote { beat = touchedBeat, lane = touchedLane };
                                SpawnNote(note);
                                notes.Add(note);
                                break;
                            case "FlickNote":
                                EditorFlickNote flickNote = new EditorFlickNote { beat = touchedBeat, lane = touchedLane };
                                SpawnNote(flickNote);
                                notes.Add(flickNote);
                                break;
                            case "LongNote":
                                EditorLongNote longNote = new EditorLongNote { beat = touchedBeat, lane = touchedLane, tail = new EditorLongNoteTail() { beat = touchedBeat + 1, lane = touchedLane } };
                                longNote.tail.parent = longNote;
                                SpawnNote(longNote);
                                notes.Add(longNote);
                                notes.Add(longNote.tail);
                                break;
                        }
                    }
                }
            }
            touchDistance += Mathf.Abs(touch.deltaPosition.x) + Mathf.Abs(touch.deltaPosition.y);
        }

        if (Mathf.Abs(stageSpeed) > 1f)
        {
            stage.position += Vector3.up * stageSpeed;
            int beat = Mathf.FloorToInt(-stage.position.y / graduationSpan);
            if (beat > lastBeat)
            {
                TMP_Text first = graduations[0];
                graduations.RemoveAt(0);
                first.SetText((beat + noGraduations).ToString());
                first.rectTransform.localPosition = Vector3.up * (beat + noGraduations) * graduationSpan;
                graduations.Add(first);
            }
            else if (beat < lastBeat)
            {
                TMP_Text last = graduations[graduations.Count - 1];
                graduations.RemoveAt(graduations.Count - 1);
                last.SetText((beat - noGraduations).ToString());
                last.rectTransform.localPosition = Vector3.up * (beat - noGraduations) * graduationSpan;
                graduations.Insert(0, last);
            }
            lastBeat = beat;
            stageSpeed *= speedMultiplier;
        }
    }

    public void SwitchNote(string type)
    {
        noteType = type;
        noteIcon.color = type == "Note" ? Color.white : Color.gray;
        flickNoteIcon.color = type == "FlickNote" ? Color.white : Color.gray;
        longNoteIcon.color = type == "LongNote" ? Color.white : Color.gray;
    }

    public void Play()
    {
        GameManager.level.pattern = EditorUtility____.EncodeLevel(notes);
        Debug.Log(GameManager.level.pattern);
        GameManager.Play(GameManager.level);
    }

    public void Back()
    {
        GameManager.level.pattern = EditorUtility____.EncodeLevel(notes);
        Debug.Log(GameManager.level.pattern);
        GameManager.Back();
    }

    public void SpawnNote(EditorNote note){
        note.obj = Instantiate(notePrefab, noteParent);
        note.obj.transform.localPosition = BeatLaneToWorld(note.beat, note.lane);
        note.obj.GetComponent<RectTransform>().sizeDelta = Vector2.one * Screen.width / 14;
    }

    public void SpawnNote(EditorFlickNote note){
        note.obj = Instantiate(flickNotePrefab, noteParent);
        note.obj.transform.localPosition = BeatLaneToWorld(note.beat, note.lane);
        note.obj.GetComponent<RectTransform>().sizeDelta = Vector2.one * Screen.width / 14;
    }

    public void SpawnNote(EditorLongNote note){
        note.obj = Instantiate(longNotePrefab, noteParent);
        note.obj.transform.localPosition = BeatLaneToWorld(note.beat, note.lane);
        note.obj.GetComponent<RectTransform>().sizeDelta = Vector2.one * Screen.width / 14;
        note.obj.transform.Find("Head").GetComponent<RectTransform>().sizeDelta = Vector2.one * Screen.width / 14;
        note.obj.transform.Find("Tail").GetComponent<RectTransform>().sizeDelta = new Vector2(1, 0.5f) * Screen.width / 14;
        TransformLongNote(note);
    }

    public void TransformLongNote(EditorLongNote note)
    {
        Transform body = note.obj.transform.Find("Body");
        Transform tail = note.obj.transform.Find("Tail");
        Vector3 relative = BeatLaneToWorld(note.tail.beat, note.tail.lane) - BeatLaneToWorld(note.beat, note.lane);
        tail.localPosition = relative;
        body.localPosition = relative / 2;
        body.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 14f, relative.magnitude);
        body.eulerAngles = new Vector3(0, 0, Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg - 90);
    }

    public Vector3 BeatLaneToWorld(float beat, float lane) {
        return new Vector3(Screen.width / 12 * (lane - 3.5f), beat * graduationSpan, 0);
    }

    public int ScreenToLaneSnap(float x){
        return Mathf.FloorToInt(x / Screen.width * 2 * 6) - 2;
    }

    public float ScreenToBeatSnap(float y){
        return Mathf.Round((-stage.position.y + y) / graduationSpan * division) / division;
    }

    public void SetDivision(int n){
        divisionButtons[division-1].color = new Color(1, 1, 1, 0.25f);
        divisionButtons[n-1].color = new Color(1, 1, 1, 1f);
        division = n;
    }

    public SortedSet<EditorNote> DecodePattern(string pattern){
        SortedSet<EditorNote> notes = new SortedSet<EditorNote>();
        foreach(string noteInfo in pattern.Split('\n')){
            string[] nums = noteInfo.Split(' ');
            if (nums.Length == 2) {
                EditorNote note = new EditorNote { beat = float.Parse(nums[0]), lane = int.Parse(nums[1]) };
                SpawnNote(note);
                notes.Add(note);
            }else if (nums.Length == 3){
                EditorFlickNote note = new EditorFlickNote { beat = float.Parse(nums[0]), lane = int.Parse(nums[1]) };
                SpawnNote(note);
                notes.Add(note);
            }else if(nums.Length == 4){
                EditorLongNote note = new EditorLongNote { beat = float.Parse(nums[0]), lane = int.Parse(nums[1]) };
                note.tail = new EditorLongNoteTail { beat = note.beat + float.Parse(nums[2]), lane = int.Parse(nums[3]), parent = note };
                SpawnNote(note);
                notes.Add(note);
                notes.Add(note.tail);
            }
        }
        return notes;
    }
}
