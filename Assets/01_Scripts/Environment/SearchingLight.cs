using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SearchingLight : MonoBehaviour
{
    [Header("너무 멀어지면 그림자를 못 만드네")]
    Light2D light2D;
    [SerializeField] float width = 2f;
    [SerializeField] float searchRange = 5f;
    [SerializeField] float searchSpeed = 1f;

    float timer;

    private void Awake()
    {
        TryGetComponent(out light2D);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float offset = Mathf.PingPong(timer += Time.deltaTime * searchRange * searchSpeed, searchRange) - searchRange * .5f;
        light2D.shapePath[0] = .5f * Vector3.right;
        light2D.shapePath[1] = .5f * -Vector3.right;
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position + new Vector3(-width + offset, 0, 0), -transform.up, Mathf.Infinity, 1 << 6);
        light2D.shapePath[2] = transform.InverseTransformPoint(raycastHit.point);
        raycastHit = Physics2D.Raycast(transform.position + new Vector3(width + offset, 0, 0), -transform.up, Mathf.Infinity, 1 << 6);
        light2D.shapePath[3] = transform.InverseTransformPoint(raycastHit.point);
    }
}
