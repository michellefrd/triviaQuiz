using UnityEngine;
using UnityEngine.UI;
using System;


    [Serializable]
    public class Jugador
    {
        [Tooltip("The email of the player")]
        public string email = "Player 1";

        [Tooltip("The text that displays the name of the player")]
        public Transform nameText;

        //The current score of the player
        internal float score = 0;
        internal float scoreCount = 0;

        [Tooltip("The text that displays the current score of the player")]
        public Transform scoreText;

        //The current lives of the player
        internal float lives = 3;
		
        [Tooltip("The image that displays how many lives the player has left")]
        public Image livesBar;

        [Tooltip("The color of the player name")]
        public Color color;
    }

