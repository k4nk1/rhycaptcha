using System;
using UnityEngine;

public class EditorNote : IComparable<EditorNote>
{
    public float beat;
    public int lane;
    public GameObject obj;

    public int CompareTo(EditorNote? other)
    {
        if (other == null) return 1;
        return (beat + lane * 0.01f).CompareTo(other.beat + other.lane * 0.01f);
    }
}

public class EditorFlickNote : EditorNote { }

public class EditorLongNote : EditorNote
{
    public EditorLongNoteTail tail;
}

public class EditorLongNoteTail : EditorNote {
    public EditorLongNote parent;
}
