//Version 1.99 (26.02.2018)

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using TriviaQuizGame.Types;
using System.Collections.Generic;
using TigerForge.UniDB;

/// <summary>
/// This script controls the game, starting it, following game progress, and finishing it with game over.
/// </summary>
public class TriviaGameManager : MonoBehaviour
{
    private int selectedCategoryID;
    
    private UniDB.Trivia triviaDB;

    // Holds the current event system
    internal EventSystem eventSystem;

    // Holds the name of the category loaded into this quiz, if it exists
    internal string currentCategory;

    [Header("<Player Options>")]
    [Tooltip(
        "A list of the players in the game. Each player can be assigned a name, a score text, lives and lives bar. You must have at least one player in the list in order to play the game. You don't need to assign all fields. For example, a player may have a name with no lives bar and it will work fine.")]
    public Player player;

    [Tooltip(
        "The number of lives each player has. You lose a life if time runs out, or you answer wrongly too many times")]
    public float lives = 3;

    // The width of a single life in the lives bar. This is calculated from the total width of a life bar divided by the number of lives
    internal float livesBarWidth = 128;

    internal RectTransform playersObject;

    [Header("<Question Options>")] [Tooltip("The object that displays the current question")]
    public Transform questionObject;

    [Tooltip(
        "A list of all possible questions in the game. Each question has a number of correct/wrong answers, a followup text, a bonus value, time, and can also have an image/video as the background of the question")]
    public Pregunta[] questions;

    //public Question[] questions;
    internal Pregunta[] questionsTemp;

    [Tooltip(
        "The number of the first question being asked. You can change this to start from a higher question number")]
    public int firstQuestion = 1;

    // The index of the current question being asked. -1 is the index of the first question, 0 the index of the second, and so on
    internal int currentQuestion = 0;

    // Is a question being asked right now?
    internal bool askingQuestion;

    [Tooltip(
        "Randomize the list of questions. Use this if you don't want the questions to appear in the same order every time you play. Combine this with 'sortQuestions' if you want the questions to be randomized within the bonus groups.")]
    public bool randomizeQuestions = true;

    //The buttons that display the possible answers
    internal Transform[] answerObjects;

    [Tooltip("Randomize the display order of answers when a new question is presented")]
    public bool randomizeAnswers = true;

    // The currently selected answer when using the ButtonSelector
    internal int currentAnswer = 0;
    internal GameObject multiChoiceButton;

    // Holds the answers when shuffling them
    internal string[] tempAnswers;

    [Tooltip(
        "Sort the list of questions from lowest bonus to highest bonus and put them into groups. Use this if you want the questions to be displayed from the easiest to the hardest ( The difficulty of a question is decided by the bonus value you give to it )")]
    public bool sortQuestions = true;

    [Tooltip(
        "Prevent a quiz from repeating questions. Once all questions in a quiz have been asked, they will repeat again.")]
    public bool dontRepeatQuestions = true;
    
    internal int defaultQuestionsPerGroup;
    internal int questionCount = 0;

    // The progress object which shows all the progress tabs and how many questions are left to win
    internal RectTransform progressObject;

    // The progress tab and text objects which show which question we are on. The tab also shows if we answered a question correctly or not
    internal GameObject progressTabObject;
    internal GameObject progressTextObject;

    // The size of the progress tab. This is calculated automatically and used to align the progress bar to the center.
    internal float progressTabSize;

    [Tooltip(
        "Limit the total number of questions asked, regardless of whether we answered correctly or not. Use this if you want to have a strict number of questions asked in the game (ex: 10 questions). If you keep it at 0 the number of questions will not be limited and you will go through all the question groups in the quiz before finishing it")]
    public int questionLimit = 0;

    // The total number of questions we asked. This is used to check if we reached the question limit.
    internal int questionLimitCount = 0;

    // The number of questions we answered correctly. This is used for displaying the result at the end of the game
    internal int correctAnswers = 0;
    internal float wrongAnswers = 0;

    [Tooltip(
        "The maximum number of mistakes allowed. If you make to many mistakes you lose a life and move to the next question")]
    public int maximumMistakes = 2;

    internal int mistakeCount = 0;

    // How many seconds are left before game over
    internal float timeLeft = 10;

    // Is the timer running?
    internal bool timerRunning = false;

    [Tooltip(
        "If we set this time higher than 0, it will override the individual times for each question. The global time does not reset between questions")]
    public float globalTime = 0;

    [Tooltip("How many seconds do we lose from the timer when we make a mistake")]
    public float timeLoss = 0;

    [Tooltip("How many seconds do we add to the timer when answering correctly")]
    public float timeBonus = 0;

    // The bonus we currently have
    internal float bonus;

    [Tooltip(
        "The percentage we lose from our potential bonus if we answer a question wrongly. For example 0.5 makes us lose half the bonus if we answer wrongly once, and ¾ of the bonus if we answer twice incorrectly.")]
    public float bonusLoss = 0.5f;

    // The highscore recorded for a level ( used in single player only )
    internal float highScore = 0;

    [Tooltip(
        "You can set a score limit, if you reach it you win the quiz. If you don't want a score limit just keep it at 0")]
    public float scoreToVictory = 0;

    [Tooltip("Highlight the correct answer/s when showing the result")]
    public bool showCorrectAnswer = true;

    [Tooltip(
        "Set the same question time for all questions in the quiz. This overwrites the individual times set on each question. Keep this at 0 if you don't want to overwrite any values")]
    public float quizTime = 0;

    [Tooltip(
        "Set the same question bonus for all questions in the quiz. This overwrites the individual bonuses set on each question. Keep this at 0 if you don't want to overwrite any values")]
    public float quizBonus = 0;

    [Header("<User Interface Options>")]
    [Tooltip("The bonus object that displays how much we can win if we answer correctly")]
    public Transform bonusObject;

    [Tooltip(
        "The menu that appears at the start of the game. This is used in the hotseat mode where we show a menu asking how many players want to participate")]
    public Transform startCanvas;

    //The canvas of the timer in the game
    internal GameObject timerIcon;
    internal Image timerBar;
    internal Text timerText;

    // This is a special animation-based timer. Instead of filling up a bar it calculates the animation of the timer
    internal Animation timerAnimated;

    [Tooltip("The menu that appears if we lose all lives in a single player game")]
    public Transform gameOverCanvas;

    [Tooltip("The menu that appears after finishing all the questions in the game. Used for single player and hotseat")]
    public Transform victoryCanvas;


    // Is the game over?
    internal bool isGameOver = false;

    [Tooltip("The level of the main menu that can be loaded after the game ends")]
    public string mainMenuLevelName = "CS_StartMenu";

    [Header("<Animation & Sounds>")] [Tooltip("The animation that plays when showing an answer")]
    public AnimationClip animationShow;

    [Tooltip("The animation that plays when hiding an answer")]
    public AnimationClip animationHide;

    [Tooltip("The animation that plays when choosing the correct answer")]
    public AnimationClip animationCorrect;

    [Tooltip("The animation that plays when choosing the wrong answer")]
    public AnimationClip animationWrong;

    [Tooltip("The animation that plays when showing a new question")]
    public AnimationClip animationQuestion;

    [Tooltip("Various sounds and their source")]
    public AudioClip soundQuestion;

    public AudioClip soundCorrect;
    public AudioClip soundWrong;
    public AudioClip soundTimeUp;
    public AudioClip soundGameOver;
    public AudioClip soundVictory;
    public string soundSourceTag = "Sound";
    internal GameObject soundSource;

    // This counts the time of the current sound playing now, so that we don't play another sound
    internal float soundPlayTime = 0;
    internal bool isPaused = false;

    // A general use index
    internal int index = 0;
    internal int indexB = 0;

    internal bool keyboardControls = false;

    // This stat keeps track of the time if took from the start of the quiz to the end of the quiz (gameover or victory)
    internal DateTime startTime;
    internal TimeSpan playTime;

    /// <summary>
    /// Start is only called once in the lifetime of the behaviour.
    /// The difference between Awake and Start is that Start is only called if the script instance is enabled.
    /// This allows you to delay any initialization code, until it is really needed.
    /// Awake is always called before any Start functions.
    /// This allows you to order initialization of scripts
    /// </summary>
    void Start()
    {
        // Obtiene el ID de la categoría seleccionada desde PlayerPrefs
        selectedCategoryID = PlayerPrefs.GetInt("SelectedCategoryID", 0);

        // Haz lo que necesites con el ID de la categoría seleccionada
        Debug.Log("Categoría seleccionada: " + selectedCategoryID);
        
        triviaDB = new UniDB.Trivia(); // Asumiendo que ya tienes una conexión configurada

        LoadQuestionsFromDatabase(selectedCategoryID);

        // Disable multitouch so that we don't tap two answers at the same time ( prevents multi-answer cheating, thanks to Miguel Paolino for catching this bug )
        Input.multiTouchEnabled = false;

        // Cache the current event system so we can enable and disable it between questions
        eventSystem = UnityEngine.EventSystems.EventSystem.current;

        // If the quiz is running on a mobile platform ( iOS, Android, etc ), disable the Standalone Input Module, so that gamepad can't take over
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            eventSystem.GetComponent<StandaloneInputModule>().enabled = false;
        }

        //Hide the game over ,victory ,and larger image screens
        if (gameOverCanvas) gameOverCanvas.gameObject.SetActive(false);
        if (victoryCanvas) victoryCanvas.gameObject.SetActive(false);

        //Get the highscore for the player
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
        highScore = PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name + "HighScore", 0);
        Debug.Log("Player high score: " + highScore);
#else
            highScore = PlayerPrefs.GetFloat(Application.loadedLevelName + "HighScore", 0);
#endif

        //Assign the timer icon and text for quicker access
        if (GameObject.Find("TimerIcon"))
        {
            timerIcon = GameObject.Find("TimerIcon");
            if (GameObject.Find("TimerIcon/Bar")) timerBar = GameObject.Find("TimerIcon/Bar").GetComponent<Image>();
            if (GameObject.Find("TimerIcon/Text")) timerText = GameObject.Find("TimerIcon/Text").GetComponent<Text>();
        }


        //Assign the players object for quicker access to player names, scores, and lives
        if (GameObject.Find("PlayersObject"))
        {
            playersObject = GameObject.Find("PlayersObject").GetComponent<RectTransform>();
        }

        //Assign the sound source for easier access
        if (GameObject.FindGameObjectWithTag(soundSourceTag))
            soundSource = GameObject.FindGameObjectWithTag(soundSourceTag);

        // Clear the bonus object text
        if (bonusObject) bonusObject.Find("Text").GetComponent<Text>().text = "";

        // Clear the question text
        questionObject.Find("Text").GetComponent<Text>().text = "";

        // Hide the CONTINUE button that moves to the next question
        if (questionObject.Find("ButtonContinue")) questionObject.Find("ButtonContinue").gameObject.SetActive(false);

        // Hide multi choice confirm button, set reference, and button listener 
        if (transform.Find("MultiChoiceButton"))
        {
            multiChoiceButton = transform.Find("MultiChoiceButton").gameObject;
            multiChoiceButton.SetActive(false);
            multiChoiceButton.GetComponent<Button>().onClick.AddListener(delegate() { CheckMultiChoice(); });
        }

        // Find the "Answer" object which holds all the answer buttons, and assign all the answer buttons to an array for easier access
        if (GameObject.Find("Answers"))
        {
            // Define the answers holder which we will fill with the answer buttons
            Transform answersHolder = GameObject.Find("Answers").transform;

            // Prepare the list of answer buttons so we can fill it out
            answerObjects = new Transform[answersHolder.childCount];

            // Assign the answer buttons
            for (index = 0; index < answerObjects.Length; index++) answerObjects[index] = answersHolder.GetChild(index);

            // Listen for a click on each button to choose the answer
            foreach (Transform answerObject in answerObjects)
            {
                // We need this temporary variable to be able to assign event listeners to multiple objects
                Transform tempAnswerObject = answerObject;

                // Listen for a click to choose the answer
                tempAnswerObject.GetComponent<Button>().onClick.AddListener(delegate()
                {
                    ChooseAnswer(tempAnswerObject);
                });
            }
        }

        // Clear all the answers
        foreach (Transform answerObject in answerObjects)
        {
            // Clear the answer text
            answerObject.Find("Text").GetComponent<Text>().text = "";

            // Hide answer outline 
            if (answerObject.Find("Outline")) answerObject.Find("Outline").GetComponent<Image>().enabled = false;

            // Deactivate the answer object
            answerObject.gameObject.SetActive(false);
        }

        // If we have a start canvas, pause the game and display it. Otherwise, just start the game.
        if (startCanvas)
        {
            // Show the start screen
            startCanvas.gameObject.SetActive(true);
        }
    }

    private void LoadQuestionsFromDatabase(int selectedCategoryID)
{
    var questionsTable = triviaDB.GetTable_Questions(); // Obtiene la tabla de preguntas
    _ = questionsTable
        .Select()
        .Where(questionsTable.C.category.Equal(selectedCategoryID)) // Filtra por el ID de la categoría seleccionada
        .Run((List<UniDB.Trivia.Questions.Record> records, Info info) =>
        {
            if (info.isOK && records.Count > 0)
            {
                questions = new Pregunta[records.Count]; // Inicializa el array con el número de preguntas obtenidas
                int index = 0;

                foreach (var record in records)
                {
                    questions[index] = new Pregunta
                    {
                        question = record.question,
                        answers = new[]
                        {
                            new Respuesta { answer = record.option0, isCorrect = record.correctAnswer == 0 },
                            new Respuesta { answer = record.option1, isCorrect = record.correctAnswer == 1 },
                            new Respuesta { answer = record.option2, isCorrect = record.correctAnswer == 2 },
                            new Respuesta { answer = record.option3, isCorrect = record.correctAnswer == 3 }
                        },
                        multiChoice = false, // Configurar si la pregunta permite múltiples respuestas correctas
                        bonus = (int)record.bonus, // Configurar según la lógica del juego
                        time = 30 // Configurar según la lógica del juego
                    };
                    index++; // Incrementa el índice para el siguiente elemento del array
                }

                Debug.Log("Preguntas cargadas correctamente!");
                // Aquí podrías iniciar el juego o mostrar la primera pregunta
                StartGame();
            }
            else
            {
                Debug.LogError("Error al cargar preguntas: " + info.error);
            }
        });
}

    /// <summary>
    /// Prepares the question list and starts the game. Also loads the XML questions from an online address if it exists.
    /// </summary>
    public void StartGame()
    {
        isGameOver = false;

        // Record the game start time so we can check how long the quiz took at the end of the game
        startTime = DateTime.Now;

        // The index of the first question in the question list is actually -1, so we adjust the number from the component ( 1 becomes -1, 2 becomes 0, 10 becomes 8, etc )
        currentQuestion = firstQuestion;

        // Reset the question counter
        questionCount = 0;

        // Make sure the question limit isn't larger than the actual number of questions available
        questionLimit = Mathf.Clamp(questionLimit, 0, questions.Length);

        // If we have a text object in the progress object, assign it and set the number of the first question
        if (GameObject.Find("ProgressObject/Text"))
        {
            // Assign the text object
            progressTextObject = GameObject.Find("ProgressObject/Text");

            // Reset the question limit counter
            questionLimitCount = 0;

            // Update the question count in the text
            if (progressTextObject)
                progressTextObject.GetComponent<Text>().text =
                    questionLimitCount.ToString() + "/" + questionLimit.ToString();
        }

        // Go through all the questions and overwrite their time values with the quiz-wide values, if they exist
        if (quizTime > 0)
            foreach (Pregunta question in questions)
                question.time = quizTime;

        // Go through all the questions and overwrite their bonus values with the quiz-wide values, if they exist
        if (quizBonus > 0)
            foreach (Pregunta question in questions)
                question.bonus = quizBonus;

        // Set the list of questions for this match
        SetQuestionList();

        // Ask the first question
        StartCoroutine(AskQuestion(false));
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // Move the progress object so that the current question is centered in the screen
        if (progressObject && progressTabObject)
            progressObject.anchoredPosition =
                new Vector2(
                    Mathf.Lerp(progressObject.anchoredPosition.x, progressTabSize * (0.5f - questionLimitCount),
                        Time.deltaTime * 10), progressObject.anchoredPosition.y);

        // Make the score count up to its current value, for the current player
        if (player.score < player.scoreCount)
        {
            // Count up to the courrent value
            player.score = Mathf.Lerp(player.score,
                player.scoreCount, Time.deltaTime * 10);

            // Round up the score value
            player.score = Mathf.CeilToInt(player.score);

            // Update the score text
            UpdateScore();
        }

        // Update the lives bar

        // If the lives bar has a text in it, update it. Otherwise, resize the lives bar based on the number of lives left
        if (player.livesBar.transform.Find("Text"))
            player.livesBar.transform.Find("Text").GetComponent<Text>().text =
                player.lives.ToString();
        else
            player.livesBar.rectTransform.sizeDelta = Vector2.Lerp(
                player.livesBar.rectTransform.sizeDelta,
                new Vector2(player.lives * livesBarWidth,
                    player.livesBar.rectTransform.sizeDelta.y), Time.deltaTime * 8);


        if (isGameOver == false)
        {
            // If we use the keyboard or gamepad, keyboardControls take effect
            if (keyboardControls == false && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
            {
                keyboardControls = true;

                // If no answer is selected, select the next available answer button
                if (askingQuestion == true && eventSystem.firstSelectedGameObject == null)
                {
                    // Go through the answer buttons and select the first available one
                    for (index = 0; index < answerObjects.Length; index++)
                    {
                        if (answerObjects[index].GetComponent<Button>().IsInteractable() == true)
                        {
                            eventSystem.SetSelectedGameObject(answerObjects[index].gameObject);

                            break;
                        }
                    }
                }
            }

            // If we move the mouse in any direction or click it, or touch the screen on a mobile device, then keyboard/gamepad controls are lost
            if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0 || Input.GetMouseButtonDown(0) ||
                Input.touchCount > 0) keyboardControls = false;

            // Count down the time until game over
            if (timeLeft > 0 && timerRunning == true)
            {
                // Count down the time
                timeLeft -= Time.deltaTime;
            }

            // Update the timer
            UpdateTime();
        }

        // Update the play sound time
        if (soundPlayTime > 0) soundPlayTime -= Time.deltaTime;
    }

    /// <summary>
    /// Sets the question list, first shuffling them, then sorting them by bonus value, 
    /// and finally choosing a limited number of questions from each bonus group
    /// </summary>
    void SetQuestionList()
    {
        // Shuffle all the available questions
        if (randomizeQuestions == true) questions = ShuffleQuestions(questions);

    }

    /// <summary>
    /// Shuffles the specified questions list, and returns it
    /// </summary>
    /// <param name="questions">A list of questions</param>
    Pregunta[] ShuffleQuestions(Pregunta[] questions)
    {
        // Go through all the questions and shuffle them
        for (index = 0; index < questions.Length; index++)
        {
            // Hold the question in a temporary variable
            Pregunta tempQuestion = questions[index];

            // Choose a random index from the question list
            int randomIndex = UnityEngine.Random.Range(index, questions.Length);

            // Assign a random question from the list
            questions[index] = questions[randomIndex];

            // Assign the temporary question to the random question we chose
            questions[randomIndex] = tempQuestion;
        }

        return questions;
    }

    /// <summary>
    /// Shuffles the specified answers list, and returns it
    /// </summary>
    /// <param name="answers">A list of answers</param>
    Respuesta[] ShuffleAnswers(Respuesta[] answers)
    {
        // Go through all the answers and shuffle them
        for (index = 0; index < answers.Length; index++)
        {
            // Hold the question in a temporary variable
            Respuesta tempAnswer = answers[index];

            // Choose a random index from the question list
            int randomIndex = UnityEngine.Random.Range(index, answers.Length);

            // Assign a random question from the list
            answers[index] = answers[randomIndex];

            // Assign the temporary question to the random question we chose
            answers[randomIndex] = tempAnswer;
        }

        return answers;
    }


    /// <summary>
    /// Presents a question from the list, along with possible answers.
    /// </summary>
    IEnumerator AskQuestion(bool animateQuestion)
    {
        if (isGameOver == false)
        {
            // This boolean is used to check if we already asked this question, and then ask another instead
            bool questionIsUsed = false;

            // We are now asking a question
            askingQuestion = true;
            // Go to the next question
            currentQuestion++;

            // If we got to the last question in the quiz, check if there are unused questions we can put in the quiz again
            if (dontRepeatQuestions == true && currentQuestion >= questions.Length)
            {
                // Go through all the questions and reset their "used" status, so that we can use them in the quiz again
                for (index = 0; index < questions.Length; index++) PlayerPrefs.DeleteKey(questions[index].question);

                // Display a text indicating that we are resetting the question list 
                questionObject.Find("Text").GetComponent<Text>().text =
                    " All questions in the quiz have been asked, \nResetting questions list... ";

                // Wait a couple of seconds to display the reset message
                yield return new WaitForSeconds(2.0f);

                // Reset the question index back to 0, so we begin from the first question
                currentQuestion = 0;
            }

            // Check if the question has already been used, and if so, ask another question instead
            if (currentQuestion < questions.Length && dontRepeatQuestions == true &&
                PlayerPrefs.HasKey(questions[currentQuestion].question)) questionIsUsed = true;

            // Animate the question
            if (animationQuestion && questionIsUsed == false)
            {
                // If the animation clip doesn't exist in the animation component, add it
                if (questionObject.GetComponent<Animation>().GetClip(animationQuestion.name) == null)
                    questionObject.GetComponent<Animation>().AddClip(animationQuestion, animationQuestion.name);

                // Play the animation
                questionObject.GetComponent<Animation>().Play(animationQuestion.name);

                // Wait for half the animation time, then display the next question. This will make the question appear while the question tab flips. Just a nice effect
                yield return new WaitForSeconds(questionObject.GetComponent<Animation>().clip.length * 0.5f);
            }

            if (questionIsUsed == false)
            {
                // If we still have questions in the list, ask the next question
                if (currentQuestion < questions.Length)
                {
                    // Display the current question
                    questionObject.Find("Text").GetComponent<Text>().text = questions[currentQuestion].question;

                    // if the question is multi choice, show check button
                    if (multiChoiceButton) multiChoiceButton.SetActive(questions[currentQuestion].multiChoice);

                    // Record this question in PlayerPrefs so that we know it has been asked
                    PlayerPrefs.SetInt(questions[currentQuestion].question, 1);

                    // Set the time for this question, unless we have a global timer, in which case ignore the local time of the question
                    if (globalTime <= 0) timeLeft = questions[currentQuestion].time;

                    // Start the timer
                    timerRunning = true;


                    // Clear all the answers
                    foreach (Transform answerObject in answerObjects)
                    {
                        answerObject.Find("Text").GetComponent<Text>().text = "";

                        // Hide the answer outline
                        if (answerObject.Find("Outline"))
                            answerObject.Find("Outline").GetComponent<Image>().enabled = false;

                        // If the answer has an image, clear it and hide it
                        if (answerObject.Find("Image"))
                        {
                            answerObject.Find("Image").GetComponent<Image>().sprite = null;
                            answerObject.Find("Image").gameObject.SetActive(false);
                        }

                        // If the answer has a video, hide it
                        if (answerObject.Find("Video")) answerObject.Find("Video").gameObject.SetActive(false);
                    }

                    // Shuffle the list of answers
                    if (randomizeAnswers == true)
                        questions[currentQuestion].answers = ShuffleAnswers(questions[currentQuestion].answers);

                    // Display the wrong and correct answers in the answer slots
                    for (index = 0;
                         index < questions[currentQuestion].answers.Length;
                         index++) //  answerObjects.Length; index++)
                    {
                        // If the answer object is inactive, activate it
                        if (answerObjects[index].gameObject.activeSelf == false)
                            answerObjects[index].gameObject.SetActive(true);

                        // Play the animation Show
                        if (animationShow)
                        {
                            // If the animation clip doesn't exist in the animation component, add it
                            if (answerObjects[index].GetComponent<Animation>().GetClip(animationShow.name) == null)
                                answerObjects[index].GetComponent<Animation>()
                                    .AddClip(animationShow, animationShow.name);

                            // Play the animation
                            answerObjects[index].GetComponent<Animation>().Play(animationShow.name);
                        }

                        // Enable the button so we can press it
                        answerObjects[index].GetComponent<Button>().interactable = true;

                        // Select each button as it becomes enabled. This action solves a bug that appeared in Unity 5.5 where buttons stay highlighted from the previous question.
                        answerObjects[index].GetComponent<Button>().Select();

                        // Display the text of the answer
                        if (index < questions[currentQuestion].answers.Length)
                            answerObjects[index].Find("Text").GetComponent<Text>().text =
                                questions[currentQuestion].answers[index].answer;
                        else answerObjects[index].gameObject.SetActive(false);
                    }

                    // If we started a new bonus group, reset the question counter
                    if (bonus > questions[currentQuestion].bonus) questionCount = 0;

                    // Set the bonus we can get for this question 
                    bonus = questions[currentQuestion].bonus;

                    if (bonusObject && bonusObject.GetComponent<Animation>())
                    {
                        // Animate the bonus object
                        bonusObject.GetComponent<Animation>().Play();

                        // Reset the bonus animation
                        bonusObject.GetComponent<Animation>()[bonusObject.GetComponent<Animation>().clip.name].speed =
                            -1;

                        // Display the bonus text
                        bonusObject.Find("Text").GetComponent<Text>().text = bonus.ToString();
                    }

                    // If keyboard controls are on, highlight the first answer. Otherwise, deselect all answers
                    if (keyboardControls == true) eventSystem.SetSelectedGameObject(answerObjects[0].gameObject);
                    else eventSystem.SetSelectedGameObject(null);

                    //If there is a source and a sound, play it from the source
                    if (soundSource && soundQuestion)
                        soundSource.GetComponent<AudioSource>().PlayOneShot(soundQuestion);
                }
                else // If we have no more questions in the list, win the game
                {
                    //Disable the question object
                    //questionObject.gameObject.SetActive(false);

                    //If we have no more questions, we win the game!
                    StartCoroutine(Victory(0));
                }

                // If we have a question limit, count towards it to win
                if (isGameOver == false && questionLimit > 0)
                {
                    questionLimitCount++;

                    if (progressTextObject)
                    {
                        // Update the question count in the text
                        progressTextObject.GetComponent<Text>().text =
                            questionLimitCount.ToString() + "/" + questionLimit.ToString();
                    }

                    // If we reach the question limit, win the game
                    if (questionLimitCount > questionLimit) StartCoroutine(Victory(0));
                }
            }
            else
            {
                // Ask the next question
                StartCoroutine(AskQuestion(true));
            }
        }
    }

    /// <summary>
    /// Chooses an answer from the list by index
    /// </summary>
    /// <param name="answerIndex">The number of the answer we chose</param>
    public void ChooseAnswer(Transform answerSource)
    {
        // Get the index of this answer object
        int answerIndex = answerSource.GetSiblingIndex();

        // We can only choose an answer if a question is being asked now
        if (askingQuestion == true)
        {
            // If this is a multi-choice question, allow the player to choose more than one question before checking the result
            if (questions[currentQuestion].multiChoice == true)
            {
                if (answerObjects[answerIndex].Find("Outline"))
                    answerObjects[answerIndex].Find("Outline").GetComponent<Image>().enabled =
                        !answerObjects[answerIndex].Find("Outline").GetComponent<Image>().enabled;

                return;
            }

            // If the chosen answer is wrong, disable it and reduce the bonus for this question
            //if ( answerObjects[answerIndex].Find("Text").GetComponent<Text>().text != questions[currentQuestion].correctAnswer )
            if (questions[currentQuestion].answers[answerIndex].isCorrect == false)
            {
                // Play the animation Wrong
                if (animationWrong)
                {
                    // If the animation clip doesn't exist in the animation component, add it
                    if (answerObjects[answerIndex].GetComponent<Animation>().GetClip(animationWrong.name) == null)
                        answerObjects[answerIndex].GetComponent<Animation>()
                            .AddClip(animationWrong, animationWrong.name);

                    // Play the animation
                    answerObjects[answerIndex].GetComponent<Animation>().Play(animationWrong.name);
                }

                // Disable the button so we can't press it again
                answerObjects[answerIndex].GetComponent<Button>().interactable = false;

                // If no answer is selected, select the next available answer button
                if (eventSystem.firstSelectedGameObject == null)
                {
                    // Go through the answer buttons and select the first available one
                    for (index = 0; index < answerObjects.Length; index++)
                    {
                        if (answerObjects[index].GetComponent<Button>().IsInteractable() == true)
                        {
                            if (Application.isMobilePlatform == false && keyboardControls == true)
                                eventSystem.SetSelectedGameObject(answerObjects[index].gameObject);

                            break;
                        }
                    }
                }

                // Set the color of the tab to red, if it exists
                if (progressTabObject)
                    progressObject.transform.GetChild(questionLimitCount).GetComponent<Image>().color = Color.red;

                // Cut the bonus to half its current value
                bonus *= bonusLoss;

                // Lose some time as a penalty
                timeLeft -= timeLoss;

                // Display the bonus text
                if (bonusObject) bonusObject.Find("Text").GetComponent<Text>().text = bonus.ToString();

                // Increase the mistake count
                mistakeCount++;

                // If we reach the maximum number of mistakes, give no bonus and move on to the next question
                if (mistakeCount >= maximumMistakes)
                {
                    // Give no bonus
                    bonus = 0;

                    // Display the bonus text
                    if (bonusObject) bonusObject.Find("Text").GetComponent<Text>().text = bonus.ToString();

                    // Reduce from lives
                    player.lives--;

                    // Update the lives we have left
                    Updatelives();

                    // Add to the stat wrong answer
                    wrongAnswers++;

                    // Show the result of this question, which is wrong
                    ShowResult(false);
                }


                //If there is a source and a sound, play it from the source
                if (soundSource && soundWrong) soundSource.GetComponent<AudioSource>().PlayOneShot(soundWrong);
            }
            else // Choosing the correct answer
            {
                // If we answered correctly this round, increase the question count for this bonus group
                questionCount++;

                // Increase the count of the correct answers. This is used to show how many answers we got right at the end of the game
                correctAnswers++;

                // Play the animation Correct
                if (animationCorrect)
                {
                    // If the animation clip doesn't exist in the animation component, add it
                    if (answerObjects[answerIndex].GetComponent<Animation>().GetClip(animationCorrect.name) == null)
                        answerObjects[answerIndex].GetComponent<Animation>()
                            .AddClip(animationCorrect, animationCorrect.name);

                    // Play the animation
                    answerObjects[answerIndex].GetComponent<Animation>().Play(animationCorrect.name);
                }

                // If we have a progress object, color the question tab based on the relevant answer object
                if (progressObject)
                {
                    // Set the color of the tab to green, if it exists
                    if (progressTabObject)
                        progressObject.transform.GetChild(questionLimitCount).GetComponent<Image>().color = Color.green;
                }

                // Animate the bonus being added to the score
                if (bonusObject && bonusObject.GetComponent<Animation>())
                {
                    // Play the animation
                    bonusObject.GetComponent<Animation>().Play();

                    // Reset the speed of the animation
                    bonusObject.GetComponent<Animation>()[bonusObject.GetComponent<Animation>().clip.name].speed = 1;
                }

                // Add the bonus to the score of the current player
                player.scoreCount += bonus;

                // Add the time bonus to the time left, if we have a global timer
                if (globalTime > 0)
                {
                    timeLeft += timeBonus;

                    // If we have go beyond the original global time value, updated the fill bar to accomodate the new value
                    if (timeLeft > globalTime) globalTime = timeLeft;
                }

                //If there is a source and a sound, play it from the source
                if (soundSource && soundCorrect) soundSource.GetComponent<AudioSource>().PlayOneShot(soundCorrect);

                // Show the result of this question, which is correct
                ShowResult(true);
            }
        }
    }

    public void CheckMultiChoice()
    {
        // Hide the check button
        multiChoiceButton.SetActive(false);

        bool goodResult = true;

        for (int answerIndex = 0; answerIndex < questions[currentQuestion].answers.Length; answerIndex++)
        {
            // Disable the button so we can't press it again
            answerObjects[answerIndex].GetComponent<Button>().interactable = false;

            if (questions[currentQuestion].answers[answerIndex].isCorrect ==
                answerObjects[answerIndex].Find("Outline").GetComponent<Image>().enabled)
            {
                // Play the animation Wrong
                if (animationCorrect)
                {
                    // If the animation clip doesn't exist in the animation component, add it
                    if (answerObjects[answerIndex].GetComponent<Animation>().GetClip(animationCorrect.name) == null)
                        answerObjects[answerIndex].GetComponent<Animation>()
                            .AddClip(animationCorrect, animationCorrect.name);

                    // Play the animation
                    answerObjects[answerIndex].GetComponent<Animation>().Play(animationCorrect.name);
                }
            }
            else
            {
                // Play the animation Wrong
                if (animationWrong)
                {
                    // If the animation clip doesn't exist in the animation component, add it
                    if (answerObjects[answerIndex].GetComponent<Animation>().GetClip(animationWrong.name) == null)
                        answerObjects[answerIndex].GetComponent<Animation>()
                            .AddClip(animationWrong, animationWrong.name);

                    // Play the animation
                    answerObjects[answerIndex].GetComponent<Animation>().Play(animationWrong.name);
                }

                goodResult = false;
            }
        }

        if (goodResult == true)
        {
            // If we answered correctly this round, increase the question count for this bonus group
            questionCount++;

            // Increase the count of the correct answers. This is used to show how many answers we got right at the end of the game
            correctAnswers++;

            // If we have a progress object, color the question tab based on the relevant answer object
            if (progressObject)
            {
                // Set the color of the tab to green, if it exists
                if (progressTabObject)
                    progressObject.transform.GetChild(questionLimitCount).GetComponent<Image>().color = Color.green;
            }

            // Animate the bonus being added to the score
            if (bonusObject && bonusObject.GetComponent<Animation>())
            {
                // Play the animation
                bonusObject.GetComponent<Animation>().Play();

                // Reset the speed of the animation
                bonusObject.GetComponent<Animation>()[bonusObject.GetComponent<Animation>().clip.name].speed = 1;
            }

            // Add the bonus to the score of the current player
            player.scoreCount += bonus;

            // Add the time bonus to the time left, if we have a global timer
            if (globalTime > 0)
            {
                timeLeft += timeBonus;

                // If we have go beyond the original global time value, updated the fill bar to accomodate the new value
                if (timeLeft > globalTime) globalTime = timeLeft;
            }

            //If there is a source and a sound, play it from the source
            if (soundSource && soundCorrect) soundSource.GetComponent<AudioSource>().PlayOneShot(soundCorrect);
        }
        else
        {
            // If no answer is selected, select the next available answer button
            if (eventSystem.firstSelectedGameObject == null)
            {
                // Go through the answer buttons and select the first available one
                for (index = 0; index < answerObjects.Length; index++)
                {
                    if (answerObjects[index].GetComponent<Button>().IsInteractable() == true)
                    {
                        if (Application.isMobilePlatform == false && keyboardControls == true)
                            eventSystem.SetSelectedGameObject(answerObjects[index].gameObject);

                        break;
                    }
                }
            }

            // Set the color of the tab to red, if it exists
            if (progressTabObject)
                progressObject.transform.GetChild(questionLimitCount).GetComponent<Image>().color = Color.red;

            // Cut the bonus to half its current value
            bonus *= bonusLoss;

            // Lose some time as a penalty
            timeLeft -= timeLoss;

            // Display the bonus text
            if (bonusObject) bonusObject.Find("Text").GetComponent<Text>().text = bonus.ToString();

            // Increase the mistake count
            mistakeCount++;

            // If we reach the maximum number of mistakes, give no bonus and move on to the next question
            if (mistakeCount >= 1)
            {
                // Give no bonus
                bonus = 0;

                // Display the bonus text
                if (bonusObject) bonusObject.Find("Text").GetComponent<Text>().text = bonus.ToString();

                // Reduce from lives
                player.lives--;

                // Update the lives we have left
                Updatelives();

                // Add to the stat wrong answer
                wrongAnswers++;

                // Show the result of this question, which is wrong
                ShowResult(false);
            }

            //If there is a source and a sound, play it from the source
            if (soundSource && soundWrong) soundSource.GetComponent<AudioSource>().PlayOneShot(soundWrong);
        }

        ShowResult(goodResult);
    }

    /// <summary>
    /// Shows the result of the question, whether we answered correctly or not. Also displays a followup text and reveals a closeup image, if they exist
    /// </summary>
    /// <param name="isCorrectAnswer">We got the correct answer.</param>
    public void ShowResult(bool isCorrectAnswer)
    {
        // We are not asking a question now
        askingQuestion = false;

        // Stop the timer
        timerRunning = false;

        // Reset the mistake counter
        mistakeCount = 0;


        // Disable the button from the question, so that we don't accidentally try to open an image that isn't there
        if (questionObject.GetComponent<Button>()) questionObject.GetComponent<Button>().enabled = false;


        // Go through all the answers and make them unclickable
        for (index = 0; index < answerObjects.Length; index++)
        {
            // If this is the correct answer, highlight it and delay its animation
            if (index < questions[currentQuestion].answers.Length)
            {
                if (questions[currentQuestion].answers[index].isCorrect == true)
                {
                    // Highlight the correct answer
                    if (showCorrectAnswer == true) eventSystem.SetSelectedGameObject(answerObjects[index].gameObject);
                    else answerObjects[index].GetComponent<Button>().interactable = false;
                }
                else
                {
                    // Make all the buttons uninteractable
                    answerObjects[index].GetComponent<Button>().interactable = false;
                }
            }
        }


        // Reset the question and answers in order to display the next question
        StartCoroutine(ResetQuestion(0.5f));
    }

    /// <summary>
    /// Resets the question and answers, in preparation for the next question
    /// </summary>
    /// <returns>The question.</returns>
    /// <param name="delay">Delay in seconds before showing the next question</param>
    IEnumerator ResetQuestion(float delay)
    {
        // Go through all the answers hide the wrong ones
        for (index = 0; index < answerObjects.Length; index++)
        {
            // If this is a wrong answer, hide it. Also if we are not supposed to show the correct answer, hide all the answers
            if (index < questions[currentQuestion].answers.Length &&
                (questions[currentQuestion].answers[index].isCorrect == false || showCorrectAnswer == false))
            {
                // Play the animation Hide, after the current animation is over
                if (animationHide)
                {
                    // If the animation clip doesn't exist in the animation component, add it
                    if (answerObjects[index].GetComponent<Animation>().GetClip(animationHide.name) == null)
                        answerObjects[index].GetComponent<Animation>().AddClip(animationHide, animationHide.name);

                    // Play the animation queded in line after te current animation
                    answerObjects[index].GetComponent<Animation>().PlayQueued(animationHide.name);
                }
            }
        }

        // If we are supposed to show the correct answer, find it and keep it animated longer than the other answers
        if (showCorrectAnswer == true && questions[currentQuestion].multiChoice == false)
        {
            // Go through all the answers again and highlight the correct one
            for (index = 0; index < answerObjects.Length; index++)
            {
                // If this is the correct answer, highlight it and delay its animation
                //if ( answerObjects[index].Find("Text").GetComponent<Text>().text == questions[currentQuestion].correctAnswer )
                if (index < questions[currentQuestion].answers.Length &&
                    questions[currentQuestion].answers[index].isCorrect == true)
                {
                    // Highlight the correct answer
                    eventSystem.SetSelectedGameObject(answerObjects[index].gameObject);

                    // Wait for a while
                    yield return new WaitForSeconds(0.5f);

                    // Play the animation Hide
                    if (animationHide)
                    {
                        // If the animation clip doesn't exist in the animation component, add it
                        if (answerObjects[index].GetComponent<Animation>().GetClip(animationHide.name) == null)
                            answerObjects[index].GetComponent<Animation>().AddClip(animationHide, animationHide.name);

                        // Play the animation
                        answerObjects[index].GetComponent<Animation>().Play(animationHide.name);
                    }
                }
            }
        }

        // For for a while or until the currently playing sound ends
        //if (soundPlayTime > 0 ) yield return new WaitForSeconds(soundPlayTime);
        //else yield return new WaitForSeconds(delay);

        yield return new WaitForSeconds(delay);

        // Stop any sounds playing, and reset the sound play time
        if (soundSource)
        {
            if (isGameOver == false) soundSource.GetComponent<AudioSource>().Stop();

            soundPlayTime = 0;
        }


        // Deselect the currently selected answer
        eventSystem.SetSelectedGameObject(null);

        // Ask the next question
        StartCoroutine(AskQuestion(true));
    }

    /// <summary>
    /// Updates the timer text, and checks if time is up
    /// </summary>
    void UpdateTime()
    {
        // Update the time only if we have a timer object assigned
        if (timerIcon || timerAnimated)
        {
            // Using the timer icon, which uses FillAmount and Text to show the timer we have left
            if (timerIcon)
            {
                // Update the timer circle, if we have one
                if (timerBar)
                {
                    // If we have a global time, display the timer progress for it.
                    if (globalTime > 0)
                    {
                        timerBar.fillAmount = timeLeft / globalTime;
                    }
                    else if
                        (timerRunning ==
                         true) // If the timer is running, display the fill amount left for the question time. 
                    {
                        timerBar.fillAmount = timeLeft / questions[currentQuestion].time;
                    }
                    else // Otherwise refill the amount back to 100%
                    {
                        timerBar.fillAmount = Mathf.Lerp(timerBar.fillAmount, 1, Time.deltaTime * 10);
                    }
                }

                // Update the timer text, if we have one
                if (timerText)
                {
                    // If the timer is running, display the timer left. Otherwise hide the text
                    if (timerRunning == true || globalTime > 0) timerText.text = Mathf.RoundToInt(timeLeft).ToString();
                    else timerText.text = "";
                }
            }

            // Using the animated timer, which progresses the animation based on the timer we have left
            if (timerAnimated && timerAnimated.isPlaying == false)
            {
                // Start the timer animation
                timerAnimated.Play("TimerAnimatedProgress");

                // If we have a global time, display the correct frame from the time animation
                if (globalTime > 0)
                {
                    timerAnimated["TimerAnimatedProgress"].time = (1 - (timeLeft / globalTime)) *
                                                                  timerAnimated["TimerAnimatedProgress"].clip.length;
                }
                else if
                    (timerRunning == true) // If the timer is running, display the correct frame from the time animation
                {
                    timerAnimated["TimerAnimatedProgress"].time = (1 - (timeLeft / questions[currentQuestion].time)) *
                                                                  timerAnimated["TimerAnimatedProgress"].clip.length;
                }
                else // Otherwise rewind the time animation to the start
                {
                    timerAnimated["TimerAnimatedProgress"].time =
                        Mathf.Lerp(timerAnimated["TimerAnimatedProgress"].time, 1, Time.deltaTime * 10);
                }

                // Start animating
                timerAnimated["TimerAnimatedProgress"].enabled = true;

                // Record the current frame
                timerAnimated.Sample();

                // Stop animating
                timerAnimated["TimerAnimatedProgress"].enabled = false;
            }

            // Time's up!
            if (timeLeft <= 0 && timerRunning == true)
            {
                // If we have a global time and the timer ran out, just go straight to the GameOver screen
                if (globalTime > 0)
                {
                    StartCoroutine(GameOver(1));
                }
                else
                {
                    // Reduce from lives
                    player.lives--;

                    // Update the lives we have left
                    Updatelives();

                    // Show the result of this question, which is wrong ( because we ran out of time, we lost the question )
                    ShowResult(false);
                }

                // Play the timer icon animation
                if (timerIcon && timerIcon.GetComponent<Animation>()) timerIcon.GetComponent<Animation>().Play();

                // Play the animated timer's timeUp animation
                if (timerAnimated && timerAnimated.GetComponent<Animation>())
                {
                    timerAnimated.Stop();
                    timerAnimated.Play("TimerAnimatedTimeUp");
                }

                //If there is a source and a sound, play it from the source
                if (soundSource && soundTimeUp) soundSource.GetComponent<AudioSource>().PlayOneShot(soundTimeUp);
            }
        }
    }

    /// <summary>
    /// Updates the score value and checks if we got to the next level
    /// </summary>
    void UpdateScore()
    {
        //Update the score text
        //if ( scoreText )    scoreText.GetComponent<Text>().text = score.ToString();

        //Update the score text for the current player
        if (player.scoreText)
            player.scoreText.GetComponent<Text>().text = player.score.ToString();

        // If we reach the victory score we win the game
        if (scoreToVictory > 0 && player.score >= scoreToVictory) StartCoroutine(Victory(0));
    }

    /// <summary>
    /// Runs the game over event and shows the game over screen
    /// </summary>
    IEnumerator GameOver(float delay)
    {
        isGameOver = true;

        // Calculate the quiz duration
        playTime = DateTime.Now - startTime;

        yield return new WaitForSeconds(delay);

        //Show the game over screen
        if (gameOverCanvas)
        {
            //Show the game over screen
            gameOverCanvas.gameObject.SetActive(true);

            //Write the score text, if it exists
            if (gameOverCanvas.Find("ScoreTexts/TextScore"))
                gameOverCanvas.Find("ScoreTexts/TextScore").GetComponent<Text>().text +=
                    " " + player.score.ToString();

            //Check if we got a high score
            if (player.score > highScore)
            {
                highScore = player.score;

                //Register the new high score
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
                PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name + "HighScore", player.score);
#else
                    PlayerPrefs.SetFloat(Application.loadedLevelName + "HighScore", players[currentPlayer].score);
#endif
            }

            //Write the high sscore text
            gameOverCanvas.Find("ScoreTexts/TextHighScore").GetComponent<Text>().text += " " + highScore.ToString();

            //If there is a source and a sound, play it from the source
            if (soundSource && soundGameOver) soundSource.GetComponent<AudioSource>().PlayOneShot(soundGameOver);
        }
    }

    /// <summary>
    /// Runs the victory event and shows the victory screen
    /// </summary>
    IEnumerator Victory(float delay)
    {
        // If this quiz has no questions at all, just return to the main menu
        if (questions.Length <= 0) yield break;

        isGameOver = true;

        // Calculate the quiz duration
        playTime = DateTime.Now - startTime;

        // Record the state of the category as completed
        if (currentCategory != null)
        {
            PlayerPrefs.SetInt(currentCategory + "Completed", 1);

            currentCategory = null;
        }

        yield return new WaitForSeconds(delay);

        //Show the game over screen
        if (victoryCanvas)
        {
            //Show the victory screen
            victoryCanvas.gameObject.SetActive(true);

            // If we have a TextScore and TextHighScore objects, then we are using the single player victory canvas
            if (victoryCanvas.Find("ScoreTexts/TextScore") && victoryCanvas.Find("ScoreTexts/TextHighScore"))
            {
                //Write the score text, if it exists
                victoryCanvas.Find("ScoreTexts/TextScore").GetComponent<Text>().text +=
                    " " + player.score.ToString();

                //Check if we got a high score
                if (player.score > highScore)
                {
                    highScore = player.score;

                    //Register the new high score
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
                    PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name + "HighScore",
                        player.score);
#else
                        PlayerPrefs.SetFloat(Application.loadedLevelName + "HighScore", players[currentPlayer].score);
#endif
                }

                //Write the high sscore text
                victoryCanvas.Find("ScoreTexts/TextHighScore").GetComponent<Text>().text += " " + highScore.ToString();
            }

            // If we have a TextProgress object, then we can display how many questions we answered correctly
            if (victoryCanvas.Find("ScoreTexts/TextProgress"))
            {
                //Write the progress text
                victoryCanvas.Find("ScoreTexts/TextProgress").GetComponent<Text>().text =
                    correctAnswers.ToString() + "/" + questionLimit.ToString();
            }

            //If there is a source and a sound, play it from the source
            if (soundSource && soundVictory) soundSource.GetComponent<AudioSource>().PlayOneShot(soundVictory);
        }
    }

    /// <summary>
    /// Restart the current level
    /// </summary>
    public void Restart()
    {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
#else
            Application.LoadLevel(Application.loadedLevelName);
#endif
    }

    /// <summary>
    /// Restart the current level
    /// </summary>
    public void MainMenu()
    {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
        SceneManager.LoadScene(mainMenuLevelName);
#else
            Application.LoadLevel(mainMenuLevelName);
#endif
    }

    /// <summary>
    /// Updates the lives we have
    /// </summary>
    public void Updatelives()
    {
        // Update lives only if we have a lives bar assigned
        if (player.livesBar)
        {
            // If we run out of lives, it's game over
            if (player.lives <= 0) StartCoroutine(GameOver(1));
        }
    }

    /// <summary>
    /// Sets the questions list from an external question list. This is used when getting the questions from a category selector.
    /// </summary>
    /// <param name="setValue">The list of questions we got</param>
    public void SetQuestions(Pregunta[] setValue)
    {
        questions = setValue;
    }

    /// <summary>
    /// Sets the name of the category from an external category grid. This is used when getting the category name from a category grid
    /// </summary>
    /// <param name="setValue">Set value.</param>
    public void SetCategoryName(string setValue)
    {
        currentCategory = setValue;
    }

    /// <summary>
    /// Skips the current question and displays the next one
    /// </summary>
    public void SkipQuestion()
    {
        // Stop listening for a click on the button to move to the next question
        if (questionObject.GetComponent<Button>()) questionObject.GetComponent<Button>().onClick.RemoveAllListeners();

        // Reset the question and answers in order to display the next question
        StartCoroutine(ResetQuestion(0.5f));
    }

    /// <summary>
    /// Gives the player an extra life once per match. This can be used with UnityAds to reward the player when watching an ad
    /// </summary>
    public void ExtraLife()
    {
        // Add to lives
        player.lives++;

        // Update the lives we have left
        Updatelives();

        // The game is not over anymore
        isGameOver = false;

        // Reset the score texts and hide the gameover object
        if (gameOverCanvas)
        {
            gameOverCanvas.Find("ScoreTexts/TextScore").GetComponent<Text>().text = gameOverCanvas
                .Find("ScoreTexts/TextScore").GetComponent<Text>().text.Substring(0,
                    gameOverCanvas.Find("ScoreTexts/TextScore").GetComponent<Text>().text.Length -
                    player.score.ToString().Length);

            gameOverCanvas.Find("ScoreTexts/TextHighScore").GetComponent<Text>().text = gameOverCanvas
                .Find("ScoreTexts/TextHighScore").GetComponent<Text>().text.Substring(0,
                    gameOverCanvas.Find("ScoreTexts/TextHighScore").GetComponent<Text>().text.Length -
                    highScore.ToString().Length);

            gameOverCanvas.gameObject.SetActive(false);
        }

        // Ask the next question
        StartCoroutine(AskQuestion(false));
    }
}