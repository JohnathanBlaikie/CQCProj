using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FloatReference 
{
    public bool useConstant = true;
    public float constantValue;
    public SOFloat variable;

    public float Value
    {
        get { return useConstant ? constantValue : variable.Value; }
    }
}
