using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace TiledMapEditor
{

    public class TiledMapDataModifier
    {
        public BaseDataAdapter Data { get; private set; }

        Vector3[] mVertices = null;

        Mesh[] mMesh;
        Material[] mMat;
        List<int>[] mTriangle;

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

            mVertices = new Vector3[(rangeZ + 1) * (rangeX + 1)];

            int idx = 0;
            for (int i = 0; i < rangeX + 1; ++i)
                for (int j = 0; j < rangeZ + 1; ++j)
                {
                    mVertices[idx++] = new Vector3(i, 0, j);
                }

            int count = Data.GetEnumCount();
            mMesh = new Mesh[count];
            mMat = new Material[count];

            mRootGo = new GameObject("EditRoot");
            mRootGo.transform.localPosition = Vector3.zero;
            mRootGo.transform.localScale = Vector3.one;
            mRootGo.transform.localRotation = Quaternion.identity;

            var collider = mRootGo.AddComponent<BoxCollider>();
            collider.center = new Vector3(rangeX,0,rangeZ) * 0.5f;
            collider.size = new Vector3(rangeX, 1, rangeZ);

            var names = Data.GetEnumNames();

            Shader shader = Shader.Find("AillieoUtils/TiledMapGrid");

            for (int i = 0; i < count; ++i)
            {
                GameObject go = new GameObject(names[i]);
                MeshRenderer renderType = go.AddComponent<MeshRenderer>();
                mMat[i] = new Material(shader);
                renderType.material = mMat[i];
                MeshFilter meshFilterType = go.AddComponent<MeshFilter>();
                Mesh mesh = new Mesh();
                mMesh[i] = mesh;
                meshFilterType.sharedMesh = mesh;
                go.transform.parent = mRootGo.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;
            }
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
            int count = Data.GetEnumCount();

            if(null == mTriangle)
            {
                mTriangle = new List<int>[count];
            }

            for (int i = 0; i < count; ++i)
                mMat[i].SetColor("_Color", Data.GetConfigColor(i));

            int rangeZ = Data.Range.y;
            int rangeX = Data.Range.x;


            for(int i = 0; i < count; ++i)
            {
                if (null == mTriangle[i])
                    mTriangle[i] = new List<int>();
                else
                    mTriangle[i].Clear();
            }

            for (int i = 0; i < rangeX; ++i)
                for (int j = 0; j < rangeZ; ++j)
                {
                    int t = Data.GridData[i, j];

                    AddQuad(mTriangle[t],i,j,rangeX,rangeZ);
                }

            for (int i = 0; i < count; ++i)
            {
                if (null == mTriangle[i])
                {
                    continue;
                }
                mMesh[i].vertices = mVertices;
                mMesh[i].triangles = mTriangle[i].ToArray();
            }
        }


        public void AddQuad(List<int> list, int i, int j, int rangeX, int rangeZ)
        {
            // //////////////
            // 2 ------- 34
            // |       /  |
            // |     /    |
            // |   /      |
            // 16 ------- 5
            // //////////////

            list.Add(i * (rangeZ + 1) + j);
            list.Add(i * (rangeZ + 1) + j + 1);
            list.Add((i + 1) * (rangeZ + 1) + j + 1);
            list.Add((i + 1) * (rangeZ + 1) + j + 1);
            list.Add((i + 1) * (rangeZ + 1) + j);
            list.Add(i * (rangeZ + 1) + j);
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
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
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
