using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    public float time = 0;
    public bool running = true;


    // Start is called before the first frame update
    void Start()
    {
        timerText.text = "0.00";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (running)
        {
            time += Time.fixedDeltaTime;
        }
        timerText.text = time.ToString("0.00");
    }

    public void Restart() {
        time = 0;
        running = false;
    }

    public void Stop() {
        running = false;
    }

    public void Begin() {
        running = true;
    }
}
