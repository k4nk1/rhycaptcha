using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public static Nullable<Vector3> TouchBegan() {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].phase == TouchPhase.Began)
                return Input.touches[i].position;
        }
        return null;
    }

    public static bool Touching(float lane, int noLane)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            float touchLane = LevelUtility.Screen2Lane(Input.touches[i].position, noLane);
            if (lane - 0.5f <= touchLane && touchLane <= lane + 0.5f)
                return true;
        }
        return false;
    }

    public static Nullable<Vector3> TouchEnded() {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].phase == TouchPhase.Ended)
                return Input.touches[i].position;
        }
        return null;
    }
}
