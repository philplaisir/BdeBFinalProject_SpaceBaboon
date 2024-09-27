


namespace SpaceBaboon
{
    public class GameStats
    {
        private static CurrentGameStats m_stats;

        public static void ResetStats()
        {

        }

        public static void CollectPlayerDamageTaken(float damage)
        {
            m_stats.playerDamageTaken += damage;
        }

        public static float GetPlayerDamageTaken()
        {
            return m_stats.playerDamageTaken;
        }
    }

    public struct CurrentGameStats
    {        
        public float playerDamageTaken;
        public int enemyKillsAmount;
    }
}
