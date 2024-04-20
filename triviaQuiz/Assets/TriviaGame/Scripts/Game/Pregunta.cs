using System;
using System.Collections;
using System.Collections.Generic;
using TriviaQuizGame.Types;
using UnityEngine;

[Serializable]
public class Pregunta
{
		[Tooltip("The question presented")]
		public string question;
		
        [Tooltip("A list of answers to choose from. A question may have several correct/wrong answers")]
        public Respuesta[] answers;

		[Tooltip("If true, the player must select all correct questions and confirm to check the result")]
		public bool multiChoice = false;
		
        [Tooltip("How many point we get if we answer this question correctly. The bonus value is also used to sort the questions from the easy ( low bonus ) to the difficult ( high bonus )")]
		public float bonus;
		
		[Tooltip("How many seconds to answer this question we have. This should logically be tied to the difficulty of the question, same as the bonus. But the questions are sorted only based on the bonus, and not the time")]
		public float time;}

