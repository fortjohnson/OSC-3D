﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class OSCSpaceReactor : MonoBehaviour
{

    public OSCSpaceController spaceController;

    private float equalityFidelity = 0.001f;
    private Vector3 prevPosition;
    private Vector3 prevStepValues;

    private Limits2D constrainLimit = new Limits2D(-0.5f, 0.5f);

    private void Start()
    {
        prevPosition = transform.localPosition;
    }

    private void FixedUpdate()
    {
        if (spaceController.sendContinously)
        {
            Vector3 pos = transform.localPosition;
            spaceController.SendOSCMessage(spaceController.oscAddress, CalcOSCValues(pos));
        }
    }

    private void Update()
    {
        Vector3 stepValues = CalcOSCValues(transform.localPosition);
        bool stepsChanged = !VRTK_SharedMethods.Vector3ShallowCompare(prevStepValues, stepValues, equalityFidelity);
        if (stepsChanged)
        {   
            spaceController.SendOSCMessage(spaceController.oscAddress, stepValues.x, stepValues.y, stepValues.z);
        }
        prevStepValues = stepValues;
    }

    private Vector3 CalcOSCValues(Vector3 pos)
    {
        Vector3 values = new Vector3();

        values.x = GetStepValue(MapValue(pos.x, constrainLimit, spaceController.xValueRange), spaceController.xStep);
        values.y = GetStepValue(MapValue(pos.y, constrainLimit, spaceController.yValueRange), spaceController.yStep);
        values.z = GetStepValue(MapValue(pos.z, constrainLimit, spaceController.zValueRange), spaceController.zStep);

        return values;
    }

    private float MapValue(float value, Limits2D inRange, Limits2D outRange)
    {
        return outRange.minimum + (outRange.maximum - outRange.minimum) * ((value - inRange.minimum) / (inRange.maximum - inRange.minimum));
    }


    public virtual float GetStepValue(float currentValue, float stepSize)
    {
        return Mathf.Round(currentValue / stepSize) * stepSize;
    }

}
