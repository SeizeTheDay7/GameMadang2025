using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatUpCard : MonoBehaviour
{
    public Character character;
    private StatType type;
    [field: SerializeField] public TextMeshProUGUI title { get; private set; }
    [field: SerializeField] public Image image { get; private set; }
    [field: SerializeField] public TextMeshProUGUI detail { get; private set; }

    public void SetType(StatType type)
    {
        this.type = type;
    }

    public void StatUp()
    {
        character.StatUp(type);
    }
}