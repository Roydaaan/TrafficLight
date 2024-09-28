using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    [SerializeField] private GameObject redLight;
    [SerializeField] private GameObject yellowLight;
    [SerializeField] private GameObject greenLight;

    [SerializeField] private float redLightDuration = 5f;
    [SerializeField] private float yellowLightDuration = 2f;
    [SerializeField] private float greenLightDuration = 5f;

    private float _timer;
    private enum LightState { Red, YellowToGreen, Green, YellowToRed }
    private LightState _currentState;

    private void Start()
    {
        _currentState = LightState.Red;
        UpdateLights();
        _timer = redLightDuration;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0) SwitchState();
    }

    private void SwitchState()
    {
        switch (_currentState)
        {
            case LightState.Red:
                _currentState = LightState.YellowToGreen;
                _timer = yellowLightDuration;
                break;
            case LightState.YellowToGreen:
                _currentState = LightState.Green;
                _timer = greenLightDuration;
                break;
            case LightState.Green:
                _currentState = LightState.YellowToRed;
                _timer = yellowLightDuration;
                break;
            case LightState.YellowToRed:
                _currentState = LightState.Red;
                _timer = redLightDuration;
                break;
        }
        UpdateLights();
    }

    private void UpdateLights()
    {
        redLight.GetComponent<Renderer>().material.color = _currentState == LightState.Red ? Color.red : Color.gray;
        yellowLight.GetComponent<Renderer>().material.color = (_currentState == LightState.YellowToGreen || _currentState == LightState.YellowToRed) ? Color.yellow : Color.gray;
        greenLight.GetComponent<Renderer>().material.color = _currentState == LightState.Green ? Color.green : Color.gray;
    }
}
