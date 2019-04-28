using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Range(0,.4f)]
    public float speed;
    [Range(0, 5)]
    public float maxAngleDelta;
    [Range(0, 1)]
    public float angleFuzzFactor;
    [Range(0, 1)]
    public float movementFuzzFactor;
    public float forward = 0;
    public float turn = 0;
    Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float angle = turn * 360;
        float rotationAmount = turn * maxAngleDelta + maxAngleDelta * Random.Range(-angleFuzzFactor, angleFuzzFactor);
        rb2d.MoveRotation(rb2d.rotation + rotationAmount);


        // Magic PI/2 makes the forward vector "up" instead of "right" from world space prespective
        float x = Mathf.Cos(Mathf.Deg2Rad * rb2d.rotation + Mathf.PI/2);
        float y = Mathf.Sin(Mathf.Deg2Rad * rb2d.rotation + Mathf.PI/2);
        Vector2 direction = new Vector2(x, y);
        float fuzzedForward = forward + forward * Random.Range(-movementFuzzFactor, movementFuzzFactor);
        rb2d.MovePosition(rb2d.position + direction * speed * fuzzedForward);
    }

    public void PutMovement(float forwardIn, float turnIn) {
        forward = forwardIn;
        turn = turnIn;
    }
}
