using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TiledMapEditor
{

    public class DefaultBrush : BaseBrush
    {

        public override int BrushSizeMin { get { return 1; } }
        public override int BrushSizeMax { get { return 5; } }

        int mBackOffset;

        protected override void OnBrushSizeChange()
        {
            mEffectedGrids = new Vector2Int[mBrushSize * mBrushSize];
            mBackOffset = Mathf.FloorToInt(mBrushSize / 2);
        }

        protected override void OnBrushValueChange()
        { }

        protected override void FillEffectedGrid(Vector2Int pos)
        {
            for (int i = 0; i < mBrushSize; ++i)
                for (int j = 0; j < mBrushSize; ++j)
                {
                    mEffectedGrids[i * mBrushSize + j] = new Vector2Int(pos.x - mBackOffset + i, pos.y - mBackOffset + j);
                }
        }

        protected override void FillCursorVertices(Vector2Int pos)
        {

            //Debug.Log("Brush ----> " + pos.ToString());

            mCursorVertices[0].x = pos.x - mBackOffset;
            mCursorVertices[0].z = pos.y - mBackOffset;

            mCursorVertices[1].x = pos.x - mBackOffset;
            mCursorVertices[1].z = pos.y - mBackOffset + mBrushSize;

            mCursorVertices[2].x = pos.x - mBackOffset + mBrushSize;
            mCursorVertices[2].z = pos.y - mBackOffset + mBrushSize;

            mCursorVertices[3].x = pos.x - mBackOffset + mBrushSize;
            mCursorVertices[3].z = pos.y - mBackOffset;

            mCursorVertices[4].x = pos.x - mBackOffset;
            mCursorVertices[4].z = pos.y - mBackOffset;


        }

    }

}
