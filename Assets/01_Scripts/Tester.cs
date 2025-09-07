using EditorAttributes;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [Button]
    void SetHP(float hp)
    {
        GlobalData.OnHPChange?.Invoke(hp);
    }
    [Button]
    void SetMP(float mp)
    {
        GlobalData.OnMPChange?.Invoke(mp);
    }
    [Button]
    void GetTimePiece()
    {
        GlobalData.OnTimePieceGet?.Invoke();
    }
}
