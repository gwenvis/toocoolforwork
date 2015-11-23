using UnityEngine;
using System.Collections;

public class Grind : MonoBehaviour {

    public Transform pointA;
    public Transform pointB;
    public float slope;

    public void Start()
    {
        slope = (pointB.position.y - pointA.position.y) / (pointB.position.x - pointA.position.x);
    }

    public float GetYPosition(float playerXPos)
    {
        Vector2 A = new Vector2(0, 0);
        Vector2 B = pointB.position - pointA.position;
        playerXPos = playerXPos - pointA.position.x;
        return slope * playerXPos + pointA.position.y;
    }
}
