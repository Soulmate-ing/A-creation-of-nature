//#if UNITY_EDITOR
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(GridMap))]
//public class GridMapEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        GridMap gridMap = (GridMap)target;
//        if (GUILayout.Button("Generate Map Data"))
//        {
//            gridMap.UpdateTileProperties();
//        }
//    }
//}
//#endif