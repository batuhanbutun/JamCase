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
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private Transform gridParent;
    private HashSet<Vector2Int> lockedGrids = new();
   

    [Header("Passenger Settings")]
    [SerializeField] private ObjColor selectedColor;
    [SerializeField] private GameObject passengerVisualPrefab;

    [Header("Bus Sequence")]
    [SerializeField] private List<ObjColor> busColorSequence = new();

    [Header("Level Duration")]
    [SerializeField] private float levelDuration = 30f;
    
    [Header("Save Target")]
    [SerializeField] private LevelData saveTarget;

    private Dictionary<Vector2Int, (GameObject obj, ObjColor color)> spawnedPassengers = new();
    private GameObject[,] gridVisuals;
    
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) )
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var gridObj = hit.collider.GetComponent<Transform>();
                if (gridObj == null) return;

                /*Vector2Int gridPos = new(
                    Mathf.RoundToInt(gridObj.position.x / gridSpacing),
                    Mathf.RoundToInt(gridObj.position.z / gridSpacing)
                );*/
                
                Vector2Int gridPos = WorldToGridIndex(hit.point);

                if (Input.GetMouseButtonDown(0))
                    PlacePassenger(gridPos);
                else if (Input.GetMouseButtonDown(1))
                    RemovePassenger(gridPos);
                else if (Input.GetMouseButtonDown(2))
                    ToggleLockGrid(gridPos);
            }
            
        }
    }
    
    public void GenerateGrid()
    {
        ClearGrid();

        gridVisuals = new GameObject[gridWidth, gridHeight];

        float totalWidth = (gridWidth - 1) * GridMovementPathfinder.GRİD_SPACİNG;
        float totalHeight = (gridHeight - 1) * GridMovementPathfinder.GRİD_SPACİNG;

        float offsetX = -totalWidth / 2f;
        float offsetZ = 5f - totalHeight; // z = 5 sabit üst hizaya göre

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 pos = new Vector3(
                    x * GridMovementPathfinder.GRİD_SPACİNG + offsetX,
                    0,
                    y * GridMovementPathfinder.GRİD_SPACİNG + offsetZ
                );

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
            if (i.Value.obj != null)
                DestroyImmediate(i.Value.obj);
        }
        lockedGrids.Clear();
        spawnedPassengers.Clear();

        Debug.Log("Grid ve üstündekiler temizlendi");
    }
    
    private void ToggleLockGrid(Vector2Int gridPos)
    {
        if (!lockedGrids.Add(gridPos))
        {
            lockedGrids.Remove(gridPos);
            gridVisuals[gridPos.x, gridPos.y].GetComponent<Renderer>().material.color = Color.white;
        }
        else
        {
            gridVisuals[gridPos.x, gridPos.y].GetComponent<Renderer>().material.color = Color.black;
        }
    }

    public void PlacePassenger(Vector2Int gridPos)
    {
        if (spawnedPassengers.ContainsKey(gridPos))
        {
            DestroyImmediate(spawnedPassengers[gridPos].obj);
            spawnedPassengers.Remove(gridPos);
        }
        
        float totalWidth = (gridWidth - 1) * GridMovementPathfinder.GRİD_SPACİNG;
        float totalHeight = (gridHeight - 1) * GridMovementPathfinder.GRİD_SPACİNG;

        float offsetX = -totalWidth / 2f;
        float offsetZ = 5f - totalHeight;

        Vector3 pos = new Vector3(
            gridPos.x * GridMovementPathfinder.GRİD_SPACİNG + offsetX,
            0f,
            gridPos.y * GridMovementPathfinder.GRİD_SPACİNG + offsetZ
        );

        var vis = Instantiate(passengerVisualPrefab, pos, Quaternion.identity);
        vis.name = $"Passenger_{gridPos.x}_{gridPos.y}";

        var renderer = vis.GetComponent<Passenger>().passengerRenderer;
        if (renderer != null)
            renderer.material.color = ColorUtils.FromObjColor(selectedColor);

        spawnedPassengers[gridPos] = (vis, selectedColor);
    }
    
    public void RemovePassenger(Vector2Int gridPos)
    {
        if (spawnedPassengers.TryGetValue(gridPos, out var passenger))
        {
            if (passenger.obj != null)
                DestroyImmediate(passenger.obj);

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
        saveTarget.lockedGridPositions = new List<Vector2Int>(lockedGrids);
        saveTarget.levelDuration = this.levelDuration;
        
        foreach (var kvp in spawnedPassengers)
        {
            saveTarget.passengerList.Add(new PassengerData
            {
                gridPosition = kvp.Key,
                color = kvp.Value.color 
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
        lockedGrids = new HashSet<Vector2Int>(saveTarget.lockedGridPositions);

        foreach (var locked in lockedGrids)
        {
            gridVisuals[locked.x, locked.y].GetComponent<Renderer>().material.color = Color.black;
        }
        foreach (var data in saveTarget.passengerList)
        {
            selectedColor = data.color;
            PlacePassenger(data.gridPosition);
        }
        Debug.Log("Level Yüklendi");
    }

    private Vector2Int WorldToGridIndex(Vector3 worldPos)
    {
        float totalWidth = (gridWidth - 1) * GridMovementPathfinder.GRİD_SPACİNG;
        float totalHeight = (gridHeight - 1) * GridMovementPathfinder.GRİD_SPACİNG;

        float offsetX = -totalWidth / 2f;
        float offsetZ = 5f - totalHeight;

        int x = Mathf.RoundToInt((worldPos.x - offsetX) / GridMovementPathfinder.GRİD_SPACİNG);
        int y = Mathf.RoundToInt((worldPos.z - offsetZ) / GridMovementPathfinder.GRİD_SPACİNG);

        return new Vector2Int(x, y);
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
