using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace VARLab.Velcro
{
    public class VelcroAssetsImporter : Editor
    {
        private static bool initialized = false;

        private const string PackageFolder = "Packages/com.varlab.velcro";
        private const string PackageFileSearch = "*.unitypackage";
        private const string ImportDebugMessage = "VELCRO Assets Refreshed";
        private const string VelcroPackageName = "com.varlab.velcro";

        // Initialize callbacks on load
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            if (!initialized)
            {
                Events.registeredPackages += AutoImportAssets;
                initialized = true;
            }
        }

        [MenuItem("Tools/VELCRO/Refresh VELCRO Assets")]
        private static void ImportAssets()
        {
            // Get the unity package file from the packages directory
            if (Directory.Exists(PackageFolder))
            {
                string[] filePaths = Directory.GetFiles(PackageFolder, PackageFileSearch);

                // Import the unity package file to the project
                if (filePaths != null && filePaths.Length > 0)
                {
                    string filePath = filePaths[0];
                    if (!string.IsNullOrWhiteSpace(filePath))
                    {
                        AssetDatabase.ImportPackage(filePath, false);
                        Debug.Log(ImportDebugMessage);
                    }
                }
            }
        }

        private static void AutoImportAssets(PackageRegistrationEventArgs args)
        {
            // Packages that were added
            foreach (PackageInfo package in args.added)
            {
                if (package.name == VelcroPackageName)
                {
                    ImportAssets();
                    return;
                }
            }

            // Packages that were updated
            foreach (PackageInfo package in args.changedTo)
            {
                if (package.name == VelcroPackageName)
                {
                    ImportAssets();
                    return;
                }
            }
        }
    }
}
