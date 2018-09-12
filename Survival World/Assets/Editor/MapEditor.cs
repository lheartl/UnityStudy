using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(MapGenerator))]
public class MapEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //target 을 따로 설정하지 않아도 CustomEditor를 사용했기 때문에 target으로 자동으로 접근할수 있도록 설정
        MapGenerator map = target as MapGenerator;

        map.GenaratorMap();
    }
}
