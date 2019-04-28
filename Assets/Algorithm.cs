using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithm : MonoBehaviour
{
    Movement movement;
    Sensors sensors;
    public GridMap grid;



    public int turnTimer = 0;

    // The amount we think we're rotated
    public float assumedRotation = 0;
    public bool turning = false;

    Vector2 gridPos = new Vector2(50,0);
    int turnsToMake = 0;


    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Movement>();
        sensors = GetComponent<Sensors>();
        turnsToMake = 1;
        
    }

    void DrawGrid(float[] readings) {
        int x1 = Mathf.RoundToInt(gridPos.x);
        int y1 = Mathf.RoundToInt(gridPos.y);
        int x2 = Mathf.RoundToInt(gridPos.x);
        int y2 = Mathf.RoundToInt(gridPos.y + readings[4]*30);


        List<Vector2> points = GridMap.bresenham(x1, y1, x2, y2);
        Vector2 lastPoint = points[points.Count-1];

        

        foreach (Vector2 point in points) {
            grid.SetPixel(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y), System.Drawing.Color.White);
        }

        grid.SetPixel(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(lastPoint.y), System.Drawing.Color.Black);


    }

    void Rotate() {
        if (turnsToMake > 0)
        {
            movement.PutMovement(0, 1);
            assumedRotation += movement.maxAngleDelta;
            turnTimer--;
            if (turnTimer == 0)
            {
                turning = false;
                turnsToMake--;
            }
        }
        else if (turnsToMake < 0)
        {
            movement.PutMovement(0, 1);
            assumedRotation += movement.maxAngleDelta;
            turnTimer--;
            if (turnTimer == 0)
            {
                turning = false;
                turnsToMake++;
            }
        }
        else {
            throw new System.Exception("You shouldn't call rotate with turnsToMake = 0");
        }
        
    }

    // turnTo 1 means left turnTo -1 means right.
    // 73 frames to turn 90 deg
    // Update is called once per frame
    void LateUpdate()
    {

        DrawGrid(sensors.GetReadings());
        // Handles turning
        if (!turning && turnsToMake != 0)
        {
            // Set the turnTimer to the the number of frames it takes to turn 90 degrees if we move precisely
            turnTimer = Mathf.RoundToInt((Mathf.PI / 2) / (Mathf.Deg2Rad * movement.maxAngleDelta));
            turning = true;
            Rotate();
        }
        else if (turning) {
            Rotate();
        }
        // Handles logic for when we need to decide what to do
        else
        {
            movement.PutMovement(0, 0);
        }
    }
}
