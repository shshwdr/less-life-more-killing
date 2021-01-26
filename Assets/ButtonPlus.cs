using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPlus : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Text theText;
    AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();

    }

    public void OnClick()
    {
        audio.Play();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        theText.color = Color.red; //Or however you do your color
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        theText.color = Color.white; //Or however you do your color
    }
}