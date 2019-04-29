using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensors : MonoBehaviour
{
    private float[] readings = new float[8];

    [Range(0,1)]
    public float fuzzFactor;
    public float maxDist;


    public float[] GetReadings() {
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 2 * Mathf.PI / 16 + Mathf.Deg2Rad * transform.rotation.eulerAngles.z;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            RaycastHit2D hit2d = Physics2D.Raycast(transform.position, dir, maxDist);
            // If no hit hit2d.distance will be 0 so we set it to max instead
            float orig_reading = hit2d.distance;
            if (orig_reading == 0)
            {
                orig_reading = maxDist;
            }
            readings[i] = orig_reading + orig_reading * Random.Range(-fuzzFactor, fuzzFactor);

            if (i == 4)
            {
                Debug.DrawLine(transform.position, (Vector2)transform.position + readings[i] * dir, Color.red);
            }
            else
            {
                Debug.DrawLine(transform.position, (Vector2)transform.position + readings[i] * dir, Color.white);
            }
        }
        return readings;
    }
}
