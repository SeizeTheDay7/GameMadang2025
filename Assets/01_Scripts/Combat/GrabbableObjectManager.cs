using UnityEngine;

public class GrabbableObjectManager : MonoBehaviour
{
    [Header(" - GrabbableObjectManager - ")]
    [SerializeField] GrabbableObject[] grabbableObjects;
    [SerializeField] float spawnCoolTime = 5f;

    private void Awake()
    {
        foreach (var obj in GetComponentsInChildren<GrabbableObject>())
            obj.Init(this);

    }

    public void QueueRespawn(Transform obj, Vector3 initPos)
    {
        if (!gameObject.activeSelf) return; // 게임 끌 때 오류 없애기 위함
        StartCoroutine(CoRespawn(obj, initPos));
    }

    public System.Collections.IEnumerator CoRespawn(Transform obj, Vector3 initPos)
    {
        yield return new WaitForSeconds(spawnCoolTime);
        obj.GetComponent<GrabbableObjectAttribute>().FullHealth();
        obj.position = initPos;
        obj.gameObject.SetActive(true);
    }
}