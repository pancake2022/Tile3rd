using System.Collections.Generic;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using ExcelDataReader;

namespace CSFramework
{
    public static class Exporter
    {
        public static void ExportAll ()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // ExportConfig("res/Config", "out/ConfigCS/Config", "out/ConfigJson/Config");
            ExportStorage("res/Storage/Storage.xlsx", "out/Storage");
        }

        public static void ExportConfig (string src_root, string dst_cs_root, string dst_json_root)
        {
            Utils.MakeSureDirectoryEmpty(dst_json_root);
            Utils.MakeSureDirectoryEmpty(dst_cs_root);

            var full_dst_json_root = Path.GetFullPath(dst_json_root);

            var file_list = Utils.GetFileList(src_root);
            var excel_extension_set = new HashSet<string>{ ".xlsx", ".xls" };

            var template_content = File.ReadAllText("src/Config/ConfigTemplate.template");
            var config_path_list = new List<string>();
            foreach (var file in file_list)
            {
                if (excel_extension_set.Contains(file.Extension.ToLower()) && file.Name[0] != '~')
                {
                    // export_file_to_json(file.FullName, template_content, config_path_list);
                    var exporter = new ConfigExporter();
                    exporter.Read(file.FullName, template_content);

                    var out_json_path = file.FullName.Replace(src_root, dst_json_root);
                    out_json_path = Path.ChangeExtension(out_json_path, ".json");

                    var out_cs_path = file.FullName.Replace(src_root, dst_cs_root);
                    
                    exporter.Export(out_json_path, Directory.GetParent(out_cs_path).FullName);

                    config_path_list.Add("Config/" + Path.GetRelativePath(full_dst_json_root, out_json_path.Replace(".json", "")));
                }
            }

            var registry_template_content = File.ReadAllText("src/Config/ConfigRegistryTemplate.template");
            var config_path_list_content = "";
            var registry_class_template = Utils.FilterContent(registry_template_content, "ConfigStart", "ConfigEnd", true);
            var config_path_template = Utils.FilterContent(registry_template_content, "ConfigStart", "ConfigEnd");
            foreach (var config_path in config_path_list)
            {
                var class_name = string.Format("{0}Config", Path.GetFileName(config_path));
                config_path_list_content += config_path_template.Replace("${ConfigPath}", config_path).Replace("${ClassName}", class_name);
            }
            var registry_class_content = registry_class_template.Replace("${ConfigList}", config_path_list_content);
            var registry_path = Path.Join(dst_cs_root, "ConfigRegistry.cs");
            Utils.SaveFile(registry_path, registry_class_content);
        }

        public static void ExportStorage (string src_file_path, string dst_root)
        {
            Utils.MakeSureDirectoryEmpty(dst_root);

            var exporter = new StorageExporter();
            // var storage_file_path = string.Format("Assets/{0}/Storage/Storage.xlsx", Environment.RawResourcesPath);
            var storage_template_path = "src/Storage/StorageTemplate.template";
            var storage_registry_template_path = "src/Storage/StorageRegistryTemplate.template";
            exporter.Read(src_file_path, storage_template_path, storage_registry_template_path);
            exporter.Export(dst_root);
        }
    }
}