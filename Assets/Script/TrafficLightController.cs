using UnityEngine;
using Cysharp.Threading.Tasks;

public class TrafficLightController : MonoBehaviour
{
    [SerializeField] private GameObject redLight;
    [SerializeField] private GameObject yellowLight;
    [SerializeField] private GameObject greenLight;

    [SerializeField] private Material redMaterial;
    [SerializeField] private Material yellowMaterial;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material defaultMaterial;

    private const float LightDuration = 1f;

    private enum LightState { Red, YellowToGreen, Green, YellowToRed }
    private LightState _currentState;

    private void Start()
    {
        _currentState = LightState.Red;
        UpdateLights();
        SwitchStateAsync().Forget();
    }

    private async UniTaskVoid SwitchStateAsync()
    {
        while (true)
        {
            switch (_currentState)
            {
                case LightState.Red:
                    await UniTask.Delay((int)(LightDuration * 1000));
                    _currentState = LightState.YellowToGreen;
                    break;
                case LightState.YellowToGreen:
                    await UniTask.Delay((int)(LightDuration * 1000));
                    _currentState = LightState.Green;
                    break;
                case LightState.Green:
                    await UniTask.Delay((int)(LightDuration * 1000));
                    _currentState = LightState.YellowToRed;
                    break;
                case LightState.YellowToRed:
                    await UniTask.Delay((int)(LightDuration * 1000));
                    _currentState = LightState.Red;
                    break;
            }
            UpdateLights();
        }
    }

    private void UpdateLights()
    {
        redLight.GetComponent<Renderer>().material = _currentState == LightState.Red ? redMaterial : defaultMaterial;
        yellowLight.GetComponent<Renderer>().material = (_currentState == LightState.YellowToGreen || _currentState == LightState.YellowToRed) ? yellowMaterial : defaultMaterial;
        greenLight.GetComponent<Renderer>().material = _currentState == LightState.Green ? greenMaterial : defaultMaterial;
    }
}
