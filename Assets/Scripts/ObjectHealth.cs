using System;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{

    [SerializeField] StateManager managerRef;

    public float health = 100f;

    public event Action<float, GameObject> OnTakeDamage;
    public event Action OnDeath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        managerRef = GetComponent<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void takeDamage(float damage, GameObject damageDealer = null) {

        health -= damage;

        OnTakeDamage?.Invoke(damage, damageDealer);

        

        if (health <= 0) {
            OnDeath?.Invoke();

        
        
        }

    
    }
}
