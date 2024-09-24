using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Object = UnityEngine.Object;

public class TerrainDataSerializer
{
    public static string Serialize(Terrain terrain)
    {
        var terrainData = terrain.terrainData;
        return Serialize(terrainData);
    }

    public static string Serialize(TerrainData terrainData)
    {
        var serializedTerrain = GetSerializedTerrainByTerrainData(terrainData);

        return Serialize(serializedTerrain);
    }

    public static SerializableTerrainData GetSerializedTerrainByTerrainData(TerrainData terrainData)
    {
        return new SerializableTerrainData
        {
            heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution),
            treeInstances = terrainData.treeInstances,
            alphamapLayers = terrainData.alphamapLayers,
            detailResolution = terrainData.detailResolution,
            size = terrainData.size,
            otherMaterials = TerrainEditor.Instance.plantInstances
                .ConvertAll(x => new PrefabInstanceData
                {
                    prefabIndex = x.listPickerIndex,
                    localPosition = x.instance.transform.localPosition,
                    localRotation = x.instance.transform.localRotation
                })
                .ToArray()
        };
    }

    public static string Serialize(SerializableTerrainData serializedTerrain)
    {
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new Vector3ArrayConverter(), new QuaternionArrayConverter() }
        };

        string json = JsonConvert.SerializeObject(serializedTerrain, Formatting.Indented, settings);
        return json;
    }

    public static void Deserialize(Terrain terrain, string json)
    {
        TerrainEditor.Instance.mapEditorOptions.plantOptions.RemoveAllPlants();
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new Vector3ArrayConverter(), new QuaternionArrayConverter() }
        };
    
        var serializedTerrain = JsonConvert.DeserializeObject<SerializableTerrainData>(json, settings);

        LoadBySerializableTerrainData(terrain, serializedTerrain);
    }

    public static void LoadBySerializableTerrainData(Terrain terrain, SerializableTerrainData serializedTerrain)
    {
        var terrainData = terrain.terrainData;

        terrainData.heightmapResolution = serializedTerrain.heightMap.GetLength(0);
        terrainData.SetHeights(0, 0, serializedTerrain.heightMap);

        terrainData.treeInstances = serializedTerrain.treeInstances;
        terrainData.size = serializedTerrain.size;
        

        foreach (var prefabInstanceData in serializedTerrain.otherMaterials)
        {
            var prefab = TerrainEditor.Instance.plants.list[prefabInstanceData.prefabIndex];
            var instance = Object.Instantiate(prefab, terrain.transform);
            instance.transform.localPosition = prefabInstanceData.localPosition;
            instance.transform.localRotation = prefabInstanceData.localRotation;
            TerrainEditor.Instance.plantInstances.Add(new PlantInstance(prefabInstanceData.prefabIndex, instance));
        }
    }
}

// Seri hale getirilebilir TerrainData sınıfı
public class SerializableTerrainData
{
    public float[,] heightMap;
    public TreeInstance[] treeInstances;
    public PrefabInstanceData[] otherMaterials = Array.Empty<PrefabInstanceData>();
    public int alphamapLayers;
    public int detailResolution;
    public Vector3 size;
}

public class PrefabInstanceData
{
    public int prefabIndex;
    public Vector3 localPosition;
    public Quaternion localRotation;
}