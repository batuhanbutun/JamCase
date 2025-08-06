using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using UnityEditor;
using UnityEngine;

public enum PassengerType{Default,Bomb}

[Serializable]
public class PassengerTypeAndEnums
{
    public PassengerType passengerType;
    public GameObject passengerPrefab;
}
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
    [Tooltip("Hangi tür passenger yerleştirilecek")]
    [SerializeField] private PassengerType selectedPassengerType = PassengerType.Default;
    [SerializeField] private ObjColor selectedColor;
    [Tooltip("Passenger Prefabları")]
    [SerializeField] private List<PassengerTypeAndEnums> passengerTypeAndEnums;

    [Header("Bus Sequence")]
    [SerializeField] private List<ObjColor> busColorSequence = new();

    [Header("Level Duration")]
    [SerializeField] private float levelDuration = 30f;
    
    [Header("Save Target")]
    [SerializeField] private LevelData saveTarget;

    private Dictionary<Vector2Int, (GameObject obj, ObjColor color,PassengerType passengerType)> spawnedPassengers = new();
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
                
                Vector2Int gridPos = WorldToGridIndex(hit.point);

                if (Input.GetMouseButtonDown(0))
                    PlacePassenger(gridPos,selectedPassengerType);
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
        float offsetZ = 5f - totalHeight; 

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
            if (spawnedPassengers.TryGetValue(gridPos, out var entry))
            {
                if (entry.obj != null)
                    DestroyImmediate(entry.obj);
                spawnedPassengers.Remove(gridPos);
            }
            gridVisuals[gridPos.x, gridPos.y].GetComponent<Renderer>().material.color = Color.black;
        }
    }

    private void PlacePassenger(Vector2Int gridPos,PassengerType passengerType)
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
        
        var passengerPrefab = passengerTypeAndEnums.FirstOrDefault(x=>x.passengerType == passengerType)?.passengerPrefab;
        if(passengerPrefab == null)
            Debug.LogWarning("Passenger prefabı bulamadım bak bidahaa");
        var vis = Instantiate(passengerPrefab, pos, Quaternion.identity);
        vis.name = $"Passenger_{gridPos.x}_{gridPos.y}";

        var renderer = vis.GetComponent<Passenger>().passengerRenderer;
        if (renderer != null)
            renderer.material.color = ColorUtils.FromObjColor(selectedColor);

        spawnedPassengers[gridPos] = (vis, selectedColor,selectedPassengerType);
    }
    
    private void RemovePassenger(Vector2Int gridPos)
    {
        if (spawnedPassengers.TryGetValue(gridPos, out var passenger))
        {
            if (passenger.obj != null)
                DestroyImmediate(passenger.obj);

            spawnedPassengers.Remove(gridPos);
        }
    }
    
    public void GenerateBusSequenceFromPassengers()
    {
        busColorSequence.Clear();
        
        var counts = new Dictionary<ObjColor, int>();
        foreach (var kvp in spawnedPassengers)
        {
            var color = kvp.Value.color;
            if (!counts.ContainsKey(color))
                counts[color] = 0;
            counts[color]++;
        }
        
        foreach (var kv in counts)
        {
            int groups = kv.Value / 3;
            for (int i = 0; i < groups; i++)
                busColorSequence.Add(kv.Key);
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
                color = kvp.Value.color,
                passengerType = kvp.Value.passengerType
            });
        }
        UnityEditor.EditorUtility.SetDirty(saveTarget);
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
        levelDuration = saveTarget.levelDuration;
        foreach (var locked in lockedGrids)
        {
            gridVisuals[locked.x, locked.y].GetComponent<Renderer>().material.color = Color.black;
        }
        foreach (var data in saveTarget.passengerList)
        {
            selectedColor = data.color;
            PlacePassenger(data.gridPosition,data.passengerType);
        }
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
        EditorGUILayout.HelpBox(
            "- Editoru kullanmak için sahneyi başlatın ve editör objesini seçin.\n" +
            "- Passenger Type ile yerleştirmek istediğiniz passengerin tarzını seçebilirsiniz.\n" +
            "- Passenger Color ile ise passengerin rengini belirlersiniz.\n" +
            "- Sol tık: Sağ taraftan seçili renkte bir yolcu yerleştirir.\n" +
            "- Sağ tık: Seçtiğiniz hücredeki yolcuyu siler.(tabi varsa)\n" +
            "- Orta tık: Hücreyi kilitler. Zaten kilitliyse kilidi temizler.\n" +
            "- Level Duration: Seviyenin kaç saniye süresi olduğunu belirler. Saniye cinsinden tabi.\n" +
            "- Generate Grid: Yazdığınız width ve heightte grid oluşturur.\n" +
            "- Clear Grid: Mevcut gridi ve üstündeki yolcuları temizler.\n" +
            "- Generate Bus Sequence: Hangi renk otobüsten kaç adet olması gerekiyorsa listeye ekler(sırasını değiştirebilirsiniz).\n" +
            "- Save Level / Load Level: Level datayı kaydeder/ yükler. Eğer level data bulunuyorsa üstüne yazar. Yoksa yeni oluşturur",
            MessageType.Info
        );
        
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
        if (GUILayout.Button("Generate Bus Colors"))
        {
            editorTool.GenerateBusSequenceFromPassengers();
            UnityEditor.EditorUtility.SetDirty(editorTool);
        }
    }
}
#endif
