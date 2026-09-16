using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.AssetPackage;

namespace UnityUtility.Editor
{
    public static class GenUnityPackage
    {
        private const string k_MenuPath = "Tools/GenUnityPackage";
        private const string k_PackageFileName = "package.json";
        private const string k_OutputFolderName = "GeneratedPackages";

        [MenuItem(k_MenuPath)]
        private static void GeneratePackage()
        {
            // Find the first package.json in the project
            string[] packageJsonPaths =
                Directory.GetFiles(Application.dataPath, k_PackageFileName, SearchOption.AllDirectories);
            if (packageJsonPaths.Length == 0)
            {
                Debug.LogError($"{k_PackageFileName} not found in the project.");
                return;
            }

            // Use the first found package.json (you can modify to handle multiple)
            foreach (string packageJsonPath in packageJsonPaths)
            {
                if (Export(packageJsonPath, out string? outputPath))
                {
                    Debug.Log($"UnityPackage generated at: {outputPath}");
                }
                else
                {
                    Debug.Log($"failure to export: {packageJsonPaths}");
                }
            }
        }

        private static bool Export(string packageJsonPath, [NotNullWhen(true)] out string? outputPath)
        {
            string? packageFolderPath = Path.GetDirectoryName(packageJsonPath);
            if (string.IsNullOrEmpty(packageFolderPath))
            {
                Debug.LogError($"Failed to determine folder for {k_PackageFileName}.");
                outputPath = null;
                return false;
            }

            // Determine the relative path from the Unity project root (Assets folder)
            string? relativePath = GetRelativePathToAssets(packageFolderPath);
            if (string.IsNullOrEmpty(relativePath))
            {
                Debug.LogError($"Folder containing {k_PackageFileName} is not under the Assets directory.");
                outputPath = null;
                return false;
            }

            // Ensure output directory exists
            string projectRoot = Path.GetDirectoryName(Application.dataPath)!;
            string outputDir = Path.Combine(projectRoot, k_OutputFolderName);
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            // Determine package name from package.json (optional) – fallback to folder name
            string packageName = Path.GetFileName(packageFolderPath);
            try
            {
                var json = File.ReadAllText(packageJsonPath);
                var jsonObj = JsonUtility.FromJson<PackageJson>(json);
                if (!string.IsNullOrEmpty(jsonObj.name))
                    packageName = jsonObj.name.Replace('/', '_');
            }
            catch
            {
                // ignore parsing errors, keep folder name
            }

            outputPath = Path.Combine(outputDir, $"{packageName}.unitypackage");

            // Export the package
            var exportParam = new ExportPackageParameters(relativePath, outputPath, flags: ExportPackageOptions.Recurse);
            Package.Export(exportParam);
            return true;
        }

        private static string? GetRelativePathToAssets(string fullPath)
        {
            // Unity's AssetDatabase works with paths relative to the project root (e.g., "Assets/YourFolder")
            string assetsRoot = Application.dataPath; // ends with ".../Assets"
            fullPath = fullPath.Replace('\\', '/');
            if (!fullPath.StartsWith(assetsRoot))
                return null;
            return fullPath[(assetsRoot.Length - "Assets".Length)..].Replace('\\', '/');
        }

        // Simple DTO for parsing package.json name field
        [System.Serializable]
        private class PackageJson
        {
            public string name = null!;
        }
    }
}