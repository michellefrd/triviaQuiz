using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Respuesta 
{
    [Tooltip("The answer to a question")]
    public string answer;
    
    [Tooltip("This answer is correct")]
    public bool isCorrect = false; 
}
