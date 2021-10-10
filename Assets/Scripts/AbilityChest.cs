using Runemark.Common.Gameplay;
using UnityEngine;

public class AbilityChest : MonoBehaviour
{
    public RMRotator Rotator;
    public Player.PlayerAbility Ability = Player.PlayerAbility.None;

    void Activate()
    {
        Rotator.Activate();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().UnlockAbility(Ability);
    }
}
