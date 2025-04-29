using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace CSFramework
{
    [Serializable]
    public class SpriteAtlasConfigItem
    {
        public string AtlasName;
        public string DirectoryPath;

        public bool Equal (SpriteAtlasConfigItem other)
        {
            return DirectoryPath == other.DirectoryPath && AtlasName == other.AtlasName;
        }
    }

    [Serializable]
    public class SpriteAtlasConfig : ScriptableObject
    {
        public List<SpriteAtlasConfigItem> ItemList = new List<SpriteAtlasConfigItem>();

        public bool CheckUpdateFromFile ()
        {
            var list = new List<SpriteAtlasConfigItem>(); 

            var extra_resource_path = Utils.GetEditorExtraResourcesPath("");
            var file_list = Utils.GetFileList(extra_resource_path, null, true);
            foreach (var file in file_list)
            {
                if (file.Extension == ".spriteatlas")
                {
                    list.Add(new SpriteAtlasConfigItem
                    {
                        DirectoryPath = Path.GetDirectoryName(Utils.GetRelativeExtraResourcesPath(file.FullName)),
                        AtlasName = file.Name.Replace(file.Extension, ""),
                    });
                }
            }

            // check dirty
            var dirty = false;
            if (list.Count == ItemList.Count)
            {
                for (var i = 0; i < list.Count; ++i)
                {
                    if (list[i].Equal(ItemList[i]))
                    {
                        dirty = true;
                        break;
                    }
                }
            }
            else
            {
                dirty = true;
            }

            if (dirty)
                ItemList = list;

            return dirty;
        }
    }
}