using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace VARLab.Velcro.Internal
{
    public class ExportVelcroPackage : Editor
    {
        private const string AssetsFolder = "Assets/VELCRO UI";
        private const string PackageFolder = "Assets/Package";
        private const string ExportPath = PackageFolder + "/velcro-assets.unitypackage";
        private const string PackageFileSearch = "*.unitypackage";

        [MenuItem("Tools/VELCRO/Export VELCRO Assets to .unitypackage file")]
        private static void ExportPackage()
        {
            string[] oldPackages = Directory.GetFiles(PackageFolder, PackageFileSearch);
            if (oldPackages != null && oldPackages.Length > 0)
            {
                foreach (string package in oldPackages)
                {
                    // Delete the old file
                    File.Delete(package);

                    // Delete the meta file too
                    string metaPath = $"{package}.meta";
                    if (File.Exists(metaPath))
                    {
                        File.Delete(metaPath);
                    }
                }
            }

            // Export the VELCRO UI folder ands its files to the propery directory
            AssetDatabase.ExportPackage(AssetsFolder, ExportPath, ExportPackageOptions.Recurse);

            Debug.Log($"Successfully exported VELCRO package to the Package folder\n{DateTime.Now}");
        }
    }
}