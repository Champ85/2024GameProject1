using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUIManager : MonoBehaviour
{
    private Health health;
    private TextMeshProUGUI healthText;
    private Camera cam;

    void Start()
    {
        health = GetComponentInParent<Health>();
        healthText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        cam = Camera.main;
    }

    void Update()
    {
        healthText.text = string.Format("HP: {0}", health.GetCurrentHealth());
        transform.LookAt(cam.transform);
    }
}
