using System.Collections.Generic;
using _Scripts;
using UnityEditor;
using UnityEngine;

public class LevelEditorTool : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private LayerMask gridLayer;
    [SerializeField] private int gridWidth = 6;
    [SerializeField] private int gridHeight = 8;
    [SerializeField] private float gridSpacing = 1f;
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private Transform gridParent;

    [Header("Passenger Settings")]
    [SerializeField] private ObjColor selectedColor;
    [SerializeField] private GameObject passengerVisualPrefab;

    [Header("Bus Sequence")]
    [SerializeField] private List<ObjColor> busColorSequence = new();

    [Header("Save Target")]
    [SerializeField] private LevelData saveTarget;

    private Dictionary<Vector2Int, GameObject> spawnedPassengers = new();
    private GameObject[,] gridVisuals;
    
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var gridObj = hit.collider.GetComponent<Transform>();
                if (gridObj == null) return;

                Vector2Int gridPos = new(
                    Mathf.RoundToInt(gridObj.position.x / gridSpacing),
                    Mathf.RoundToInt(gridObj.position.z / gridSpacing)
                );

                if (Input.GetMouseButtonDown(0))
                    PlacePassenger(gridPos);
                else if (Input.GetMouseButtonDown(1))
                    RemovePassenger(gridPos);
            }
        }
    }
    
    public void GenerateGrid()
    {
        ClearGrid();

        gridVisuals = new GameObject[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 pos = new Vector3(x * gridSpacing, 0, y * gridSpacing);
                var obj = Instantiate(gridPrefab, pos, Quaternion.identity, gridParent);
                
                if (obj.GetComponent<Collider>() == null)
                        obj.AddComponent<BoxCollider>();
                
                gridVisuals[x, y] = obj;
            }
        }
    }

    public void ClearGrid()
    {
        if (gridParent != null)
        {
            for (int i = gridParent.childCount - 1; i >= 0; i--)
            {
                Transform child = gridParent.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
        }
        
        foreach (var i in spawnedPassengers)
        {
            if (i.Value != null)
                DestroyImmediate(i.Value);
        }

        spawnedPassengers.Clear();

        Debug.Log("Grid ve üstündekiler temizlendi");
    }

    public void PlacePassenger(Vector2Int gridPos)
    {
        if (spawnedPassengers.ContainsKey(gridPos))
        {
            DestroyImmediate(spawnedPassengers[gridPos]);
            spawnedPassengers.Remove(gridPos);
        }
        
        Vector3 pos = new Vector3(gridPos.x * gridSpacing, 0.2f, gridPos.y * gridSpacing);

        var vis = Instantiate(passengerVisualPrefab, pos, Quaternion.identity);
        vis.name = $"Passenger_{gridPos.x}_{gridPos.y}";

        var renderer = vis.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = ColorUtils.FromObjColor(selectedColor);

        spawnedPassengers[gridPos] = vis;
    }
    
    public void RemovePassenger(Vector2Int gridPos)
    {
        if (spawnedPassengers.TryGetValue(gridPos, out var passenger))
        {
            if (passenger != null)
                DestroyImmediate(passenger);

            spawnedPassengers.Remove(gridPos);
        }
    }
    
    public void SaveLevel()
    {
        if (saveTarget == null)
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Level Data",
                "NewLevelData",
                "asset",
                "Select a location"
            );

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            saveTarget = ScriptableObject.CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(saveTarget, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Yeni Level: {path}");
        }

        saveTarget.gridWidth = gridWidth;
        saveTarget.gridHeight = gridHeight;
        saveTarget.passengerList.Clear();
        saveTarget.busColorSequence = new(busColorSequence);

        foreach (var kvp in spawnedPassengers)
        {
            saveTarget.passengerList.Add(new PassengerData
            {
                gridPosition = kvp.Key,
                color = selectedColor
            });
        }

        UnityEditor.EditorUtility.SetDirty(saveTarget);
        Debug.Log("Level kaydedildi");
    }
    
    public void LoadLevel()
    {
        if (saveTarget == null)
            return;
        
        ClearGrid();
        gridWidth = saveTarget.gridWidth;
        gridHeight = saveTarget.gridHeight;
        busColorSequence = new(saveTarget.busColorSequence);
        GenerateGrid();
        foreach (var data in saveTarget.passengerList)
        {
            selectedColor = data.color;
            PlacePassenger(data.gridPosition);
        }
        Debug.Log("Level Yüklendi");
    }
    
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelEditorTool))]
public class LevelEditorToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelEditorTool editorTool = (LevelEditorTool)target;

        if (GUILayout.Button("Generate Grid"))
        {
            editorTool.GenerateGrid();
        }
        
        if (GUILayout.Button("Clear Grid"))
        {
            editorTool.ClearGrid();
        }

        if (GUILayout.Button("Save Level"))
        {
            editorTool.SaveLevel();
        }
        
        if (GUILayout.Button("Load Level"))
        {
            editorTool.LoadLevel();
        }
    }
}
#endif
