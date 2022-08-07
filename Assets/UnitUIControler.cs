using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUIControler : MonoBehaviour
{
    public Slider slider;
    public void SetHealth(float health, float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = health;
    }

    public void SetAllyName(string _allyName)
    {
        transform.GetChild(1).GetComponent<Text>().text = _allyName;
    }
}
