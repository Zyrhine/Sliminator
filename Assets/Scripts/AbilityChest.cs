using Runemark.Common.Gameplay;
using UnityEngine;

public class AbilityChest : MonoBehaviour
{
    public RMRotator Rotator;
    public Player.PlayerAbility Ability = Player.PlayerAbility.None;

    [Header("Prefabs")]
    public GameObject AbilityPopup;

    /// <summary>
    /// Unlock the ability for the player.
    /// </summary>
    void Activate()
    {
        Rotator.Activate();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().UnlockAbility(Ability);
        GameObject popup = Instantiate(AbilityPopup, transform.position, Quaternion.identity);

        // Display a tip on what the ability is
        string text = "";
        switch (Ability)
        {
            case Player.PlayerAbility.Dash:
                text = "Press Shift to dodge";
                break;
            case Player.PlayerAbility.MortarMine:
                text = "Right click to launch an explosive";
                break;
            case Player.PlayerAbility.Resistance:
                text = "Resist damage from Minion Slime ooze";
                break;
            case Player.PlayerAbility.Shield:
                text = "Shield regenerates over time";
                break;
        }

        popup.GetComponent<AbilityPopup>().SetTipText(text);
    }
}
