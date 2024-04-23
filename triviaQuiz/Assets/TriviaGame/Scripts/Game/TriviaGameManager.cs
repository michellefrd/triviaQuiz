using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TigerForge.UniDB;

/// <summary>
/// Este script controla el juego, lo inicia, sigue el progreso del mismo y finaliza el juego.
/// </summary>
public class TriviaGameManager : MonoBehaviour
{
    // Categoría de las preguntas a cargar
    private int selectedCategoryID;

    // Conexión a la base de datos
    private UniDB.Trivia triviaDB;

    // Holds the current event system
    internal EventSystem eventSystem;

    [Header("<Opciones del jugador>")]
    [Tooltip(
        "Una lista de los jugadores en el juego. A cada jugador se le puede asignar un nombre, un texto de puntuación, vidas y una barra de vidas. Debes tener al menos " +
        "un jugador en la lista para poder jugar. No es necesario asignar todos los campos. Por ejemplo, un jugador puede tener un nombre sin " +
        "live bar y funcionará bien")]
    public Jugador player;

    [Tooltip(
        "El número de vidas que tiene cada jugador. Pierdes una vida si se acaba el tiempo o respondes mal demasiadas veces.")]
    public float lives = 3;

    // El ancho de una sola vida en la barra de vidas. Esto se calcula a partir del ancho total de una barra de vida dividido por el número de vidas.
    internal float livesBarWidth = 128;

    internal RectTransform playersObject;

    [Header("<Opciones de preguntas>")] [Tooltip("El objeto que muestra la pregunta actual.")]
    public Transform questionObject;

    [Tooltip(
        "Una lista de todas las preguntas posibles del juego. Cada pregunta tiene una cantidad de respuestas correctas/incorrectas,un valor extra y tiempo")]
    public Pregunta[] questions;

    internal Pregunta[] questionsTemp;

    [Tooltip(
        "El número de la primera pregunta que se hace. Puede cambiar esto para comenzar desde un número de pregunta más alto.")]
    public int firstQuestion = -1;


    // El índice de la pregunta actual que se hace. -1 es el índice de la primera pregunta, 0 el índice de la segunda, y así sucesivamente
    internal int currentQuestion = -1;


    // ¿Se está haciendo alguna pregunta ahora mismo?
    internal bool askingQuestion;

    [Tooltip(
        "Aleatoriza la lista de preguntas. Utilice esto si no desea que las preguntas aparezcan en el mismo orden cada " +
        "vez que juegas.")]
    public bool randomizeQuestions = true;

    //Los botones que muestran las posibles respuestas
    internal Transform[] answerObjects;

    [Tooltip("Aleatorizar el orden de visualización de las respuestas cuando se presenta una nueva pregunta")]
    public bool randomizeAnswers = true;


    // La respuesta actualmente seleccionada cuando se usa ButtonSelector
    internal int currentAnswer = 0;
    internal GameObject multiChoiceButton;


    // Mantiene las respuestas al barajarlas
    internal string[] tempAnswers;

    internal int questionCount = 0;

    // El objeto de progreso que muestra todas las pestañas de progreso y cuántas preguntas quedan para ganar
    internal RectTransform progressObject;

    [Tooltip(
        "Limitar el número total de preguntas formuladas, independientemente de si respondimos correctamente o no." +
        "Utiliza esto si quieres que se haga un número estricto de preguntas en el juego (por ejemplo, 10 preguntas)" +
        "Si lo mantienes en 0 el número de preguntas no estará limitado y pasarás por todas las preguntas del cuestionario antes de terminarlo")]
    public int questionLimit = 0;

    // El número total de preguntas que hicimos. Esto se utiliza para comprobar si alcanzamos el límite de preguntas
    internal int questionLimitCount = 0;

    // El número de preguntas que respondimos correctamente. Esto se utiliza para mostrar el resultado al final del juego.
    internal int correctAnswers = 0;
    internal float wrongAnswers = 0;

    [Tooltip(
        "El número máximo de errores permitidos. Si cometes muchos errores, perderás una vida y pasarás a la siguiente pregunta")]
    public int maximumMistakes = 2;

    internal int mistakeCount = 0;

    // ¿Cuántos segundos quedan antes de que termine el juego?
    internal float timeLeft = 10;


    // ¿Está corriendo el cronómetro?
    internal bool timerRunning = false;

    [Tooltip(
        "Si configuramos este tiempo por encima de 0, anulará los tiempos individuales para cada pregunta. La hora global no se pone a cero entre preguntas")]
    public float globalTime = 0;

    [Tooltip("¿Cuántos segundos perdemos del cronómetro cuando cometemos un error?")]
    public float timeLoss = 0;

    [Tooltip("¿Cuantos segundos sumamos al cronómetro al responder correctamente?")]
    public float timeBonus = 0;

    // El bono que tenemos actualmente
    internal float bonus;

    [Tooltip(
        "El porcentaje que perdemos de nuestro bono potencial si respondemos mal a una pregunta." +
        "Por ejemplo 0,5 nos hace perder la mitad del bono si respondemos mal una vez, y ¾ del bono si respondemos mal dos veces.")]
    public float bonusLoss = 0.5f;


    // La puntuación más alta registrada
    internal float highScore = 0;

    [Tooltip(
        "Puede establecer un límite de puntuación; si lo alcanza, ganará la prueba. Si no quieres un límite de puntuación, mantenlo en 0.")]
    public float scoreToVictory = 0;

    [Tooltip("Resalta la/s respuesta/s correcta/s al mostrar el resultado")]
    public bool showCorrectAnswer = true;

    [Tooltip(
        "Establezca el mismo tiempo de preguntas para todas las preguntas del cuestionario. " +
        "Esto sobrescribe los tiempos individuales establecidos en cada pregunta" +
        "Mantén esto en 0 si no quieres sobrescribir ningún valor")]
    public float quizTime = 0;

    [Tooltip(
        "Establezca la misma bonificación de pregunta para todas las preguntas del cuestionario" +
        "Esto sobrescribe las bonificaciones individuales establecidas en cada pregunta. Mantenlo en 0 si no deseas sobrescribir ningún valor.")]
    public float quizBonus = 0;

    [Header("<Opciones de UI>")]
    [Tooltip("El objeto de bonificación que muestra cuánto podemos ganar si respondemos correctamente")]
    public Transform bonusObject;

    //El canvas del cronómetro del juego.
    internal GameObject timerIcon;
    internal Image timerBar;
    internal Text timerText;

    // Este es un temporizador especial basado en animación. En lugar de llenar una barra, calcula la animación del temporizador.
    internal Animation timerAnimated;

    [Tooltip("El menú que aparece si perdemos todas las vidas en una partida para un solo jugador")]
    public Transform gameOverCanvas;

    [Tooltip("El menú que aparece después de terminar todas las preguntas del juego. Utilizado para un jugador.")]
    public Transform victoryCanvas;

    // ¿Se acabó el juego?
    internal bool isGameOver = false;

    [Tooltip("El nivel del menú principal que se puede cargar una vez finalizado el juego.")]
    public string mainMenuLevelName = "MenuScene";

    [Header("<Animaciones y Sonidos>")] [Tooltip("La animación que se reproduce al mostrar una respuesta.")]
    public AnimationClip animationShow;

    [Tooltip("La animación que se reproduce al ocultar una respuesta.")]
    public AnimationClip animationHide;

    [Tooltip("La animación que se reproduce al elegir la respuesta correcta.")]
    public AnimationClip animationCorrect;

    [Tooltip("La animación que se reproduce al elegir la respuesta incorrecta")]
    public AnimationClip animationWrong;

    [Tooltip("La animación que se reproduce al mostrar una nueva pregunta.")]
    public AnimationClip animationQuestion;

    [Tooltip("Varios sonidos y su fuente.")]
    public AudioClip soundQuestion;

    public AudioClip soundCorrect;
    public AudioClip soundWrong;
    public AudioClip soundTimeUp;
    public AudioClip soundGameOver;
    public AudioClip soundVictory;
    public string soundSourceTag = "Sound";
    internal GameObject soundSource;

    // Esto cuenta el tiempo que el sonido actual se reproduce ahora, para que no reproduzcamos otro sonido
    internal float soundPlayTime = 0;
    internal bool isPaused = false;

    // Indices de uso general
    internal int index = 0;
    internal int indexB = 0;

    internal bool keyboardControls = false;

    // Esta estadística realiza un seguimiento del tiempo si se toma desde el inicio de la prueba hasta el final de la prueba (final del juego o victoria)
    internal DateTime startTime;
    internal TimeSpan playTime;

    /// <summary>
    /// Start solo se llama una vez durante la vida del behaviour.
    /// La diferencia entre Awake y Start es que Start solo se llama si la instancia del script está habilitada.
    /// Esto le permite retrasar cualquier código de inicialización hasta que sea realmente necesario.
    /// Awake siempre se llama antes de cualquier función de inicio.
    /// Esto le permite ordenar la inicialización de scripts
    /// </summary>
    void Start()
    {
        //Obtiene el email del jugador
        player.email = PlayerPrefs.GetString("Email");

        // Obtiene el ID de la categoría seleccionada desde PlayerPrefs
        selectedCategoryID = PlayerPrefs.GetInt("SelectedCategoryID", 0);

        Debug.Log("Categoría seleccionada: " + selectedCategoryID);

        // DatabaseConnection
        triviaDB = new UniDB.Trivia();

        //Cargar las preguntas de la categoría seleccionada
        LoadQuestionsFromDatabase(selectedCategoryID);

        // Desactiva el multitouch para que no toquemos dos respuestas al mismo tiempo (evita las trampas de respuestas múltiples)
        Input.multiTouchEnabled = false;

        // Almacenar en caché el sistema de eventos actual para que podamos habilitarlo y deshabilitarlo entre preguntas
        eventSystem = UnityEngine.EventSystems.EventSystem.current;

        // Si el cuestionario se ejecuta en una plataforma móvil (iOS, Android, etc.), deshabilite el módulo de entrada independiente para que el gamepad no pueda tomar el control.
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            eventSystem.GetComponent<StandaloneInputModule>().enabled = false;
        }


        // Ocultar el final del juego, la victoria y las pantallas de imágenes más grandes
        if (gameOverCanvas) gameOverCanvas.gameObject.SetActive(false);
        if (victoryCanvas) victoryCanvas.gameObject.SetActive(false);

        // Consigue la puntuación más alta para el jugador
        highScore = PlayerPrefs.GetFloat("HighScore", 0);
        Debug.Log("Player high score: " + highScore);


        //Asigne el icono del temporizador y el texto para un acceso más rápido
        if (GameObject.Find("TimerIcon"))
        {
            timerIcon = GameObject.Find("TimerIcon");
            if (GameObject.Find("TimerIcon/Bar")) timerBar = GameObject.Find("TimerIcon/Bar").GetComponent<Image>();
            if (GameObject.Find("TimerIcon/Text")) timerText = GameObject.Find("TimerIcon/Text").GetComponent<Text>();
        }

        //Asigna el objeto de los jugadores para un acceso más rápido a los nombres, puntuaciones y vidas de los jugadores
        if (GameObject.Find("PlayersObject"))
        {
            playersObject = GameObject.Find("PlayersObject").GetComponent<RectTransform>();
        }

        //Asignar la fuente de sonido para un acceso más fácil
        if (GameObject.FindGameObjectWithTag(soundSourceTag))
            soundSource = GameObject.FindGameObjectWithTag(soundSourceTag);

        // Borrar el texto del objeto de bonificación
        if (bonusObject) bonusObject.Find("Text").GetComponent<Text>().text = "";

        // Borrar el texto del objeto de pregunta
        questionObject.Find("Text").GetComponent<Text>().text = "";

        // Ocultar el botón CONTINUAR que pasa a la siguiente pregunta
        if (questionObject.Find("ButtonContinue")) questionObject.Find("ButtonContinue").gameObject.SetActive(false);

        // Ocultar botón de confirmación de opción múltiple, establecer referencia y escucha de botón
        if (transform.Find("MultiChoiceButton"))
        {
            multiChoiceButton = transform.Find("MultiChoiceButton").gameObject;
            multiChoiceButton.SetActive(false);
            multiChoiceButton.GetComponent<Button>().onClick.AddListener(delegate() { CheckMultiChoice(); });
        }


        // Busque el objeto "Respuesta" que contiene todos los botones de respuesta
        // y asigne todos los botones de respuesta a una matriz para facilitar el acceso
        if (GameObject.Find("Answers"))
        {
            //Definimos el contenedor de respuestas el cual llenaremos con los botones de respuesta
            Transform answersHolder = GameObject.Find("Answers").transform;


            // Prepara la lista de botones de respuesta para que podamos completarla
            answerObjects = new Transform[answersHolder.childCount];


            //Asigna los botones de respuesta
            for (index = 0; index < answerObjects.Length; index++) answerObjects[index] = answersHolder.GetChild(index);

            // Escuche un clic en cada botón para elegir la respuesta
            foreach (Transform answerObject in answerObjects)
            {
                // Necesitamos esta variable temporal para poder asignar detectores de eventos a múltiples objetos
                Transform tempAnswerObject = answerObject;

                // Escuche un clic para elegir la respuesta
                tempAnswerObject.GetComponent<Button>().onClick.AddListener(delegate()
                {
                    ChooseAnswer(tempAnswerObject);
                });
            }
        }

        // Borrar todas las respuestas
        foreach (Transform answerObject in answerObjects)
        {
            // Borrar el texto de la respuesta
            answerObject.Find("Text").GetComponent<Text>().text = "";


            // Ocultar esquema de respuesta
            if (answerObject.Find("Outline")) answerObject.Find("Outline").GetComponent<Image>().enabled = false;

            // Desactivar el objeto de respuesta
            answerObject.gameObject.SetActive(false);
        }

        //Actualizar score del jugador
        UpdateScore();
    }

    /// <summary>
    /// Carga las preguntas de la categoria desde la base de datos
    /// </summary>
    /// <param name="selectedCategoryID">Número de ID de la categoría a cargar</param>
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
                    StartGame();
                }
                else
                {
                    Debug.LogError("Error al cargar preguntas: " + info.error);
                }
            });
    }

    /// <summary>
    /// Prepara la lista de preguntas e inicia el juego.
    /// </summary>
    public void StartGame()
    {
        isGameOver = false; // Indica que el juego aún no ha terminado.

        // Registra la hora de inicio del juego para poder verificar cuánto tiempo tomó el cuestionario al final del juego.
        startTime = DateTime.Now;

        // El índice de la primera pregunta en la lista de preguntas es en realidad -1, así que ajustamos el número desde el componente (1 se convierte en -1, 2 en 0, 10 en 8, etc.)
        currentQuestion = firstQuestion;

        // Restablece el contador de preguntas.
        questionCount = 0;

        // Asegúrate de que el límite de preguntas no sea mayor que el número real de preguntas disponibles.
        questionLimit = Mathf.Clamp(questionLimit, 0, questions.Length);

        // Recorre todas las preguntas y sobrescribe sus valores de tiempo con los valores generales del cuestionario, si existen.
        if (quizTime > 0)
            foreach (Pregunta question in questions)
                question.time = quizTime;

        // Recorre todas las preguntas y sobrescribe sus valores de bonificación con los valores generales del cuestionario, si existen.
        if (quizBonus > 0)
            foreach (Pregunta question in questions)
                question.bonus = quizBonus;

        // Establece la lista de preguntas para este partido.
        SetQuestionList();

        // Hacer la primera pregunta.
        StartCoroutine(AskQuestion(false));
    }


    /// <summary>
    /// Update se llama cada frame, si MonoBehaviour está habilitado.
    /// </summary>
    void Update()
    {
        // Hacer que la puntuación aumente hasta su valor actual, para el jugador actual
        if (player.score < player.scoreCount)
        {
            // Aumentar hasta el valor actual
            player.score = Mathf.Lerp(player.score, player.scoreCount, Time.deltaTime * 10);

            // Redondear el valor de la puntuación hacia arriba
            player.score = Mathf.CeilToInt(player.score);

            // Actualizar el texto de la puntuación
            UpdateScore();
        }

        // Actualizar la barra de vidas

        // Si la barra de vidas tiene un texto en ella, actualizarlo. De lo contrario, cambiar el tamaño de la barra de vidas basándose en el número de vidas restantes
        if (player.livesBar.transform.Find("Text"))
            player.livesBar.transform.Find("Text").GetComponent<Text>().text = player.lives.ToString();
        else
            player.livesBar.rectTransform.sizeDelta = Vector2.Lerp(
                player.livesBar.rectTransform.sizeDelta,
                new Vector2(player.lives * livesBarWidth,
                    player.livesBar.rectTransform.sizeDelta.y), Time.deltaTime * 8);

        if (isGameOver == false)
        {
            // Si usamos el teclado o gamepad, los controles de teclado toman efecto
            if (keyboardControls == false && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
            {
                keyboardControls = true;

                // Si no hay respuesta seleccionada, seleccionar el siguiente botón de respuesta disponible
                if (askingQuestion == true && eventSystem.firstSelectedGameObject == null)
                {
                    // Recorrer los botones de respuesta y seleccionar el primero disponible
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

            // Si movemos el ratón en cualquier dirección o hacemos clic, o tocamos la pantalla en un dispositivo móvil, entonces se pierden los controles de teclado/gamepad
            if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0 || Input.GetMouseButtonDown(0) ||
                Input.touchCount > 0) keyboardControls = false;

            // Contar el tiempo hasta el fin del juego
            if (timeLeft > 0 && timerRunning == true)
            {
                // Contar hacia atrás el tiempo
                timeLeft -= Time.deltaTime;
            }

            // Actualizar el temporizador
            UpdateTime();
        }

        // Actualizar el tiempo de reproducción de sonido
        if (soundPlayTime > 0) soundPlayTime -= Time.deltaTime;
    }


    /// <summary>
    /// Establece la lista de preguntas, primero barajándolas, luego ordenándolas por valor de bonificación,
    /// y finalmente eligiendo un número limitado de preguntas de cada grupo de bonificación.
    /// </summary>
    void SetQuestionList()
    {
        // Baraja todas las preguntas disponibles
        if (randomizeQuestions == true) questions = ShuffleQuestions(questions);
    }

    /// <summary>
    /// Baraja la lista de preguntas especificada y la devuelve.
    /// </summary>
    /// <param name="questions">Una lista de preguntas</param>
    Pregunta[] ShuffleQuestions(Pregunta[] questions)
    {
        // Recorre todas las preguntas y las baraja
        for (index = 0; index < questions.Length; index++)
        {
            // Mantiene la pregunta en una variable temporal
            Pregunta tempQuestion = questions[index];

            // Elige un índice aleatorio de la lista de preguntas
            int randomIndex = UnityEngine.Random.Range(index, questions.Length);

            // Asigna una pregunta aleatoria de la lista
            questions[index] = questions[randomIndex];

            // Asigna la pregunta temporal a la pregunta aleatoria que elegimos
            questions[randomIndex] = tempQuestion;
        }

        return questions;
    }

    /// <summary>
    /// Baraja la lista de respuestas especificada y la devuelve.
    /// </summary>
    /// <param name="answers">Una lista de respuestas</param>
    Respuesta[] ShuffleAnswers(Respuesta[] answers)
    {
        // Recorre todas las respuestas y las baraja
        for (index = 0; index < answers.Length; index++)
        {
            // Mantiene la respuesta en una variable temporal
            Respuesta tempAnswer = answers[index];

            // Elige un índice aleatorio de la lista de respuestas
            int randomIndex = UnityEngine.Random.Range(index, answers.Length);

            // Asigna una respuesta aleatoria de la lista
            answers[index] = answers[randomIndex];

            // Asigna la respuesta temporal a la respuesta aleatoria que elegimos
            answers[randomIndex] = tempAnswer;
        }

        return answers;
    }

    /// <summary>
    /// Presenta una pregunta de la lista, junto con las posibles respuestas.
    /// </summary>
    IEnumerator AskQuestion(bool animateQuestion)
    {
        if (isGameOver == false)
        {
            // Estamos haciendo una pregunta ahora
            askingQuestion = true;

            // Pasar a la siguiente pregunta
            currentQuestion++;

            if (currentQuestion >= questions.Length)
            {
                Debug.Log(currentQuestion);
                Debug.Log(questions.Length);
                // Mostrar un texto indicando que estamos reiniciando la lista de preguntas
                questionObject.Find("Text").GetComponent<Text>().text =
                    "Se han realizado todas las preguntas del cuestionario.";
                // En lugar de reiniciar las preguntas, terminar el juego aquí
                Debug.Log("Todas las preguntas han sido realizadas. Terminando el juego.");
                yield return new WaitForSeconds(2.5f);
                StartCoroutine(Victory(0)); // Activar la secuencia de victoria o fin del juego
                yield break; // Salir temprano de la coroutine
            }

            // Animar la pregunta
            if (animateQuestion)
            {
                // Si el clip de animación no existe en el componente de animación, añadirlo
                if (questionObject.GetComponent<Animation>().GetClip(animationQuestion.name) == null)
                    questionObject.GetComponent<Animation>().AddClip(animationQuestion, animationQuestion.name);

                // Reproducir la animación
                questionObject.GetComponent<Animation>().Play(animationQuestion.name);

                // Esperar la mitad del tiempo de la animación, luego mostrar la siguiente pregunta. Esto hace que la pregunta aparezca mientras la pestaña de la pregunta se voltea. Solo un efecto agradable
                yield return new WaitForSeconds(questionObject.GetComponent<Animation>().clip.length * 0.5f);
            }

            // Si todavía tenemos preguntas en la lista, hacer la siguiente pregunta
            if (currentQuestion < questions.Length)
            {
                // Mostrar la pregunta actual
                questionObject.Find("Text").GetComponent<Text>().text = questions[currentQuestion].question;

                // si la pregunta es de elección múltiple, mostrar el botón de verificación
                if (multiChoiceButton) multiChoiceButton.SetActive(questions[currentQuestion].multiChoice);

                // Establecer el tiempo para esta pregunta, a menos que tengamos un temporizador global, en cuyo caso ignorar el tiempo local de la pregunta
                if (globalTime <= 0) timeLeft = questions[currentQuestion].time;

                // Iniciar el temporizador
                timerRunning = true;

                // Limpiar todas las respuestas
                foreach (Transform answerObject in answerObjects)
                {
                    answerObject.Find("Text").GetComponent<Text>().text = "";

                    // Ocultar el contorno de la respuesta
                    if (answerObject.Find("Outline"))
                        answerObject.Find("Outline").GetComponent<Image>().enabled = false;

                    // Si la respuesta tiene una imagen, borrarla y ocultarla
                    if (answerObject.Find("Image"))
                    {
                        answerObject.Find("Image").GetComponent<Image>().sprite = null;
                        answerObject.Find("Image").gameObject.SetActive(false);
                    }

                    // Si la respuesta tiene un video, ocultarlo
                    if (answerObject.Find("Video")) answerObject.Find("Video").gameObject.SetActive(false);
                }

                // Barajar la lista de respuestas
                if (randomizeAnswers == true)
                    questions[currentQuestion].answers = ShuffleAnswers(questions[currentQuestion].answers);

                // Mostrar las respuestas incorrectas y correctas en los espacios de respuesta
                for (index = 0; index < questions[currentQuestion].answers.Length; index++)
                {
                    // Si el objeto de respuesta está inactivo, activarlo
                    if (!answerObjects[index].gameObject.activeSelf)
                        answerObjects[index].gameObject.SetActive(true);

                    // Reproducir la animación Show
                    if (animationShow)
                    {
                        // Si el clip de animación no existe en el componente de animación, añadirlo
                        if (answerObjects[index].GetComponent<Animation>().GetClip(animationShow.name) == null)
                            answerObjects[index].GetComponent<Animation>().AddClip(animationShow, animationShow.name);

                        // Reproducir la animación
                        answerObjects[index].GetComponent<Animation>().Play(animationShow.name);
                    }

                    // Habilitar el botón para que podamos presionarlo
                    answerObjects[index].GetComponent<Button>().interactable = true;

                    // Seleccionar cada botón a medida que se habilita. Esta acción resuelve un bug que apareció en Unity 5.5 donde los botones permanecen resaltados de la pregunta anterior.
                    answerObjects[index].GetComponent<Button>().Select();

                    // Mostrar el texto de la respuesta
                    if (index < questions[currentQuestion].answers.Length)
                        answerObjects[index].Find("Text").GetComponent<Text>().text =
                            questions[currentQuestion].answers[index].answer;
                    else
                        answerObjects[index].gameObject.SetActive(false);
                }

                // Si comenzamos un nuevo grupo de bonificación, restablecer el contador de preguntas
                if (bonus > questions[currentQuestion].bonus) questionCount = 0;

                // Establecer la bonificación que podemos obtener por esta pregunta
                bonus = questions[currentQuestion].bonus;

                if (bonusObject && bonusObject.GetComponent<Animation>())
                {
                    // Animar el objeto de bonificación
                    bonusObject.GetComponent<Animation>().Play();

                    // Restablecer la animación de bonificación
                    bonusObject.GetComponent<Animation>()[bonusObject.GetComponent<Animation>().clip.name].speed =
                        -1;

                    // Mostrar el texto de bonificación
                    bonusObject.Find("Text").GetComponent<Text>().text = bonus.ToString();
                }

                // Si los controles de teclado están activados, resaltar la primera respuesta. De lo contrario, deseleccionar todas las respuestas
                if (keyboardControls == true) eventSystem.SetSelectedGameObject(answerObjects[0].gameObject);
                else eventSystem.SetSelectedGameObject(null);

                // Si hay una fuente y un sonido, reproducirlo desde la fuente
                if (soundSource && soundQuestion)
                    soundSource.GetComponent<AudioSource>().PlayOneShot(soundQuestion);
            }
            else // Si no tenemos más preguntas en la lista, ganar el juego
            {
                // Desactivar el objeto de pregunta
                // questionObject.gameObject.SetActive(false);

                // ¡Si no tenemos más preguntas, ganamos el juego!
                StartCoroutine(Victory(0));
            }

            // Si tenemos un límite de preguntas, contar hacia él para ganar
            if (isGameOver == false && questionLimit > 0)
            {
                questionLimitCount++;

                Debug.Log("REACHING");
                // Si alcanzamos el límite de preguntas, ganar el juego
                if (questionLimitCount > questionLimit) StartCoroutine(Victory(0));
            }

            else
            {
                // Hacer la siguiente pregunta
                StartCoroutine(AskQuestion(true));
            }
        }
    }

    /// <summary>
    /// Elige una respuesta de la lista por índice.
    /// </summary>
    /// <param name="answerSource">La fuente de la respuesta que elegimos</param>
    public void ChooseAnswer(Transform answerSource)
    {
        // Obtener el índice de este objeto de respuesta
        int answerIndex = answerSource.GetSiblingIndex();

        // Solo podemos elegir una respuesta si actualmente se está haciendo una pregunta
        if (askingQuestion == true)
        {
            // Si esta es una pregunta de elección múltiple, permitir al jugador elegir más de una respuesta antes de verificar el resultado
            if (questions[currentQuestion].multiChoice == true)
            {
                if (answerObjects[answerIndex].Find("Outline"))
                    answerObjects[answerIndex].Find("Outline").GetComponent<Image>().enabled =
                        !answerObjects[answerIndex].Find("Outline").GetComponent<Image>().enabled;

                return;
            }

            // Si la respuesta elegida es incorrecta, deshabilitarla y reducir el bono para esta pregunta
            if (!questions[currentQuestion].answers[answerIndex].isCorrect)
            {
                // Reproducir la animación de respuesta incorrecta
                if (animationWrong)
                {
                    // Si el clip de animación no existe en el componente de animación, añadirlo
                    if (answerObjects[answerIndex].GetComponent<Animation>().GetClip(animationWrong.name) == null)
                        answerObjects[answerIndex].GetComponent<Animation>()
                            .AddClip(animationWrong, animationWrong.name);

                    // Reproducir la animación
                    answerObjects[answerIndex].GetComponent<Animation>().Play(animationWrong.name);
                }

                // Deshabilitar el botón para que no podamos presionarlo de nuevo
                answerObjects[answerIndex].GetComponent<Button>().interactable = false;

                // Si no hay respuesta seleccionada, seleccionar el siguiente botón de respuesta disponible
                if (eventSystem.firstSelectedGameObject == null)
                {
                    // Recorrer los botones de respuesta y seleccionar el primero disponible
                    for (int index = 0; index < answerObjects.Length; index++)
                    {
                        if (answerObjects[index].GetComponent<Button>().IsInteractable())
                        {
                            if (!Application.isMobilePlatform && keyboardControls)
                                eventSystem.SetSelectedGameObject(answerObjects[index].gameObject);

                            break;
                        }
                    }
                }

                // Reducir el bono a la mitad de su valor actual
                bonus *= 0.5f;

                // Perder algo de tiempo como penalización
                timeLeft -= 10f; // asumiendo timeLoss es 10 segundos

                // Mostrar el texto del bono
                if (bonusObject) bonusObject.Find("Text").GetComponent<Text>().text = bonus.ToString();

                // Aumentar el contador de errores
                mistakeCount++;

                // Si alcanzamos el número máximo de errores, dar sin bono y pasar a la siguiente pregunta
                if (mistakeCount >= maximumMistakes)
                {
                    // No dar bono
                    bonus = 0;

                    // Mostrar el texto del bono
                    if (bonusObject) bonusObject.Find("Text").GetComponent<Text>().text = bonus.ToString();

                    // Reducir las vidas
                    player.lives--;

                    // Actualizar las vidas que quedan
                    Updatelives();

                    // Aumentar la estadística de respuestas incorrectas
                    wrongAnswers++;

                    // Mostrar el resultado de esta pregunta, que es incorrecto
                    ShowResult(false);
                }

                // Si hay una fuente y un sonido, reproducirlo desde la fuente
                if (soundSource && soundWrong) soundSource.GetComponent<AudioSource>().PlayOneShot(soundWrong);
            }
            else // Elegir la respuesta correcta
            {
                // Si respondimos correctamente en esta ronda, aumentar el contador de preguntas para este grupo de bonificación
                questionCount++;

                // Aumentar el contador de respuestas correctas. Esto se usa para mostrar cuántas respuestas acertamos al final del juego
                correctAnswers++;

                // Reproducir la animación de respuesta correcta
                if (animationCorrect)
                {
                    // Si el clip de animación no existe en el componente de animación, añadirlo
                    if (answerObjects[answerIndex].GetComponent<Animation>().GetClip(animationCorrect.name) == null)
                        answerObjects[answerIndex].GetComponent<Animation>()
                            .AddClip(animationCorrect, animationCorrect.name);

                    // Reproducir la animación
                    answerObjects[answerIndex].GetComponent<Animation>().Play(animationCorrect.name);
                }

                // Animar el bono siendo añadido al puntaje
                if (bonusObject && bonusObject.GetComponent<Animation>())
                {
                    // Reproducir la animación
                    bonusObject.GetComponent<Animation>().Play();

                    // Restablecer la velocidad de la animación
                    bonusObject.GetComponent<Animation>()[bonusObject.GetComponent<Animation>().clip.name].speed = 1;
                }

                // Añadir el bono al puntaje del jugador actual
                player.scoreCount += bonus;

                // Añadir el bono de tiempo al tiempo restante, si tenemos un temporizador global
                if (globalTime > 0)
                {
                    timeLeft += 30; // asumiendo timeBonus es 30 segundos

                    // Si hemos superado el valor original del tiempo global, actualizar la barra de relleno para acomodar el nuevo valor
                    if (timeLeft > globalTime) globalTime = timeLeft;
                }

                // Si hay una fuente y un sonido, reproducirlo desde la fuente
                if (soundSource && soundCorrect) soundSource.GetComponent<AudioSource>().PlayOneShot(soundCorrect);

                // Mostrar el resultado de esta pregunta, que es correcto
                ShowResult(true);
            }
        }
    }

    /// <summary>
    /// Verifica las respuestas de una pregunta de elección múltiple.
    /// </summary>
    public void CheckMultiChoice()
    {
        // Ocultar el botón de verificación
        multiChoiceButton.SetActive(false);

        bool goodResult = true;

        for (int answerIndex = 0; answerIndex < questions[currentQuestion].answers.Length; answerIndex++)
        {
            // Deshabilitar el botón para que no podamos presionarlo nuevamente
            answerObjects[answerIndex].GetComponent<Button>().interactable = false;

            if (questions[currentQuestion].answers[answerIndex].isCorrect ==
                answerObjects[answerIndex].Find("Outline").GetComponent<Image>().enabled)
            {
                // Reproducir la animación Correcta
                if (animationCorrect)
                {
                    // Si el clip de animación no existe en el componente de animación, añadirlo
                    if (answerObjects[answerIndex].GetComponent<Animation>().GetClip(animationCorrect.name) == null)
                        answerObjects[answerIndex].GetComponent<Animation>()
                            .AddClip(animationCorrect, animationCorrect.name);

                    // Reproducir la animación
                    answerObjects[answerIndex].GetComponent<Animation>().Play(animationCorrect.name);
                }
            }
            else
            {
                // Reproducir la animación Incorrecta
                if (animationWrong)
                {
                    // Si el clip de animación no existe en el componente de animación, añadirlo
                    if (answerObjects[answerIndex].GetComponent<Animation>().GetClip(animationWrong.name) == null)
                        answerObjects[answerIndex].GetComponent<Animation>()
                            .AddClip(animationWrong, animationWrong.name);

                    // Reproducir la animación
                    answerObjects[answerIndex].GetComponent<Animation>().Play(animationWrong.name);
                }

                goodResult = false;
            }
        }

        if (goodResult)
        {
            // Si respondimos correctamente esta ronda, aumentar el contador de preguntas para este grupo de bonificación
            questionCount++;

            // Aumentar el contador de respuestas correctas. Esto se usa para mostrar cuántas respuestas acertamos al final del juego
            correctAnswers++;

            // Animar el bono siendo añadido al puntaje
            if (bonusObject && bonusObject.GetComponent<Animation>())
            {
                // Reproducir la animación
                bonusObject.GetComponent<Animation>().Play();

                // Restablecer la velocidad de la animación
                bonusObject.GetComponent<Animation>()[bonusObject.GetComponent<Animation>().clip.name].speed = 1;
            }

            // Añadir el bono al puntaje del jugador actual
            player.scoreCount += bonus;

            // Añadir el bono de tiempo al tiempo restante, si tenemos un temporizador global
            if (globalTime > 0)
            {
                timeLeft += timeBonus;

                // Si vamos más allá del valor original del tiempo global, actualizar la barra de llenado para acomodar el nuevo valor
                if (timeLeft > globalTime) globalTime = timeLeft;
            }

            // Si hay una fuente y un sonido, reproducirlo desde la fuente
            if (soundSource && soundCorrect) soundSource.GetComponent<AudioSource>().PlayOneShot(soundCorrect);
        }
        else
        {
            // Si no hay respuesta seleccionada, seleccionar el siguiente botón de respuesta disponible
            if (eventSystem.firstSelectedGameObject == null)
            {
                // Recorrer los botones de respuesta y seleccionar el primero disponible
                for (int index = 0; index < answerObjects.Length; index++)
                {
                    if (answerObjects[index].GetComponent<Button>().IsInteractable())
                    {
                        if (!Application.isMobilePlatform && keyboardControls)
                            eventSystem.SetSelectedGameObject(answerObjects[index].gameObject);

                        break;
                    }
                }
            }

            // Reducir el bono a la mitad de su valor actual
            bonus *= bonusLoss;

            // Perder algo de tiempo como penalización
            timeLeft -= timeLoss;

            // Mostrar el texto del bono
            if (bonusObject) bonusObject.Find("Text").GetComponent<Text>().text = bonus.ToString();

            // Aumentar el contador de errores
            mistakeCount++;

            // Si alcanzamos el número máximo de errores, dar sin bono y pasar a la siguiente pregunta
            if (mistakeCount >= maximumMistakes)
            {
                // No dar bono
                bonus = 0;

                // Mostrar el texto del bono
                if (bonusObject) bonusObject.Find("Text").GetComponent<Text>().text = bonus.ToString();

                // Reducir de vidas
                player.lives--;

                // Actualizar las vidas que quedan
                Updatelives();

                // Añadir a la estadística de respuestas incorrectas
                wrongAnswers++;

                // Mostrar el resultado de esta pregunta, que es incorrecto
                ShowResult(false);
            }

            // Si hay una fuente y un sonido, reproducirlo desde la fuente
            if (soundSource && soundWrong) soundSource.GetComponent<AudioSource>().PlayOneShot(soundWrong);
        }

        // Mostrar el resultado de esta pregunta
        ShowResult(goodResult);
    }

    /// <summary>
    /// Muestra el resultado de la pregunta, indicando si la respuesta fue correcta o no. También muestra un texto de seguimiento y revela una imagen de primer plano, si existen.
    /// </summary>
    /// <param name="isCorrectAnswer">Indica si obtuvimos la respuesta correcta.</param>
    public void ShowResult(bool isCorrectAnswer)
    {
        // Ahora no estamos haciendo una pregunta
        askingQuestion = false;

        // Detener el temporizador
        timerRunning = false;

        // Restablecer el contador de errores
        mistakeCount = 0;

        // Deshabilitar el botón de la pregunta, para que no intentemos abrir una imagen que no está ahí
        if (questionObject.GetComponent<Button>()) questionObject.GetComponent<Button>().enabled = false;

        // Recorrer todas las respuestas y hacerlas no clicables
        for (int index = 0; index < answerObjects.Length; index++)
        {
            // Si esta es la respuesta correcta, resaltarla y retrasar su animación
            if (index < questions[currentQuestion].answers.Length)
            {
                if (questions[currentQuestion].answers[index].isCorrect == true)
                {
                    // Resaltar la respuesta correcta
                    if (showCorrectAnswer == true) eventSystem.SetSelectedGameObject(answerObjects[index].gameObject);
                    else answerObjects[index].GetComponent<Button>().interactable = false;
                }
                else
                {
                    // Hacer todos los botones no interactivos
                    answerObjects[index].GetComponent<Button>().interactable = false;
                }
            }
        }

        // Restablecer la pregunta y las respuestas para mostrar la próxima pregunta
        StartCoroutine(ResetQuestion(0.5f));
    }

    /// <summary>
    /// Restablece la pregunta y las respuestas, en preparación para la próxima pregunta.
    /// </summary>
    /// <returns>La pregunta.</returns>
    /// <param name="delay">Retraso en segundos antes de mostrar la próxima pregunta.</param>
    IEnumerator ResetQuestion(float delay)
    {
        // Recorrer todas las respuestas y ocultar las incorrectas
        for (int index = 0; index < answerObjects.Length; index++)
        {
            // Si esta es una respuesta incorrecta, ocultarla. También si no se supone que debemos mostrar la respuesta correcta, ocultar todas las respuestas
            if (index < questions[currentQuestion].answers.Length &&
                (questions[currentQuestion].answers[index].isCorrect == false || showCorrectAnswer == false))
            {
                // Reproducir la animación Ocultar, después de que termine la animación actual
                if (animationHide)
                {
                    // Si el clip de animación no existe en el componente de animación, añadirlo
                    if (answerObjects[index].GetComponent<Animation>().GetClip(animationHide.name) == null)
                        answerObjects[index].GetComponent<Animation>().AddClip(animationHide, animationHide.name);

                    // Reproducir la animación en cola después de la animación actual
                    answerObjects[index].GetComponent<Animation>().PlayQueued(animationHide.name);
                }
            }
        }

        // Si se supone que debemos mostrar la respuesta correcta, encontrarla y mantenerla animada más tiempo que las otras respuestas
        if (showCorrectAnswer == true && questions[currentQuestion].multiChoice == false)
        {
            // Recorrer todas las respuestas nuevamente y resaltar la correcta
            for (int index = 0; index < answerObjects.Length; index++)
            {
                // Si esta es la respuesta correcta, resaltarla y retrasar su animación
                if (index < questions[currentQuestion].answers.Length &&
                    questions[currentQuestion].answers[index].isCorrect == true)
                {
                    // Resaltar la respuesta correcta
                    eventSystem.SetSelectedGameObject(answerObjects[index].gameObject);

                    // Esperar un momento
                    yield return new WaitForSeconds(0.5f);

                    // Reproducir la animación Ocultar
                    if (animationHide)
                    {
                        // Si el clip de animación no existe en el componente de animación, añadirlo
                        if (answerObjects[index].GetComponent<Animation>().GetClip(animationHide.name) == null)
                            answerObjects[index].GetComponent<Animation>().AddClip(animationHide, animationHide.name);

                        // Reproducir la animación
                        answerObjects[index].GetComponent<Animation>().Play(animationHide.name);
                    }
                }
            }
        }

        // Esperar un rato o hasta que termine el sonido que se está reproduciendo actualmente
        yield return new WaitForSeconds(delay);

        // Detener cualquier sonido que se esté reproduciendo y restablecer el tiempo de reproducción de sonido
        if (soundSource)
        {
            if (!isGameOver) soundSource.GetComponent<AudioSource>().Stop();
            soundPlayTime = 0;
        }

        // Deseleccionar la respuesta actualmente seleccionada
        eventSystem.SetSelectedGameObject(null);

        // Hacer la próxima pregunta
        StartCoroutine(AskQuestion(true));
    }


    /// <summary>
    /// Actualiza el texto del temporizador y verifica si se acabó el tiempo.
    /// </summary>
    void UpdateTime()
    {
        // Actualizar el tiempo solo si tenemos un objeto de temporizador asignado
        if (timerIcon || timerAnimated)
        {
            // Usando el icono de temporizador, que usa FillAmount y Text para mostrar el tiempo que nos queda
            if (timerIcon)
            {
                // Actualizar el círculo del temporizador, si tenemos uno
                if (timerBar)
                {
                    // Si tenemos un tiempo global, mostrar el progreso del temporizador para eso.
                    if (globalTime > 0)
                    {
                        timerBar.fillAmount = timeLeft / globalTime;
                    }
                    else if
                        (timerRunning ==
                         true) // Si el temporizador está en marcha, mostrar la cantidad de relleno que queda para el tiempo de la pregunta.
                    {
                        timerBar.fillAmount = timeLeft / questions[currentQuestion].time;
                    }
                    else // De lo contrario, rellenar la cantidad de nuevo al 100%
                    {
                        timerBar.fillAmount = Mathf.Lerp(timerBar.fillAmount, 1, Time.deltaTime * 10);
                    }
                }

                // Actualizar el texto del temporizador, si tenemos uno
                if (timerText)
                {
                    // Si el temporizador está en marcha, mostrar el tiempo que queda. De lo contrario, ocultar el texto
                    if (timerRunning == true || globalTime > 0) timerText.text = Mathf.RoundToInt(timeLeft).ToString();
                    else timerText.text = "";
                }
            }

            // Usando el temporizador animado, que progresa la animación basada en el tiempo que nos queda
            if (timerAnimated && timerAnimated.isPlaying == false)
            {
                // Iniciar la animación del temporizador
                timerAnimated.Play("TimerAnimatedProgress");

                // Si tenemos un tiempo global, mostrar el fotograma correcto de la animación de tiempo
                if (globalTime > 0)
                {
                    timerAnimated["TimerAnimatedProgress"].time = (1 - (timeLeft / globalTime)) *
                                                                  timerAnimated["TimerAnimatedProgress"].clip.length;
                }
                else if
                    (timerRunning ==
                     true) // Si el temporizador está en marcha, mostrar el fotograma correcto de la animación de tiempo
                {
                    timerAnimated["TimerAnimatedProgress"].time = (1 - (timeLeft / questions[currentQuestion].time)) *
                                                                  timerAnimated["TimerAnimatedProgress"].clip.length;
                }
                else // De lo contrario, rebobinar la animación de tiempo al inicio
                {
                    timerAnimated["TimerAnimatedProgress"].time =
                        Mathf.Lerp(timerAnimated["TimerAnimatedProgress"].time, 1, Time.deltaTime * 10);
                }

                // Comenzar a animar
                timerAnimated["TimerAnimatedProgress"].enabled = true;

                // Registrar el fotograma actual
                timerAnimated.Sample();

                // Detener la animación
                timerAnimated["TimerAnimatedProgress"].enabled = false;
            }

            // ¡Se acabó el tiempo!
            if (timeLeft <= 0 && timerRunning == true)
            {
                // Si tenemos un tiempo global y el temporizador se agotó, ir directamente a la pantalla de GameOver
                if (globalTime > 0)
                {
                    StartCoroutine(GameOver(1));
                }
                else
                {
                    // Reducir de vidas
                    player.lives--;

                    // Actualizar las vidas que nos quedan
                    Updatelives();

                    // Mostrar el resultado de esta pregunta, que es incorrecto (porque se nos acabó el tiempo, perdimos la pregunta)
                    ShowResult(false);
                }

                // Reproducir la animación del icono de temporizador
                if (timerIcon && timerIcon.GetComponent<Animation>()) timerIcon.GetComponent<Animation>().Play();

                // Reproducir la animación de tiempo agotado del temporizador animado
                if (timerAnimated && timerAnimated.GetComponent<Animation>())
                {
                    timerAnimated.Stop();
                    timerAnimated.Play("TimerAnimatedTimeUp");
                }

                // Si hay una fuente y un sonido, reproducirlo desde la fuente
                if (soundSource && soundTimeUp) soundSource.GetComponent<AudioSource>().PlayOneShot(soundTimeUp);
            }
        }
    }

    /// <summary>
    /// Actualiza el valor de la puntuación y verifica si alcanzamos el siguiente nivel.
    /// </summary>
    void UpdateScore()
    {
        // Actualizar el texto de la puntuación
        Debug.Log("PUNTUACIÓN DEL JUGADOR: " + player.score);
        if (player.scoreText) player.scoreText.GetComponent<Text>().text = player.score.ToString();

        // Si alcanzamos la puntuación de la victoria ganamos el juego
        if (scoreToVictory > 0 && player.score >= scoreToVictory) StartCoroutine(Victory(0));
    }

    /// <summary>
    /// Ejecuta el evento de fin del juego y muestra la pantalla de fin de juego.
    /// </summary>
    IEnumerator GameOver(float delay)
    {
        isGameOver = true;

        // Calcular la duración del cuestionario
        playTime = DateTime.Now - startTime;

        yield return new WaitForSeconds(delay);

        // Mostrar la pantalla de fin de juego
        if (gameOverCanvas)
        {
            // Mostrar la pantalla de fin de juego
            gameOverCanvas.gameObject.SetActive(true);

            // Escribir el texto de la puntuación, si existe
            if (gameOverCanvas.Find("ScoreTexts/TextScore"))
                gameOverCanvas.Find("ScoreTexts/TextScore").GetComponent<Text>().text +=
                    " " + player.score.ToString();

            // Verificar si obtuvimos un récord
            if (player.score > highScore)
            {
                highScore = player.score;

                // Registrar el nuevo récord
                PlayerPrefs.SetFloat("HighScore", player.score);

                OnUpdateLeaderboard();
            }

            // Escribir el texto del récord
            gameOverCanvas.Find("ScoreTexts/TextHighScore").GetComponent<Text>().text += " " + highScore.ToString();

            // Si hay una fuente y un sonido, reproducirlo desde la fuente
            if (soundSource && soundGameOver) soundSource.GetComponent<AudioSource>().PlayOneShot(soundGameOver);
        }
    }


    /// <summary>
    /// Ejecuta el evento de victoria y muestra la pantalla de victoria.
    /// </summary>
    IEnumerator Victory(float delay)
    {
        // Si este cuestionario no tiene preguntas en absoluto, simplemente regresa al menú principal.
        if (questions.Length <= 0) yield break;

        isGameOver = true;

        // Calcular la duración del cuestionario.
        playTime = DateTime.Now - startTime;

        yield return new WaitForSeconds(delay);

        // Mostrar la pantalla de victoria.
        if (victoryCanvas)
        {
            // Mostrar la pantalla de victoria.
            victoryCanvas.gameObject.SetActive(true);

            // Si tenemos objetos TextScore y TextHighScore, entonces estamos usando el lienzo de victoria para un solo jugador.
            if (victoryCanvas.Find("ScoreTexts/TextScore") && victoryCanvas.Find("ScoreTexts/TextHighScore"))
            {
                // Escribir el texto de la puntuación, si existe.
                victoryCanvas.Find("ScoreTexts/TextScore").GetComponent<Text>().text +=
                    " " + player.score.ToString();

                // Verificar si obtuvimos un puntaje alto.
                if (player.score > highScore)
                {
                    highScore = player.score;

                    // Registrar el nuevo puntaje alto.
                    PlayerPrefs.SetFloat("HighScore", player.score);

                    OnUpdateLeaderboard();
                }

                // Escribir el texto del puntaje alto.
                victoryCanvas.Find("ScoreTexts/TextHighScore").GetComponent<Text>().text += " " + highScore.ToString();
            }

            // Si tenemos un objeto TextProgress, entonces podemos mostrar cuántas preguntas respondimos correctamente.
            if (victoryCanvas.Find("ScoreTexts/TextProgress"))
            {
                // Escribir el texto del progreso.
                victoryCanvas.Find("ScoreTexts/TextProgress").GetComponent<Text>().text =
                    correctAnswers.ToString() + "/" + questionLimit.ToString();
            }

            // Si hay una fuente y un sonido, reproducirlo desde la fuente.
            if (soundSource && soundVictory) soundSource.GetComponent<AudioSource>().PlayOneShot(soundVictory);
        }
    }

    private void OnUpdateLeaderboard()
    {
        var leaderboard = triviaDB.GetTable_Leaderboard();

        _ = leaderboard
            .Update()
            .Data(
                leaderboard.C.score.Value(player.score)
            )
            .Where(leaderboard.C.email.Equal(player.email))
            .Run(
                (Info info) =>
                {
                    if (info.isOK)
                        Debug.Log("He actualizado " + info.affectedRows + " registros.");
                    else
                        Debug.LogWarning("¡No se insertó ningún registro!");
                }
            );
    }

    /// <summary>
    /// Reinicia el nivel actual.
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Vuelve al menú principal.
    /// </summary>
    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenuLevelName);
    }

    /// <summary>
    /// Actualiza las vidas que tenemos.
    /// </summary>
    public void Updatelives()
    {
        // Actualizar las vidas solo si tenemos una barra de vidas asignada.
        if (player.livesBar)
        {
            // Si nos quedamos sin vidas, es el fin del juego.
            if (player.lives <= 0) StartCoroutine(GameOver(1));
        }
    }

    /// <summary>
    /// Establece la lista de preguntas desde una lista de preguntas externa. Esto se usa cuando se obtienen las preguntas de un selector de categoría.
    /// </summary>
    /// <param name="setValue">La lista de preguntas que obtuvimos.</param>
    public void SetQuestions(Pregunta[] setValue)
    {
        questions = setValue;
    }

    /// <summary>
    /// Salta la pregunta actual y muestra la siguiente.
    /// </summary>
    public void SkipQuestion()
    {
        // Dejar de escuchar un clic en el botón para pasar a la siguiente pregunta.
        if (questionObject.GetComponent<Button>()) questionObject.GetComponent<Button>().onClick.RemoveAllListeners();

        // Restablecer la pregunta y las respuestas para mostrar la próxima pregunta.
        StartCoroutine(ResetQuestion(0.5f));
    }

    /// <summary>
    /// Da al jugador una vida extra una vez por partido. Esto se puede usar con UnityAds para recompensar al jugador al ver un anuncio.
    /// </summary>
    public void ExtraLife()
    {
        // Añadir a las vidas.
        player.lives++;

        // Actualizar las vidas que nos quedan.
        Updatelives();

        // El juego ya no ha terminado.
        isGameOver = false;

        // Restablecer los textos de puntuación y ocultar el objeto de juego terminado.
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

        // Hacer la próxima pregunta.
        StartCoroutine(AskQuestion(false));
    }
}