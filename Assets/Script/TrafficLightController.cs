using UnityEngine;
using Cysharp.Threading.Tasks;

public class TrafficLightController : MonoBehaviour
{
    [SerializeField] private GameObject redLight;
    [SerializeField] private GameObject yellowLight;
    [SerializeField] private GameObject greenLight;

    [SerializeField] private Material[] redMaterials;
    [SerializeField] private Material[] yellowMaterials;
    [SerializeField] private Material[] greenMaterials;
    [SerializeField] private Material defaultMaterial;

    [SerializeField] private ParticleSystem[] redParticles;
    [SerializeField] private ParticleSystem[] yellowParticles;
    [SerializeField] private ParticleSystem[] greenParticles;

    [SerializeField, Range(0.1f, 5f)] private float redParticleScale = 1f;
    [SerializeField, Range(0.1f, 5f)] private float yellowParticleScale = 1f;
    [SerializeField, Range(0.1f, 5f)] private float greenParticleScale = 1f;

    private const float LightDuration = 1f;
    private const int MillisecondsInSecond = 1000;

    private enum LightState { Red, YellowToGreen, Green, YellowToRed }
    private LightState _currentState;
    private ParticleSystem _activeParticle;

    private void Start()
    {
        _currentState = LightState.Red;
        InitializeParticles();
        UpdateLights();
        SwitchStateAsync().Forget();
    }

    private void InitializeParticles()
    {
        foreach (var particle in redParticles)
        {
            particle.Stop();
            particle.transform.localPosition = Vector3.zero;
            particle.transform.localScale = Vector3.one * redParticleScale;
        }

        foreach (var particle in yellowParticles)
        {
            particle.Stop();
            particle.transform.localPosition = Vector3.zero;
            particle.transform.localScale = Vector3.one * yellowParticleScale;
        }

        foreach (var particle in greenParticles)
        {
            particle.Stop();
            particle.transform.localPosition = Vector3.zero;
            particle.transform.localScale = Vector3.one * greenParticleScale;
        }
    }

    private async UniTaskVoid SwitchStateAsync()
    {
        while (true)
        {
            switch (_currentState)
            {
                case LightState.Red:
                    await UniTask.Delay((int)(LightDuration * MillisecondsInSecond));
                    _currentState = LightState.YellowToGreen;
                    break;
                case LightState.YellowToGreen:
                    await UniTask.Delay((int)(LightDuration * MillisecondsInSecond));
                    _currentState = LightState.Green;
                    break;
                case LightState.Green:
                    await UniTask.Delay((int)(LightDuration * MillisecondsInSecond));
                    _currentState = LightState.YellowToRed;
                    break;
                case LightState.YellowToRed:
                    await UniTask.Delay((int)(LightDuration * MillisecondsInSecond));
                    _currentState = LightState.Red;
                    break;
            }
            UpdateLights();
        }
    }

    private void UpdateLights()
    {
        redLight.GetComponent<Renderer>().material = _currentState == LightState.Red
            ? GetRandomMaterial(redMaterials)
            : defaultMaterial;
        yellowLight.GetComponent<Renderer>().material = _currentState is LightState.YellowToGreen 
            or LightState.YellowToRed 
            ? GetRandomMaterial(yellowMaterials) 
            : defaultMaterial;
        greenLight.GetComponent<Renderer>().material = _currentState == LightState.Green 
            ? GetRandomMaterial(greenMaterials) 
            : defaultMaterial;

        ManageParticles();
    }

    private Material GetRandomMaterial(Material[] materials)
    {
        if (materials.Length == 0) 
            return defaultMaterial;
        return materials[Random.Range(0, materials.Length)];
    }

    private void ManageParticles()
    {
        if (_activeParticle != null)
        {
            _activeParticle.Stop();
            _activeParticle.transform.localPosition = Vector3.zero;
            _activeParticle = null;
        }

        ParticleSystem[] currentParticles = _currentState switch
        {
            LightState.Red => redParticles,
            LightState.YellowToGreen or LightState.YellowToRed => yellowParticles,
            LightState.Green => greenParticles,
            _ => null
        };

        if (currentParticles is { Length: > 0 } && Random.value < 0.5f)
        {
            _activeParticle = currentParticles[Random.Range(0, currentParticles.Length)];
            _activeParticle.transform.position = GetCurrentLightPosition();
            SetParticleScale(_activeParticle);
            _activeParticle.Play();
        }
    }

    private Vector3 GetCurrentLightPosition()
    {
        return _currentState switch
        {
            LightState.Red => redLight.transform.position,
            LightState.YellowToGreen or LightState.YellowToRed => yellowLight.transform.position,
            LightState.Green => greenLight.transform.position,
            _ => Vector3.zero
        };
    }

    private void SetParticleScale(ParticleSystem particle)
    {
        if (_currentState == LightState.Red)
            particle.transform.localScale = Vector3.one * redParticleScale;
        else if (_currentState is LightState.YellowToGreen or LightState.YellowToRed)
            particle.transform.localScale = Vector3.one * yellowParticleScale;
        else if (_currentState == LightState.Green) 
            particle.transform.localScale = Vector3.one * greenParticleScale;
    }
}
