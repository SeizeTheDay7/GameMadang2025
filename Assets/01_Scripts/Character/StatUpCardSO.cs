using UnityEngine;

[CreateAssetMenu(fileName = "StatUpCardSO", menuName = "ScriptableObjects/StatUpCardSO", order = 1)]
public class StatUpCardSO : ScriptableObject
{
    public StatType statType;
    public string title;
    public Sprite icon;
    public string description;
}