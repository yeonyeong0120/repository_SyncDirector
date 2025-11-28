using UnityEngine;
using System.Collections.Generic;

public class EffectSystem : MonoBehaviour
{
    public static EffectSystem Instance { get; private set; }

    [Header("¿Ã∆Â∆Æ «¡∏Æ∆’")]
    public GameObject sparkEffect;
    public GameObject glowEffect;

    private Dictionary<string, GameObject> interactableObjects = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject obj in objects)
        {
            interactableObjects[obj.name] = obj;
        }
    }

    public void PlayEffect(string effectType, string objectID)
    {
        if (!interactableObjects.ContainsKey(objectID)) return;

        GameObject obj = interactableObjects[objectID];
        GameObject effectPrefab = null;

        switch (effectType)
        {
            case "spark":
                effectPrefab = sparkEffect;
                break;
            case "glow":
                effectPrefab = glowEffect;
                break;
        }

        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, obj.transform.position, Quaternion.identity);
            Destroy(effect, 2f);
            Debug.Log($"[Effect] ¿Ã∆Â∆Æ ¿Áª˝: {effectType} on {objectID}");
        }
    }
}