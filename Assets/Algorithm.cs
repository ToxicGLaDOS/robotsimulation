using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithm : MonoBehaviour
{
    Movement movement;
    Sensors sensors;

    public int turnAmount = 0;
    int turnCounter = 0;
    int turnTo = 1;
    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Movement>();
        sensors = GetComponent<Sensors>();
    }
    // turnTo 1 means left turnTo -1 means right.
    // 73 frames to turn 90 deg
    // Update is called once per frame
    void LateUpdate()
    {
        if (turnCounter == 0)
        {
            // readings[4] is forward
            float[] readings = sensors.GetReadings();
            // If there is something right in front of us
            // we turn the direction with more open space
            if (readings[4] < .5)
            {
                // If there is more space to the right
                if (readings[0] > readings[8])
                {
                    turnTo = -1;
                    turnCounter = 73;
                    turnAmount -= 1;
                }
                else {
                    turnTo = 1;
                    turnCounter = 73;
                    turnAmount += 1;
                }

            }
            // If we have turned before then we make some choices to try and turn back hopefully
            else if(turnAmount != 0){
                // If there is more space to the right and the slightly pointed backward to the right sensor gives enough space
                // then we know we can safely turn back
                if (readings[0] > readings[8] && readings[15] > .6 && turnAmount > 0)
                {
                    turnTo = -1;
                    turnCounter = 73;
                    turnAmount -= 1;
                }
                // Same as above but the other direction
                else if (readings[0] < readings[8] && readings[9] > .6 && turnAmount < 0)
                {
                    turnTo = 1;
                    turnCounter = 73;
                    turnAmount += 1;
                }
                else {
                    movement.PutMovement(1, 0);
                }
            }
            else {
                movement.PutMovement(1, 0);
            }
        }
        else
        {
            if (turnTo == 1)
            {
                movement.PutMovement(0, 1);
            }
            else {
                movement.PutMovement(0, -1);
            }

            turnCounter--;
        }
        
    }
}
