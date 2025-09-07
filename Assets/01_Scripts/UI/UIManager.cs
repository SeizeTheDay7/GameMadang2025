using Unity.UI.Shaders.Sample;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header(" - Stats - ")]
    [SerializeField] CustomSlider hp;
    [SerializeField] CustomSlider mp;
    [SerializeField] Image[] timePieces;

    [Header(" - Menu - ")]
    [SerializeField] GameObject optionMenu;
    int timePieceCount = 0;

    private void OnEnable()
    {
        GlobalData.OnHPChange += GlobalData_OnHPChange;
        GlobalData.OnMPChange += GlobalData_OnMPChange;
        GlobalData.OnTimePieceGet += GlobalData_OnTimePieceGet;
    }

    private void OnDisable()
    {
        GlobalData.OnHPChange -= GlobalData_OnHPChange;
        GlobalData.OnMPChange -= GlobalData_OnMPChange;
        GlobalData.OnTimePieceGet -= GlobalData_OnTimePieceGet;
    }

    private void GlobalData_OnHPChange(float obj)
    {
        hp.SetValue(obj);
    }
    private void GlobalData_OnMPChange(float obj)
    {
        mp.SetValue(obj);
    }
    private void GlobalData_OnTimePieceGet()
    {
        timePieceCount++;

        if (timePieceCount <= timePieces.Length)
            timePieces[timePieceCount - 1].enabled = true;
    }

    public void ChangeScene(int index)
    {
        SceneChangeManager.Instance.LoadSceneAsync(index);
    }

    public void ActivateOptionMenu(bool activate)
    {
        optionMenu.SetActive(activate);
    }

}
