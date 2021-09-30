#if UNITY_ANDROID
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.darktable.utility {

    /// <summary>
    /// I always forget to enter the password for my debug key,
    /// which causes a build failure for Quest.
    /// This script automatically enters the password and alias name
    /// for Android keystore keys.
    /// (NOTE: Don't put the created json file in version control!)
    /// </summary>
    public static class AndroidKeystoreHelper {
        [Serializable]
        private sealed class KeystoreInfo {
            public string keystoreName;
            public string keystorePass;
            public string aliasName;
            public string aliasPass;
            public bool setAlias;
            public string WARNING = "DON'T PUT THIS FILE IN VERSION CONTROL!";
        }

        [InitializeOnLoadMethod]
        private static void Initialize() {
            string keystoreName = PlayerSettings.Android.keystoreName;

            if (string.IsNullOrEmpty(keystoreName)) {
                return;
            }

            string keystoreDirectory = Path.GetDirectoryName(keystoreName);

            if (string.IsNullOrEmpty(keystoreDirectory)) {
                return;
            }

            string keystorePrefix = Path.GetFileNameWithoutExtension(keystoreName);

            string jsonFilePath = Path.Combine(keystoreDirectory, $"{keystorePrefix}_settings.json");
            var keystoreInfo = new KeystoreInfo();

            if (!File.Exists(jsonFilePath)) {
                keystoreInfo.keystoreName = PlayerSettings.Android.keystoreName;
                keystoreInfo.aliasName = PlayerSettings.Android.keyaliasName;

                File.WriteAllText(jsonFilePath, EditorJsonUtility.ToJson(keystoreInfo, true));
            }
            else {
                EditorJsonUtility.FromJsonOverwrite(File.ReadAllText(jsonFilePath), keystoreInfo);
            }

            if (PlayerSettings.Android.keystoreName == keystoreInfo.keystoreName) {
                if (string.IsNullOrEmpty(PlayerSettings.Android.keystorePass)
                    && !string.IsNullOrEmpty(keystoreInfo.keystorePass)) {
                    Debug.Log("setting keystore password");
                    PlayerSettings.Android.keystorePass = keystoreInfo.keystorePass;
                }

                if (keystoreInfo.setAlias && PlayerSettings.Android.keyaliasName != keystoreInfo.aliasName) {
                    Debug.Log("setting key alias name");
                    PlayerSettings.Android.keyaliasName = keystoreInfo.aliasName;
                }

                if (PlayerSettings.Android.keyaliasName == keystoreInfo.aliasName) {
                    if (string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass)
                        && !string.IsNullOrEmpty(keystoreInfo.aliasPass)) {
                        Debug.Log("setting key alias password");
                        PlayerSettings.Android.keyaliasPass = keystoreInfo.aliasPass;
                    }
                }
            }
        }
    }
}
#endif
