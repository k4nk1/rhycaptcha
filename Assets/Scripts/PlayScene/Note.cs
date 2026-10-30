using UnityEngine;
using System.Collections.Generic;

public class Note
{
    public float time;
    public float endTime;
    public int lane;
    public int endLane;
    public GameObject obj;
    public NoteState state;

    public void Move(float noteSpeed)
    {
        obj.transform.position += Vector3.back * Time.deltaTime * noteSpeed;
    }

    public void Miss()
    {
        state = NoteState.Missed;
    }

    public void Perfect()
    {
        state = time == endTime ? NoteState.Touched : NoteState.Touching;
    }

    public void Good()
    {
        state = time == endTime ? NoteState.Touched : NoteState.Touching;
    }

    public void Destroy()
    {
        GameObject.Destroy(obj);
    }
}

public enum NoteState
{
    UnTouched,
    Touching,
    Touched,
    Missed
}
