using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public delegate void timerCallbackDelegate();
    private float timer;
    private timerCallbackDelegate callBack;

    // callback is a function reference
    public void set(float timer, timerCallbackDelegate callBack) {
        this.timer = timer;
        this.callBack = callBack;
    }

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update()
    {
        if (timer > 0.0f) {
            timer -= Time.deltaTime;

            if (isTimerComplete()) {
                callBack();
            }
        }
    }

    public bool isTimerComplete() {
        return timer <= 0.0f;
    }
}
