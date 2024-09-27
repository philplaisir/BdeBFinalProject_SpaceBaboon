using SpaceBaboon.EnemySystem;
using SpaceBaboon.TutorialSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceBaboon
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        public Transform m_dmgPopUpPrefab;
        public Player Player { get; set; }
        public EnemySpawner EnemySpawner { get; set; }

        private EndGameScreen m_endGameScreen;

        private TutorialPopUpWindow m_tutorialWindow;

        public ResourceSpawner m_ressourceManager;

        public SaveData SaveDataManager { get; set; }

        public float WindowSizeScale { get; set; } = 1.0f;

        public float GameTimer { get; private set; } = 0.0f;

        private bool m_isCountingTime = false;
        private bool m_isPaused = false;

        private bool m_intro1HasBeenCalled = false;
        private bool m_intro2HasBeenCalled = false;
        private bool m_weaponPresentationHasBeenCalled = false;

        private bool m_hasSetUpCrafting = false;

        [SerializeField] private bool m_buildMode = false;

        public static GameManager Instance
        {
            get
            {
                if (instance != null) { return instance; }

                Debug.LogError("GameManager instance is null");
                return null;
            }
        }

        #region Unity
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }
            instance = this;
            DontDestroyOnLoad(this);
        }
        private void Start()
        {
            //StartTimer();

            if (!m_buildMode)
            {
                StartTimer();
            }
        }

        private void Update()
        {
            if (m_isPaused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
            if (!m_isPaused && m_isCountingTime)
            {
                GameTimer += Time.deltaTime;

                if (!m_intro1HasBeenCalled && GameTimer > 2.0f)
                {
                    m_intro1HasBeenCalled = true;
                    DisplayTutorialWindow(ETutorialType.Introduction1, Player.transform.position);
                }
                if (!m_intro2HasBeenCalled && GameTimer > 4.0f)
                {
                    m_intro2HasBeenCalled = true;
                    DisplayTutorialWindow(ETutorialType.Introduction2, Player.transform.position);
                }
                if (!m_weaponPresentationHasBeenCalled && GameTimer > 5.0f)
                {
                    m_weaponPresentationHasBeenCalled = true;
                    DisplayTutorialWindow(ETutorialType.WeaponPresentation, new Vector3(1000, 1000, 0));
                }
            }
        }

        #endregion

        public void StartGame()
        {
            SceneManager.LoadScene("FINAL GAME");
            StartTimer();
            //m_isPaused = false;
            PauseGame(false);

            //DisplayTutorialWindow(ETutorialType.Introduction1, Player.transform.position);
        }

        private void StartTimer()
        {
            GameTimer = 0.0f;
            m_isCountingTime = true;
        }

        public void PauseGame(bool value)
        {
            m_isPaused = value;
        }


        public void EndGame()
        {
            var stats = EnemySpawner.GetEnemyKillStats();

            //Debug.Log("melee kills" + stats[0]);
            //Debug.Log("shooting kills" + stats[1]);
            //Debug.Log("kamikaze kills" + stats[2]);
            //Debug.Log("boss kills" + stats[3]);

            m_endGameScreen.ActivateScreen(stats);
            //m_isPaused = true;
            PauseGame(true);
            m_isCountingTime = false;
            m_hasSetUpCrafting = false;

            m_intro1HasBeenCalled = false;
            m_intro2HasBeenCalled = false;
            m_weaponPresentationHasBeenCalled = false;

    }

    public void DisplayTutorialWindow(ETutorialType type, Vector3 position)
        {
            if (m_buildMode)
            {
                PauseGame(true);
            }
            if (m_tutorialWindow != null)
            {
                m_tutorialWindow.Display(type, position);
            }
        }

        public ResourceSpawner GetResourceManager()
        {
            return m_ressourceManager;
        }

        #region Setters

        public void SetPlayer(Player player)
        {
            Player = player;
        }

        public void SetEnemySpawner(EnemySpawner enemySpawner)
        {
            EnemySpawner = enemySpawner;
        }

        public void SetEndGameScreenScript(EndGameScreen script)
        {
            m_endGameScreen = script;
        }

        public void SetTutorialWindow(TutorialPopUpWindow window)
        {
            m_tutorialWindow = window;
        }

        public void SetResourceManager(ResourceSpawner spawner)
        {
            m_ressourceManager = spawner;
        }

        public void SetSaveData(SaveData savemanager)
        {
            SaveDataManager = savemanager;
        }

        #endregion
    }
}
