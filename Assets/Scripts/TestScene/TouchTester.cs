using UnityEngine;
using UnityEngine.UI;

public class TouchTester : MonoBehaviour
{
    [SerializeField]
    private Text logText;

    void Start()
    {

    }

    void Update()
    {
        string info = "";
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            info += $"touch {i}:\n";
            info += $"  altitudeAngle: {touch.altitudeAngle}\n";
            info += $"  azimuthAngle: {touch.azimuthAngle}\n";
            info += $"  phase: {touch.phase}\n";
            info += $"  position: {touch.position}\n";
            info += $"  pressure: {touch.pressure}\n";
            info += $"  radius: {touch.radius}\n";
            info += $"  radiusVariance: {touch.radiusVariance}";
            Debug.Log(touch.phase);
        }
        logText.text = info;
    }
}
