using UnityEngine;

public class DebugInput : MonoBehaviour
{
    [SerializeField] Character character;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            character.GainEXP(1000);
        }
    }
}