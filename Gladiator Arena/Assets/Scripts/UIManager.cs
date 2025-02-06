using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private Health playerHealth;
    private TextMeshProUGUI healthText;
    
    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();
        healthText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = string.Format("HP: {0}", playerHealth.GetCurrentHealth());
    }
}
