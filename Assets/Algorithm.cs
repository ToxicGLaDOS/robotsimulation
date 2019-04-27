using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithm : MonoBehaviour
{
    Movement movement;
    Sensors sensors;
    public GridMap grid;
    public WallGenerator wallGen;
    float xmin, ymin, xmax, ymax; 


       
    public int turnAmount = 0;

    public float gridPixelWidth, gridPixelHeight;
    public Vector2Int gridPos;
    int turnCounter = 0;
    int turnTo = 1;
    float error = 0;

    public int turnTimer = 0;

    // The amount we think we're rotated
    public float assumedRotation = 0;


    // Variable to mark that we're turning and shouldn't do anything until it's done
    public bool turning = false;
    int turnsToMake = 0;


    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Movement>();
        sensors = GetComponent<Sensors>();

        xmin = wallGen.xmin;
        ymin = -5;
        xmax = wallGen.xmax;
        ymax = 5;

        gridPixelWidth = (xmax - xmin) / grid.width;
        gridPixelHeight = (ymax - ymin) / grid.height;

        int y = Mathf.RoundToInt((ymax - transform.position.y) / gridPixelHeight);

        gridPos = new Vector2Int(grid.width / 2, y);

        turnsToMake = 1;
        
    }

    void DrawGrid(float[] readings) {
       
        for (int i = 0; i < readings.Length; i++)
        {
            float angle = (2 * Mathf.PI / 16) * i;
            int x1 = Mathf.RoundToInt(gridPos.x);
            int y1 = Mathf.RoundToInt(gridPos.y);
            int x2 = Mathf.RoundToInt(x1 + Mathf.Cos(angle) * (readings[i] / gridPixelWidth));
            int y2 = Mathf.RoundToInt(y1 - Mathf.Sin(angle) * (readings[i] / gridPixelHeight));

            List<Vector2Int> points = GridMap.bresenham(x1, y1, x2, y2);
            Vector2Int lastPoint = points[points.Count - 1];

            foreach (Vector2Int point in points)
            {
                grid.SetPixel(point.x, point.y, System.Drawing.Color.White);
            }

            grid.SetPixel(lastPoint.x, lastPoint.y, System.Drawing.Color.Black);
        }

    }

    void Move(float forward, float turn) {
        movement.PutMovement(forward, turn);
        error += movement.speed * forward / gridPixelHeight;
        while (error > 1)
        {
            gridPos += new Vector2Int(0, -1);
            error--;
        }
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
    void FixedUpdate()
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
