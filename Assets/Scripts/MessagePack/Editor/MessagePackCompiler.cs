//==================================
// Execute mpc.exe from UnityEditor
// 
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace MessagePack
{
    public class MessagePackCompiler
    {
        static readonly string PathKey = "MessagePackCompilePath";

        // コンパイラのパスをリセットする
        [MenuItem("MessagePack/Reset Compiler Path", priority = 100)]
        static void ResetCompilerPath()
        {
            CompilePath = null;
        }

        // コード生成
        [MenuItem("MessagePack/Compile", priority = 0)]
        static void Compile()
        {
            if (!File.Exists(CompilePath))
            {
                // mpc.exe には存在しなかったため、選択ダイアログを表示します
                var path = EditorUtility.OpenFilePanel("Select mpc", "", "exe");

                if (String.IsNullOrEmpty(path)) return; // cancel

                // 選択したファイルは mpc.exe ではなかったので、エラーを表示する
                if (Path.GetFileName(path) != "mpc.exe")
                {
                    EditorUtility.DisplayDialog("エラー", "mpc.exe not selected!!", "OK");
                    return;
                }

                // 選択したパスを保存する
                CompilePath = path;
            }
            // ビルド開始
            var args = "";
            args += String.Format("-i \"{0}\" ", ProjectPath);
            args += String.Format("-o \"{0}\" ", Path.Combine(OutputPath, "MessagePackGenerated.cs"));
            System.Diagnostics.Process.Start(CompilePath, args);
            AssetDatabase.Refresh();
        }

        // mpc.exe のパス取得
        static string CompilePath
        {
            get
            {
                return EditorUserSettings.GetConfigValue(PathKey);
            }
            set
            {
                EditorUserSettings.SetConfigValue(PathKey, value);
            }
        }

        // Unityが生成したプロジェクトパス
        static string ProjectPath
        {
            get
            {
                return Directory.GetFiles(Path.GetDirectoryName(Application.dataPath), "*.CSharp.csproj").FirstOrDefault();
            }
        }

        // Resolvers を検索し、該当階層へ出力するようにします
        static string OutputPath
        {
            get
            {
                return Directory.GetDirectories(Application.dataPath, "Resolvers", SearchOption.AllDirectories).FirstOrDefault();
            }
        }
    }
}