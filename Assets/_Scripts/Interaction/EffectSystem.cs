using UnityEngine;
using System.Collections.Generic;

public class EffectSystem : MonoBehaviour
{
    public static EffectSystem Instance { get; private set; }

    [Header("이펙트 프리팹")]
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
        // Interactable 오브젝트 등록 (HighlightSystem과 동일)
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject obj in objects)
        {
            interactableObjects[obj.name] = obj;
        }
    }

    // OSCManager에서 호출됨 (작업자 역할)
    public void PlayEffect(string effectType, string objectID)
    {
        if (!interactableObjects.ContainsKey(objectID))
        {
            Debug.LogWarning($"[Effect] 오브젝트 없음: {objectID}");
            return;
        }

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
            // 오브젝트 위치에 이펙트 생성
            GameObject effect = Instantiate(effectPrefab, obj.transform.position, Quaternion.identity);
            Destroy(effect, 2f); // 2초 후 제거
            Debug.Log($"[Effect] 이펙트 재생: {effectType} on {objectID}");
        }
    }
}