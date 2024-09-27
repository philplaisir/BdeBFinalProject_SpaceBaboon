using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SpaceBaboon
{
    public class EndGameScreen : MonoBehaviour
    {
        private UIDocument m_uiDoc;

        private VisualElement m_root;

        private Label m_timeAmount;
        private Label m_meleesAmount;
        private Label m_flyingAmount;
        private Label m_kamikazeAmount;
        private Label m_bossAmount;

        private Label m_scoreAmount;
        private Label m_highScoreAmount;
        private Button m_playAgainButton;

        private void Awake()
        {
            m_uiDoc = GetComponent<UIDocument>();
            VisualElement visualElement = m_uiDoc.rootVisualElement;

            m_root = visualElement.Q<VisualElement>("EndGameScreen");

            m_timeAmount = visualElement.Q<Label>("TimeValue");
            m_meleesAmount = visualElement.Q<Label>("MeleeValue");
            m_flyingAmount = visualElement.Q<Label>("FlyingValue");
            m_kamikazeAmount = visualElement.Q<Label>("KamikazeValue");
            m_bossAmount = visualElement.Q<Label>("BossValue");

            m_scoreAmount = visualElement.Q<Label>("ScoreAmount");
            m_highScoreAmount = visualElement.Q<Label>("HighScoreAmount");
            m_playAgainButton = visualElement.Q<Button>("BackButton");

        }

        private void Start()
        {
            GameManager.Instance.SetEndGameScreenScript(this);

        }

        private void OnEnable()
        {
            m_playAgainButton.clicked += LaunchMainMenu;
        }

        private void OnDisable()
        {
            m_playAgainButton.clicked -= LaunchMainMenu;
        }

        private void LaunchMainMenu()
        {

            m_root.style.display = DisplayStyle.None;
            SceneManager.LoadScene("FINAL MENU");
        }

        public void ActivateScreen(int[] stats)
        {
            m_root.style.display = DisplayStyle.Flex;

            int time = (int)GameManager.Instance.GameTimer;
            m_timeAmount.text = time.ToString();

            m_meleesAmount.text = stats[0].ToString();
            m_flyingAmount.text = stats[1].ToString();
            m_kamikazeAmount.text = stats[2].ToString();
            m_bossAmount.text = stats[3].ToString();

            int score = 0;
            for (int i = 0; i < 4; i++)
            {
                if (i == 3)
                {
                    score += (stats[i] * 10);
                    continue;
                }
                score += (stats[i] * 5);
            }

            int totalScore = time + score;
            m_scoreAmount.text = totalScore.ToString();
            GameManager.Instance.SaveDataManager.SetCurrentScore(totalScore);

            int highScore = GameManager.Instance.SaveDataManager.GetSavedScoreVariables().HighScoreData;
            m_highScoreAmount.text = highScore.ToString();
        }
    }
}
