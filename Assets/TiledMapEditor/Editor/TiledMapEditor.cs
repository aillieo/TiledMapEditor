using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using UObject = UnityEngine.Object;


namespace AillieoUtils.TiledMapEditor
{
    public class TiledMapEditor : EditorWindow
    {
        [MenuItem("AillieoUtils/TiledMapEditor", false,200)]
        static void OpenTiledMapEditor()
        {
            CollectAdapters();
            GetWindow<TiledMapEditor>("TiledMapEditor");
        }

        // input
        static string[] AdapterNames;

        UObject data;

        int mModifyType = 0;

        // managed
        GameObject goRoot;
        TiledMapDataModifier tiledMapDataModifier;


        // edit state
        bool mIsEditing = false;
        int mBrushType = 0;
        int mBrushSize = 0;
        private string[] BrushTypeNames;


        static void CollectAdapters()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.BaseType == typeof(BaseDataAdapter))).ToArray();
            AdapterNames = new string[types.Length];
            int index = 0;
            foreach (var v in types)
            {
                AdapterNames[index] = v.FullName;
                index++;
            }
        }


        private void OnGUI()
        {

            GUILayout.BeginVertical("Box");
            
            data = EditorGUILayout.ObjectField(new GUIContent("Map Data", "地图数据"), data, typeof(UObject), true);

            if(!mIsEditing)
            {
                mModifyType = EditorGUILayout.Popup("Modify Type", mModifyType, AdapterNames);
            }

            GUILayout.EndVertical();

            if(mIsEditing)
            {
                OnGUIForEdit();
            }
            else
            {

                if (GUILayout.Button("Load !"))
                {
                    OnLoadClick();

                }
            }

        }



        void OnGUIForEdit()
        {
            if (GUILayout.Button("UnLoad !"))
            {
                OnUnloadClick();
                return;
            }

            if (null == tiledMapDataModifier)
            {
                GUILayout.Label("数据有问题!");
                return;
            }

            if (null == tiledMapDataModifier.Data)
            {
                //tiledMapDataModifier.CleanUp();
                GUILayout.Label("数据有问题!");
                return;
            }

            GUILayout.BeginVertical("Box");

            mBrushType = tiledMapDataModifier.Brush.BrushValue;
            int newBrushType = EditorGUILayout.Popup("Brush Type", mBrushType, BrushTypeNames);
            if (newBrushType != mBrushType)
            {
                mBrushType = newBrushType;
                tiledMapDataModifier.Brush.BrushValue = mBrushType;
            }

            mBrushSize = tiledMapDataModifier.Brush.BrushSize;
            int newBrushSize = EditorGUILayout.IntSlider("Brush Size",mBrushSize,1,5);
            if(newBrushSize != mBrushSize)
            {
                mBrushSize = newBrushSize;
                tiledMapDataModifier.Brush.BrushSize = mBrushSize;
            }

            for (int i = 0; i < BrushTypeNames.Length; ++i)
            {
                Color c = EditorGUILayout.ColorField(new GUIContent(BrushTypeNames[i], "地块颜色"), tiledMapDataModifier.Data.GetConfigColor(i));
                tiledMapDataModifier.Data.SetConfigColor(i,c);
            }

            if (GUILayout.Button(new GUIContent("Save", "保存地图数据")))
            {
                tiledMapDataModifier.SaveData();
            }

            GUILayout.EndVertical();

        }


        void OnLoadClick()
        {
            if (null == data)
            {
                Debug.LogError("没有指定数据");
                return;
            }

            tiledMapDataModifier = new TiledMapDataModifier();

            Type adapterType = Type.GetType(AdapterNames[mModifyType]);
            tiledMapDataModifier.BindDataAdapter(Activator.CreateInstance(adapterType) as BaseDataAdapter,data);

            BrushTypeNames = tiledMapDataModifier.Data.GetEnumNames();


            var sv = SceneView.lastActiveSceneView;
            sv.pivot = Vector3.zero;
            sv.rotation = new Quaternion(1, 0, 0, 1);
            sv.orthographic = true;
            sv.size = 200.0f;
            sv.Repaint();


            mIsEditing = true;

            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        }

        void OnUnloadClick()
        {
            if(null != tiledMapDataModifier)
            {
                tiledMapDataModifier.CleanUp();
                tiledMapDataModifier = null;
            }

            if (null != goRoot)
            {
                DestroyImmediate(goRoot);
                goRoot = null;
            }

            mIsEditing = false;

            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        }


        private void OnEnable()
        {
            if(!mIsEditing)
            {
                CollectAdapters();
            }
        }

        private void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        }



        void OnSceneGUI(SceneView sceneView)
        {
            tiledMapDataModifier.HandleSceneEvent(Event.current);

            if (Event.current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            }
        }

    }
}
