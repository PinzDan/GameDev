using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;


public class SettingsUI : MonoBehaviour
{
    public AudioMixer masterMixer;
    [SerializeField] UIDocument uiDocument;
    private UnityEngine.UIElements.Slider volumeSlider;


    void OnEnable()
    {
        StartCoroutine(Wait());

    }

    private IEnumerator Wait()
    {
        yield return null;
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;


        volumeSlider = root.Q<Slider>("volume-slider");

        if (volumeSlider == null)
        {
            Debug.LogError("Non riesco a trovare lo slider del volume! + root: " + root);
            yield return null;
        }



        float currentDb;

        if (masterMixer.GetFloat("MainTheme", out currentDb))
        {


            volumeSlider.value = Mathf.InverseLerp(10f, 0f, currentDb) * 100f;
        }


        volumeSlider.RegisterValueChangedCallback(evt =>
        {
            SetVolume(evt.newValue);
        });
    }

    private void SetVolume(float sliderValue)
    {
        float linear = volumeSlider.value / 100f;
        float db = Mathf.Log10(Mathf.Max(linear, 0.0001f)) * 20f;
        masterMixer.SetFloat("MainTheme", db);
    }
}

