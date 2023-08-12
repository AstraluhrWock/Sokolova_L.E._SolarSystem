using UnityEngine;
using TMPro;
internal class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private StarCollector _starCollector;

    private void OnEnable()
    {
        _starCollector = FindObjectOfType<StarCollector>();
    }

    private void Update()
    {
        _text.text = "Score: " + _starCollector.GetCountOfStar().ToString();
    }
}

