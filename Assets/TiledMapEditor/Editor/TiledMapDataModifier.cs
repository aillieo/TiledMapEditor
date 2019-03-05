using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace AillieoUtils.TiledMapEditor
{

    public class TiledMapDataModifier
    {
        public BaseDataAdapter Data { get; private set; }

        Texture2D mapTexture;

        GameObject mRootGo;

        public BaseBrush Brush { get; private set; }


        public TiledMapDataModifier()
        {
            Brush = new DefaultBrush();

            Brush.OnPaint -= UpdateGridsData;
            Brush.OnPaint += UpdateGridsData;
        }


        public void BindDataAdapter(BaseDataAdapter adapter, Object data)
        {
            if(null != Data)
            {
                Data.CleanUp();
            }
            Data = adapter;


            Data.LoadData(data);

            InitDataInView();

        }


        void InitDataInView()
        {
            int rangeX = Data.Range.x;
            int rangeZ = Data.Range.y;

            int count = Data.GetEnumCount();

            mRootGo = new GameObject("EditRoot");
            mRootGo.transform.localPosition = Vector3.zero;
            mRootGo.transform.localScale = Vector3.one;
            mRootGo.transform.localRotation = Quaternion.identity;

            var names = Data.GetEnumNames();

            Shader shader = Shader.Find("AillieoUtils/TiledMapGrid");
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "quad";
            quad.transform.SetParent(mRootGo.transform, false);
            quad.transform.localPosition = new Vector3(rangeX /2, 0, rangeZ/2);
            quad.transform.localScale = new Vector3(rangeX, rangeZ, 1);
            quad.transform.localEulerAngles = new Vector3(90, 0, 0);
            mapTexture = new Texture2D(rangeX, rangeZ);
            Material material = new Material(shader);
            material.SetTexture("_MainTex", mapTexture);
            Renderer renderer = quad.GetComponent<MeshRenderer>();
            renderer.material = material;

        }

        public void UpdateGridsData(Vector2Int[] pos, int newValue)
        {
            int rangeX = Data.Range.x;
            int rangeZ = Data.Range.y;
            foreach (var p in pos)
            {
                int x = p.x;
                int z = p.y;

                if ((x >= 0 && x < rangeX) && (z >= 0 && z < rangeZ))
                {
                    Data.GridData[x,z] = newValue;
                }

            }
            UpdateGridInView();
        }


        void UpdateGridInView()
        {
            int rangeX = Data.Range.x;
            int rangeZ = Data.Range.y;

            for (int i = 0; i < rangeZ; ++i)
                for (int j = 0; j < rangeX; ++j)
                {
                    if (Data.GridData[i, j] >= 0)
                    {
                        int t = Data.GridData[i, j];
                        mapTexture.SetPixel(i, j, Data.GetConfigColor(t));
                    }
                }
            mapTexture.Apply();
        }


        public void SaveData()
        {
            // save data
            Data.WriteData();

            // save colorConfig
            Data.Config.SaveColors();

            AssetDatabase.Refresh();
        }



        bool mIsPressed = false;
        public void HandleSceneEvent(Event current)
        {
            EventType et = current.type;

            if (current.isMouse)
            {
                mIsPressed = (current.button == 0) && (et == EventType.MouseDrag || et == EventType.MouseDown);
            }
            HandleMouseEvent(mIsPressed);

            UpdateGridInView();
        }


        void HandleMouseEvent(bool pressed)
        {
            Vector2 mousePos = Event.current.mousePosition;
            mousePos.y = Screen.height - mousePos.y - 40;
            Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePos);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                Vector2 vec = new Vector2(hitInfo.point.x, hitInfo.point.z);
                Brush.UpdateBrushState(vec, pressed);
            }
        }

        public void CleanUp()
        {
            GameObject.DestroyImmediate(mRootGo);
            Data.CleanUp();
            Data = null;
            Brush.OnPaint += UpdateGridsData;
            Brush = null;
        }


    }
}
