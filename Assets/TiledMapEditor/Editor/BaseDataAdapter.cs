using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace AillieoUtils.TiledMapEditor
{
    public abstract class BaseDataAdapter
    {

        public abstract string ConfigPath { get; }

        protected TiledMapConfig mConfig;
        public TiledMapConfig Config
        {
        get
            {
                if (null == mConfig)
                {
                    mConfig = LoadConfig();
                }
                return mConfig;
            }
        }

        public abstract Vector2Int Range { get;}
        public int[,] GridData { get; protected set; }

        TiledMapConfig LoadConfig()
        {
            TiledMapConfig config = null;
            string filePath = AssetPathToFilePath(ConfigPath);
            if (!File.Exists(filePath))
            {
                config = ScriptableObject.CreateInstance<TiledMapConfig>();
                config.InitItems();
                AssetDatabase.CreateAsset(config, ConfigPath);
            }
            else
            {
                config = AssetDatabase.LoadAssetAtPath<TiledMapConfig>(ConfigPath);
            }
            return config;
        }


        protected BaseDataAdapter()
        { }

        // create map data asset as default
        public abstract Object CreateDefaultData();

        // write map data to array GridData
        public abstract void LoadData(Object data);

        // GridData to map data
        public abstract void WriteData();


        public abstract void CleanUp();



        public void SetConfigColor(int brushValue, Color color)
        {
            Config.SetColor(brushValue, color);
        }

        public Color GetConfigColor(int brushValue)
        {
            return Config.GetColor(brushValue);
        }

        public int GetEnumCount()
        {
            return Config.Items.Length;
        }

        public string[] GetEnumNames()
        {
            return Config.Items.Select(i => i.displayName).ToArray();
        }

        public static string AssetPathToFilePath(string assetPath)
        {
            return string.Format("{0}/../{1}", Application.dataPath, assetPath);
        }


    }
}
