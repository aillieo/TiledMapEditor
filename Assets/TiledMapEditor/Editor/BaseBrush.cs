using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TiledMapEditor
{

    public abstract class BaseBrush
    {

        protected Vector2Int[] mEffectedGrids = new Vector2Int[1];
        protected Vector3[] mCursorVertices = new Vector3[5];


        public abstract int BrushSizeMin { get; }
        public abstract int BrushSizeMax { get; }


        protected int mBrushSize = 1;
        public int BrushSize {
            get { return mBrushSize; }
            set
            {
                if (value != mBrushSize)
                {
                    mBrushSize = Mathf.Clamp(value, BrushSizeMin, BrushSizeMax);
                    OnBrushSizeChange();
                }
            }
        }


        protected int mBrushValue = 0;
        public int BrushValue
        {
            get { return mBrushValue; }
            set
            {
                if (value != mBrushValue)
                {
                    mBrushValue = value;
                    OnBrushValueChange();
                }
            }
        }


        public event Action<Vector2Int[],int> OnPaint;


        public void UpdateBrushState(Vector2 pos, bool paint)
        {
            int wx = Mathf.FloorToInt(pos.x);
            int wz = Mathf.FloorToInt(pos.y);
            Vector2Int intPos = new Vector2Int(wx,wz);

            FillCursorVertices(intPos);
            HandleUtility.Repaint();
            Handles.color = Color.white;
            Handles.DrawAAPolyLine(mCursorVertices);

            if (paint)
            {
                FillEffectedGrid(intPos);
                OnPaint?.Invoke(mEffectedGrids, mBrushValue);
            }
        }

        protected abstract void OnBrushSizeChange();

        protected abstract void OnBrushValueChange();

        protected abstract void FillEffectedGrid(Vector2Int pos);

        protected abstract void FillCursorVertices(Vector2Int pos);
        

    }

}
