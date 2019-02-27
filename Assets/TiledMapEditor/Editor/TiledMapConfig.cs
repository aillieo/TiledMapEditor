using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace AillieoUtils.TiledMapEditor
{

    [System.Serializable]
    public struct ConfigItem
    {
        public string displayName;
        public int brushValue;
        public Color displayColor;
    }


    public class TiledMapConfig : ScriptableObject
    {
        public ConfigItem[] Items;

        Dictionary<int, Color> colors = null;

        public Color GetColor(int enumValue)
        {
            if(null == colors)
            {
                colors = GetColorMappings();
            }
            return colors[enumValue];
        }


        public void SetColor(int enumValue, Color color)
        {
            if (null == colors)
            {
                colors = GetColorMappings();
            }
            colors[enumValue] = color;
        }

        public Dictionary<int, Color> GetColorMappings()
        {
            var colors = new Dictionary<int, Color>();
            foreach (var i in Items)
            {
                colors.Add(i.brushValue, i.displayColor);
            }
            return colors;
        }

        public void SaveColors()
        {
            for (int i = 0 ; i < Items.Length ; i ++)
            {
                Items[i].displayColor = colors[Items[i].brushValue];
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public void InitItems(int len = 5)
        {
            Items = new ConfigItem[len];
            for (int i = 0; i < len; ++i)
            {
                Items[i].displayName = string.Format("Tile_{0}",i);
                Items[i].brushValue = i;
                Items[i].displayColor = Color.Lerp(Color.red,Color.blue,(float)i /(len - 1));
            }
        }


    }
}
