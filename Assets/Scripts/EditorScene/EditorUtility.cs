using System.Collections.Generic;

public static class EditorUtility____
{
    public static string EncodeLevel(SortedSet<EditorNote> notes){
        string level = "";
        foreach (EditorNote note in notes)
        {
            level += $"{note.beat} {note.lane}\n";
        }
        return level;
    }
}
