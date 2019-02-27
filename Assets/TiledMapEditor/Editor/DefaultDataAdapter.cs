using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace AillieoUtils.TiledMapEditor
{
    public class DefaultDataAdapter : BaseDataAdapter
    {

        public override string ConfigPath { get {
                return "Assets/TiledMapEditor/@DefaultDataConfig.asset";
            } }

        int CharToInt32(char c)
        {
            return c - 48;
        }
        char Int32ToChar(int i)
        {
            return (char)(i + 48);
        }


        string assetFilePath;

        public override Object CreateDefaultData()
        {
            string tmpAssetPath = "Assets/TiledMapEditor/defaultData.txt";
            assetFilePath = AssetPathToFilePath(tmpAssetPath);

            File.WriteAllText(assetFilePath, new string('0',81));
            AssetDatabase.Refresh();

            return AssetDatabase.LoadAssetAtPath<Object>(tmpAssetPath);
        }

        public override void LoadData(Object data)
        {
            string assetPath = AssetDatabase.GetAssetPath(data);
            assetFilePath = AssetPathToFilePath(assetPath);

            string textData = (data as TextAsset).text;
            textData = textData.Trim('\n').Trim('\r');

            var len = textData.Length;
            var size = Mathf.FloorToInt(Mathf.Sqrt(len));

            Range = new Vector2Int(size, size);
            GridData = new int[Range.x, Range.y];

            for (int i = 0; i < Range.x; ++i)
                for (int j = 0; j < Range.y; ++j)
                {
                    int value = CharToInt32(textData[Range.x * j + i]);
                    GridData[i, j] = value;
                }
        }


        public override void WriteData()
        {
            char[] textChars = new char[Range.x * Range.y];
            for (int i = 0; i < Range.x; ++i)
                for (int j = 0; j < Range.y; ++j)
                {
                    textChars[Range.x * j + i] = Int32ToChar(GridData[i, j]);
                }
            string textData = new string(textChars);
            Debug.Log("textData = " + textData);
            File.WriteAllText(assetFilePath, textData);
        }


        public override void CleanUp()
        { }

    }
}
