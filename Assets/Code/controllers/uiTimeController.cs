using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class uiTimeController : MonoBehaviour
{
    public float timeIncrement = 0;
    public float timeAdditiveIncrement = 3600;
    public bool timeEnabled {get {return _timeEnabled;}}
    private bool _timeEnabled = false;

    public float maxTimeIncrement = 604800;

    public Slider slider;
    public TMP_InputField input;
    public Image playImage;

    public Sprite play, pause;

    void Update()
    {
        if (timeEnabled) master.globalTime += timeIncrement * Time.deltaTime;
    }

    public void manualEditTimeIncrement(string value)
    {
        if (value == "" || value == "-") return;
        timeIncrement = Convert.ToInt32(value);
        if (timeIncrement > maxTimeIncrement || timeIncrement < -maxTimeIncrement)
        {
            timeIncrement = Mathf.Max(Mathf.Min(timeIncrement, maxTimeIncrement), -maxTimeIncrement);
            input.text = $"{timeIncrement}";
        }
        slider.SetValueWithoutNotify(timeIncrement);
    }

    public void manualEditTimeIncrementEnd(string value)
    {
        value = value.Replace(' ', '\0');
        if (value == "" || value == "-") timeIncrement = 0;
        input.text = $"{timeIncrement}";
        slider.SetValueWithoutNotify(timeIncrement);
    }

    public void onSliderChange()
    {
        timeIncrement = slider.value;
        input.text = $"{timeIncrement}";
    }

    public void toggleTimeEnabled()
    {
        _timeEnabled = !_timeEnabled;

        if (!_timeEnabled) playImage.sprite = play;
        else playImage.sprite = pause;
    }

    public void addTime()
    {
        master.globalTime += timeAdditiveIncrement;
    }

    public void subTime()
    {
        master.globalTime -= timeAdditiveIncrement;
    }
}
