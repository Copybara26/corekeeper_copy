using UnityEngine;

public class FloatUI : MonoBehaviour
{
    [SerializeField] private float floatAmount = 8f;
    [SerializeField] private float floatSpeed = 1.5f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        float y = Mathf.Sin(Time.unscaledTime * floatSpeed) * floatAmount;
        transform.localPosition = startPos + new Vector3(0f, y, 0f);
    }
}