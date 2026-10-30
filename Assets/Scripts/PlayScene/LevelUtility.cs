using UnityEngine;
using System.Collections.Generic;

public class LevelUtility
{
    public static Vector3 SpawnPoint(int lane, float laneWidth, int noLane, float laneLength)
    {
        return new Vector3(-laneWidth * (noLane - 1) / 2f + laneWidth * lane, 0, laneLength);
    }

    public static Queue<Note> DecodePattern(string pattern, float bpm, float offset)
    {
        Queue<Note> queue = new Queue<Note>();
        foreach (string row in pattern.Split('\n'))
        {
            string[] terms = row.Split(' ');
            Note note = new Note();
            //time
            note.time = Beat2Time(terms[0], bpm);
            if (note.time < offset) continue;
            //endtime
            note.endTime = note.time;
            if (terms.Length > 2) note.endTime += Beat2Time(terms[2], bpm);
            //lane
            note.lane = int.Parse(terms[1]);
            //endlane
            note.endLane = terms.Length == 4 ? int.Parse(terms[3]) : note.lane;
            queue.Enqueue(note);
        }
        return queue;
    }

    public static float Beat2Time(string beat, float bpm)
    {
        return float.Parse(beat) * 60 / bpm;
    }

    public static float Screen2Lane(Vector3 pos, int noLane){
        return (pos.x / (Screen.height - pos.y) * Screen.height / Screen.width + (1 - Screen.height / (Screen.height - pos.y)) / 2) * noLane;
    }
}
