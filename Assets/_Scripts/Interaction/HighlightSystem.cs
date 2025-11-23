using UnityEngine;
using System.Collections.Generic;

public class HighlightSystem : MonoBehaviour
{
    public static HighlightSystem Instance { get; private set; }

    [Header("하이라이트 설정")]
    public Material highlightMaterial;
    public float highlightDuration = 3f;

    // Key: Object ID (string)
    private Dictionary<string, GameObject> interactableObjects = new Dictionary<string, GameObject>();
    // Key: GameObject
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
        // 씬에서 "Interactable" 태그를 가진 모든 오브젝트를 찾습니다.
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject obj in objects)
        {
            string objectID = obj.name;
            interactableObjects[objectID] = obj;

            // 원본 머티리얼 저장
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                originalMaterials[obj] = renderer.sharedMaterials; // sharedMaterials로 원본 저장
            }
        }
        Debug.Log($"[Highlight] {objects.Length}개 오브젝트 등록 완료.");
    }

    // OSCManager에서 호출됨
    public void HighlightObject(string objectID)
    {
        if (!interactableObjects.ContainsKey(objectID))
        {
            Debug.LogWarning($"[Highlight] 오브젝트 없음: {objectID}");
            return;
        }

        GameObject obj = interactableObjects[objectID];
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;

        // 하이라이트 머티리얼 적용
        Material[] newMaterials = new Material[renderer.materials.Length];
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = highlightMaterial;
        }
        renderer.materials = newMaterials;

        // 타이머 설정
        highlightTimers[obj] = highlightDuration;
        Debug.Log($"[Highlight] 오브젝트 하이라이트 시작: {objectID}");
    }

    void Update()
    {
        // 하이라이트 타이머 관리
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var kvp in highlightTimers)
        {
            GameObject obj = kvp.Key;
            float timer = kvp.Value - Time.deltaTime;

            if (timer <= 0) // **수정된 핵심 로직: 시간이 다 되면 제거**
            {
                RemoveHighlight(obj);
                toRemove.Add(obj);
            }
            else
            {
                highlightTimers[obj] = timer;
            }
        }

        // 제거할 오브젝트 정리
        foreach (GameObject obj in toRemove)
        {
            highlightTimers.Remove(obj);
        }
    }

    void RemoveHighlight(GameObject obj)
    {
        // 원본 머티리얼이 저장되어 있는지 확인하고 복구합니다.
        if (!originalMaterials.ContainsKey(obj)) return;

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.materials = originalMaterials[obj];
        }
        Debug.Log($"[Highlight] 하이라이트 제거: {obj.name}");
    }
}