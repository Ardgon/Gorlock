using UnityEngine;

public class GameMode : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverScreen;
    [SerializeField]
    private int coins;
    private float hitPoints;
    private Health burgerHealth;
    
    // Singleton instance
    private static GameMode _instance;

    // Public property to access the singleton instance
    public static GameMode Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameMode>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameMode");
                    _instance = singletonObject.AddComponent<GameMode>();
                }
            }

            return _instance;
        }
    }

    public void AddCoins(int amount)
    {
        coins += Mathf.Max(0, amount);
    }

    // Function to remove coins
    public bool RemoveCoins(int amount)
    {
        if (HasEnoughCoins(amount))
        {
            coins -= amount;
            return true;
        }
        else
        {
            return false; 
        }
    }

    public bool HasEnoughCoins(int amount)
    {
        return coins >= amount;
    }

    public int GetCoins()
    {
        return coins;
    }

    public float GetHitPoints()
    {
        return hitPoints;
    }

    private void EndGame()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
    }

    // Ensure the singleton is not destroyed on scene change
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        burgerHealth = FindObjectOfType<BurgerFoodSource>().GetComponent<Health>();
    }

    private void Update()
    {
        hitPoints = burgerHealth.GetHitPointPercentage() * 100;

        if (hitPoints <= 0) 
        {
            EndGame();
        }
    }
}
