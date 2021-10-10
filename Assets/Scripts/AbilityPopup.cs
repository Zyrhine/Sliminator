using TMPro;
using UnityEngine;

public class AbilityPopup : MonoBehaviour
{
    public TMP_Text TipText;

    private void Start()
    {
        Destroy(gameObject, 8f);
    }

    public void SetTipText(string text)
    {
        TipText.text = text;
    }
}
