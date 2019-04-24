using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithm : MonoBehaviour
{
    Movement movement;
    Sensors sensors;
    public GridMap grid;



    public int turnAmount = 0;

    Vector2 gridPos = new Vector2(50,0);
    int turnCounter = 0;
    int turnTo = 1;
    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Movement>();
        sensors = GetComponent<Sensors>();
    }

    void DrawGrid(float[] readings) {
        int x1 = Mathf.RoundToInt(gridPos.x);
        int y1 = Mathf.RoundToInt(gridPos.y);
        int x2 = Mathf.RoundToInt(gridPos.x);
        int y2 = Mathf.RoundToInt(gridPos.y + readings[4]*30);
        print(y1);
        print(y2);
        List<Vector2> points = GridMap.bresenham(x1, y1, x2, y2);
        Vector2 lastPoint = points[points.Count-1];

        

        foreach (Vector2 point in points) {
            grid.SetPixel(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y), System.Drawing.Color.White);
        }

        grid.SetPixel(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(lastPoint.y), System.Drawing.Color.Black);


    }

    // turnTo 1 means left turnTo -1 means right.
    // 73 frames to turn 90 deg
    // Update is called once per frame
    void LateUpdate()
    {

        DrawGrid(sensors.GetReadings());
        
    }
}
