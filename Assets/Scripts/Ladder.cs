using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private LadderStep _defaultLadderStepPrefab;
    [SerializeField] private Transform[] _borders;
    [SerializeField] private float _stepsCount;

    private const float LadderDeltaStep = 1.5f;

    private void Awake()
    {
        ConfigureLadder();
    }

    private void ConfigureLadder()
    {
        float totalHeight = LadderDeltaStep * _stepsCount;

        foreach (var border in _borders)
        {
            Vector3 borderPosition = border.transform.position;
            Vector3 borderScale = border.transform.localScale;

            borderScale.y = totalHeight;
            borderPosition.y = totalHeight / 2f;

            border.transform.position = borderPosition;
            border.transform.localScale = borderScale;
        }

        float currentHeight = 0f;

        for (int i = 0; i < _stepsCount; i++)
        {
            Vector3 position = Vector3.zero;
            position.y = currentHeight;

            Instantiate(_defaultLadderStepPrefab, position, Quaternion.identity, transform);
            currentHeight += LadderDeltaStep;
        }
    }
}
