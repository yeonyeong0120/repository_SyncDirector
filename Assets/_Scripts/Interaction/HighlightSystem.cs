using UnityEngine;
using System.Collections.Generic;

public class HighlightSystem : MonoBehaviour
{
    public static HighlightSystem Instance { get; private set; }

    [Header("하이라이트 설정")]
    public Material highlightMaterial;
    public float highlightDuration = 3f;

    private Dictionary<string, GameObject> interactableObjects = new Dictionary<string, GameObject>();
    private Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>();
    private Dictionary<GameObject, float> highlightTimers = new Dictionary<GameObject, float>();

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
        RegisterAllInteractableObjects();
    }

    void RegisterAllInteractableObjects()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject obj in objects)
        {
            string objectID = obj.name;
            interactableObjects[objectID] = obj;

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                originalMaterials[obj] = renderer.sharedMaterials;
            }
        }
        Debug.Log($"[Highlight] {objects.Length}개 오브젝트 등록 완료.");
    }

    public void HighlightObject(string objectID)
    {
        if (!interactableObjects.ContainsKey(objectID)) return;

        GameObject obj = interactableObjects[objectID];
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;

        Material[] newMaterials = new Material[renderer.materials.Length];
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = highlightMaterial;
        }
        renderer.materials = newMaterials;

        highlightTimers[obj] = highlightDuration;
        Debug.Log($"[Highlight] 오브젝트 하이라이트 시작: {objectID}");
    }

    void Update()
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var kvp in highlightTimers)
        {
            GameObject obj = kvp.Key;
            float timer = kvp.Value - Time.deltaTime;

            if (timer <= 0)
            {
                RemoveHighlight(obj);
                toRemove.Add(obj);
            }
            else
            {
                highlightTimers[obj] = timer;
            }
        }

        foreach (GameObject obj in toRemove)
        {
            highlightTimers.Remove(obj);
        }
    }

    void RemoveHighlight(GameObject obj)
    {
        if (!originalMaterials.ContainsKey(obj)) return;

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.materials = originalMaterials[obj];
        }
        Debug.Log($"[Highlight] 하이라이트 제거: {obj.name}");
    }
}