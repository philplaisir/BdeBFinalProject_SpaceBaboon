using UnityEngine;
//using System.Collections.Generic;

namespace SpaceBaboon
{
    public class SaveData : MonoBehaviour
    {
        public Score m_score = new Score();

        private void Start()
        {
            GameManager.Instance.SetSaveData(this);
        }

        public void SaveJson()
        {
            string ScoreData = JsonUtility.ToJson(m_score);
            string ScoreFilePath = Application.persistentDataPath + "/ScoreData.json";
            Debug.Log(ScoreFilePath);
            System.IO.File.WriteAllText(ScoreFilePath, ScoreData);
            Debug.Log("Saved Done");
        }

        public void LoadJson()
        {
            string ScoreFilePath = Application.persistentDataPath + "/ScoreData.json";

            if (System.IO.File.Exists(ScoreFilePath))
            {
                string ScoreData = System.IO.File.ReadAllText(ScoreFilePath);

                if (!string.IsNullOrEmpty(ScoreData))
                {
                    m_score = JsonUtility.FromJson<Score>(ScoreData);
                    Debug.Log("Reload Completed");
                }
                else
                {
                    Debug.LogWarning("ScoreData is empty or null");
                }
            }
            else
            {
                Debug.LogWarning("Score file does not exist");
            }
        }


        [System.Serializable]
        public class Score
        {
            public int HighScoreData;
            public int LastScoreData;
        }

        public void SetCurrentScore(int _LastScoreData)
        {
            LoadJson();
            m_score.LastScoreData = _LastScoreData;

            if (_LastScoreData > m_score.HighScoreData)
            {
                m_score.HighScoreData = _LastScoreData;
            }
            SaveJson();
        }

        public Score GetSavedScoreVariables()
        {
            LoadJson();
            return m_score;
        }
    }
}
