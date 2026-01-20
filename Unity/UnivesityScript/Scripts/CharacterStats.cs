using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    public float maxHealth = 100f;
    public float currentHealth;
    public float maxStamina = 100f;
    public float currentStamina;


    // Start is called before the first frame update
    protected void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        Debug.Log("CharacterStats Start - Health: " + currentHealth);
        Debug.Log("CharacterStats Start - Stamina: " + currentStamina);
    }


    protected virtual void takeDamage(float danno = 10f)
    {
        currentHealth -= danno;
        // if (currentHealth <= 0)
        //     die();

    }

    protected virtual void useStamina(float quantita)
    {
        Debug.Log("uso");
        currentStamina -= quantita;
        if (currentStamina <= 0)
            currentStamina = 0;

    }

    public void regenerateStamina(float quantita)
    {
        currentStamina += quantita;

        if (currentStamina > maxStamina)
            currentStamina = maxStamina;
    }

    protected virtual void die()
    {
        Debug.Log("Character died: " + gameObject.name);
    }
}
