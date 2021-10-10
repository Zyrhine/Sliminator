using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public float UnlocksAvailable;
    [HideInInspector] public float TimeTaken = 0;
    [HideInInspector] public float AmmoUsed = 0;
    [HideInInspector] public float UnlocksCollected = 0;
    [HideInInspector] public int Kills = 0;

    // Store ranks in lowest to highest 
    public Rank[] ranks; 

    // Update is called once per frame
    void Update()
    {
        // Add to the time taken
        TimeTaken += Time.deltaTime;
    }

    /// <summary>
    /// Add to the kill count.
    /// </summary>
    public void AddKill()
    {
        Kills += 1;
    }

    /// <summary>
    /// Add to the unlock count.
    /// </summary>
    public void AddUnlock()
    {
        UnlocksCollected += 1;
    }

    /// <summary>
    /// Add to the ammo used count.
    /// </summary>
    public void AddAmmoUsed()
    {
        AmmoUsed++;
    }

    /// <summary>
    /// Get the highest grade from current rank stats.
    /// </summary>
    /// <returns></returns>
    public string GetGrade()
    {
        Rank currentRank = null;
        foreach (Rank rank in ranks)
        {
            if (Kills >= rank.Kills && TimeTaken <= rank.TimeTaken && AmmoUsed <= rank.AmmoUsed)
            {
                currentRank = rank;
            }
        }

        // Return D grade if didn't achieve a rank
        if (currentRank == null)
        {
            return "D";
        } else
        {
            return currentRank.Grade;
        }
    }
}

[System.Serializable]
public class Rank
{
    public string Grade;
    public int Kills;
    public float TimeTaken;
    public int AmmoUsed;

    public Rank(string grade, float timeTaken, int kills, int ammoUsed)
    {
        Grade = grade;
        TimeTaken = timeTaken;
        Kills = kills;
        AmmoUsed = ammoUsed;
    }
}
