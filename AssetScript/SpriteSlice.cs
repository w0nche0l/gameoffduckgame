using System;
using System.Collections.Generic;
using System.Linq; // for Cast()

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif


using UnityEngine;

public class SpriteSlice : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/FCO/Compute sprites")]
    static void ComputeSprites()
    {
        Debug.Log("ComputeSprites: start");

        int sliceWidth = 32;
        int sliceHeight = 32;

        string folder = "myfolder";

        Texture2D[] textures = Resources.LoadAll(folder, typeof(Texture2D)).Cast<Texture2D>().ToArray();
        Debug.Log("ComputeSprites: textures.Length: " + textures.Length);


        // must modify this to the files that you want to edit
        List<Func<string, bool>> allowlist = new List<Func<string, bool>>
        {
            (string s) => s == "testname.txt",
            (string s) => s.Contains("asdfasdf")
        };

        foreach (Texture2D texture in textures)
        {
            if (!allowlist.Any(f => f(texture.name)))
            {
                continue;
            }
            Debug.Log("ComputeSprites:name: " + texture.name);
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            //  constants
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Multiple;
            ti.spritePixelsPerUnit = 32;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;

            List<SpriteMetaData> newData = new List<SpriteMetaData>();


            //  https://answers.unity.com/questions/1113025/batch-operation-to-slice-sprites-in-editor.html
            //for (int i = 0; i < texture.width; i += sliceWidth)
            //{
            //    for (int j = texture.height; j > 0; j -= sliceHeight)
            //    {
            //        SpriteMetaData smd = new SpriteMetaData();
            //        smd.pivot = new Vector2(0.5f, 0.5f);
            //        smd.alignment = SpriteAlignment.Center;
            //        int rowNum = (texture.height - j) / sliceHeight;
            //        int colNum = i / sliceWidth;
            //        smd.name = texture.name + "_" + rowNum + "x" + colNum; // "name_1x7" for 2nd row & 8th column
            //        smd.rect = new Rect(i, j - sliceHeight, sliceWidth, sliceHeight);

            //        newData.Add(smd);
            //    }
            //}

            // https://forum.unity.com/threads/custom-texture-importer-for-automatically-generating-sprites-not-working.1022650/
            Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(
                 texture, Vector2.zero, new Vector2(sliceWidth, sliceHeight), Vector2.zero);
            for (int i = 0; i < rects.Length; i++)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.rect = rects[i];
                smd.pivot = new Vector2(0.5f, 0.5f);
                smd.alignment = (int)SpriteAlignment.Center;
                smd.name = texture.name + "_" + i; // name_41
                newData.Add(smd);
            }


            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate); // this takes time, approx. 3s per Asset
            Debug.Log("ComputeSprites: resource ok");
        }
        Debug.Log("ComputeSprites: done");
    }
#endif
}
