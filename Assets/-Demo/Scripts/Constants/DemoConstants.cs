using Sturfee.Unity.XR.Core.Constants;
using UnityEngine;

public static class DemoConstants{

    // Layers
    public static int MapLayer = LayerMask.NameToLayer("map");
    public static int MapItemLayer = LayerMask.NameToLayer("mapItem");
    public static int SturfeeTerrainLayer = LayerMask.NameToLayer(SturfeeLayers.Terrain);
    public static int SturfeeBuildingLayer = LayerMask.NameToLayer(SturfeeLayers.Building);
}
