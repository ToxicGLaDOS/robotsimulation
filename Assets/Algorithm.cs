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


       
    // The width and height in unity units of one pixel in the grid
    public float gridPixelWidth, gridPixelHeight;

    // A reference to the goal so we can get its position to calibrate the grid
    public Goal goal;

    // The position we believe we are on the grid
    public Vector2Int gridPos;

    // The number of frames left we need to turn for
    public int turnTimer = 0;

    // The amount we think we're rotated
    public float assumedRotation = 0;

    // The maximum length a sensor can read before we consider it a wall
    public float maxLengthToWall;

    // Variable to mark that we're turning and shouldn't do anything until it's done
    public bool turning = false;

    // Variable to mark that we're moving and shouldn't be doing any logic
    public bool moving = false;

    // The number and direction of turns to make
    public int turnsToMake = 0;

    // Which way we're facing
    public Vector2Int facing = Vector2Int.up;

    // This value keeps track of how far off from the grid we think we are
    // this might happen, for example, because the grid is in 1 inch chunks, but we can only move in 1.5 inch increments
    private float error = 0;

    // Where the goal is on the grid
    private Vector2Int goalPos;

    // The path we are currently following
    private List<Vector2Int> path;

    // The amount we have left to move in world space units
    private float amountToMove = 0;

    private bool getPath = true;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Movement>();
        sensors = GetComponent<Sensors>();

        xmin = wallGen.xmin;
        ymin = -5;
        xmax = wallGen.xmax;
        ymax = 5;

        // Hardcoded to 5% error allowance
        maxLengthToWall = sensors.maxDist - sensors.maxDist * 0.05f;

        gridPixelWidth = (xmax - xmin) / grid.width;
        gridPixelHeight = (ymax - ymin) / grid.height;

        int y     = Mathf.RoundToInt((ymax - transform.position.y) / gridPixelHeight);
        int goaly = Mathf.RoundToInt((ymax - goal.transform.position.y) / gridPixelHeight);

        gridPos = new Vector2Int(grid.width / 2, y);
        goalPos = new Vector2Int(grid.width / 2, goaly);

        grid.SetPixel(goalPos.x, goalPos.y, GridMap.PixelStates.GOAL);
        grid.SetPixel(gridPos.x, gridPos.y, GridMap.PixelStates.ROBOT);
    }

    void DrawGrid(float[] readings) {
       
        for (int i = 0; i < readings.Length; i++)
        {
            // angle determines the angle we believe the sensor to be at
            float angle = (2 * Mathf.PI / 16) * i + Mathf.Deg2Rad * assumedRotation;
            int x1 = Mathf.RoundToInt(gridPos.x);
            int y1 = Mathf.RoundToInt(gridPos.y);
            int x2 = Mathf.RoundToInt(x1 + Mathf.Cos(angle) * (readings[i] / gridPixelWidth));
            int y2 = Mathf.RoundToInt(y1 - Mathf.Sin(angle) * (readings[i] / gridPixelHeight));

            List<Vector2Int> points = GridMap.bresenham(x1, y1, x2, y2);
            Vector2Int lastPoint = points[points.Count - 1];

            foreach (Vector2Int point in points)
            {
                grid.SetPixel(point.x, point.y, GridMap.PixelStates.EMPTY);
            }

            // If the sensor reads further than the max distance we consider the end to be empty
            if(readings[i] > maxLengthToWall)
                grid.SetPixel(lastPoint.x, lastPoint.y, GridMap.PixelStates.EMPTY);
            // If the sensor reads closer than the max distance we consider it a wall
            // this is because the max dist should be set lower than the error could produce on the robot
            else
                grid.SetPixel(lastPoint.x, lastPoint.y, GridMap.PixelStates.WALL);
        }

    }

    void MoveForward() {
        
        // If the amount we need to move is greater than the max we can move in one frame
        // then we just move at full speed for this frame and subtract the amount we just moved from the amount we have left to do
        if( amountToMove > movement.speed)
        {
            movement.PutMovement(1, 0);
            amountToMove -= movement.speed;
        }
        // Otherwise we need to calculate how much power to put into the wheels
        else
        {
            float forwardPower = amountToMove / movement.speed;

            // Sanity check that forwardPower is less than 1
            if (forwardPower > 1)
                throw new System.Exception("Forward velocity shouldn't be greater than 1");

            movement.PutMovement(forwardPower, 0);
            // We have to invert y here because the grid is inverted from the world space
            gridPos = new Vector2Int(gridPos.x + facing.x, gridPos.y - facing.y);
            moving = false;

        }
    }

    void Rotate() {
        
        // Turning left 
        if (turnsToMake > 0)
        {
            movement.PutMovement(0, 1);
            assumedRotation += movement.maxAngleDelta;
            turnTimer--;
            if (turnTimer == 0)
            {
                turning = false;
                turnsToMake--;
                facing = RotateVector(facing, 1);
            }
        }
        // Turning right
        else if (turnsToMake < 0)
        {
            movement.PutMovement(0, 1);
            assumedRotation += movement.maxAngleDelta;
            turnTimer--;
            if (turnTimer == 0)
            {
                turning = false;
                turnsToMake++;
                facing = RotateVector(facing, -1);
            }
        }
        else {
            throw new System.Exception("You shouldn't call rotate with turnsToMake = 0");
        }
        
    }

    private Vector2Int RotateVector(Vector2Int v, int direction) {
        if (direction == 1)
        {
            if (v.x == 1 && v.y == 0)
                return new Vector2Int(0, 1);
            else if (v.x == 0 && v.y == 1)
                return new Vector2Int(-1, 0);
            else if (v.x == -1 && v.y == 0)
                return new Vector2Int(0, -1);
            else if (v.x == 0 && v.y == -1)
                return new Vector2Int(1, 0);
            else
                throw new System.Exception("You can't call rotate vector with a vector that isn't of the form ([01(-1)], [01(-1)]");
        }
        else if (direction == -1)
        {
            if (v.x == 1 && v.y == 0)
                return new Vector2Int(0, -1);
            else if (v.x == 0 && v.y == 1)
                return new Vector2Int(1, 0);
            else if (v.x == -1 && v.y == 0)
                return new Vector2Int(0, 1);
            else if (v.x == 0 && v.y == -1)
                return new Vector2Int(-1, 0);
            else
                throw new System.Exception("You can't call rotate vector with a vector that isn't of the form ([01(-1)], [01(-1)]");
        }
        else {
            throw new System.Exception("You can't call rotate vector with a direction that isn't -1 or 1.");
        }
    }

    // turnTo 1 means left turnTo -1 means right.
    void FixedUpdate()
    {

        DrawGrid(sensors.GetReadings());

        
        //Debug.Break();
        // Handles turning
        if (!turning && turnsToMake != 0)
        {
            // Set the turnTimer to the the number of frames it takes to turn 90 degrees if we move precisely
            turnTimer = Mathf.RoundToInt((Mathf.PI / 2) / (Mathf.Deg2Rad * movement.maxAngleDelta));
            turning = true;
            Rotate();
        }
        else if (turning)
        {
            Rotate();
        }
        else if (moving)
        {
            MoveForward();
        }
        // Handles logic for when we need to decide what to do
        else
        {
            //Vector2Int v = path[0];
            Vector2Int v = new Vector2Int(0, 1);
            // Rotate to the direction we need to go
            if (v != facing)
            {
                
                turnsToMake = 1;
                Rotate();
            }
            else
            {
                // We need to move one grid pixel, so if we're facing right or left we need to go one pixel width
                if (facing == Vector2Int.left || facing == Vector2Int.right)
                    amountToMove = gridPixelWidth;
                // If we're facing up or down we need to go one pixel height
                else
                    amountToMove = gridPixelHeight;
                moving = true;
                MoveForward();
            }
        }
    }
}
