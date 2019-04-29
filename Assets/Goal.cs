using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public Timer timer;
    public Algorithm robot;
    public Vector3 robotInitalPos;
    public WallGenerator wallGen;

    public int numTrials;
    public float averageTime = 0;


    public float totalTime = 0;

    public int trialsRan = 0;
    // Start is called before the first frame update
    void Start()
    {
        robotInitalPos = robot.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (robot != null && !robot.running)
        {
            totalTime += 120;
            Restart();
        }
    }

    void Restart()
    {

        trialsRan += 1;
        timer.Stop();
        totalTime += timer.time;
        timer.Restart();
        averageTime = totalTime / trialsRan;

        if (trialsRan == numTrials)
        {
            
            Destroy(robot.gameObject);
        }
        else
        {
            robot.Restart();
            wallGen.GenerateWalls();
            timer.Begin();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        Restart();
    }
}
