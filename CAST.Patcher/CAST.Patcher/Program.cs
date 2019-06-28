using System;
using System.IO;
using Mono.Cecil;

namespace CAST.Patcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var targetDllName = "Assembly-CSharp.dll";

            var injectDllName = "CAST.dll";

            var outputDllName = "InjectedDLL.dll";

            var assemblyResolver = new DefaultAssemblyResolver();
            assemblyResolver.AddSearchDirectory(baseDirectory);

            var asm = AssemblyDefinition.ReadAssembly(
                Path.Combine(baseDirectory, targetDllName),
                new ReaderParameters
                {
                    AssemblyResolver = assemblyResolver,
                }
                );

            var asmInject = AssemblyDefinition.ReadAssembly(Path.Combine(baseDirectory, injectDllName));

            var patcher = new Patcher(asm ,asmInject);

            patcher.PatchSceneFirstDownload_Start();
            patcher.PatchGameUty_Init();
            patcher.PatchEditPoseData_Load();
            patcher.PatchEditMenuItemData_Load();
            patcher.PatchEditCategoryPartsData_Load();

            patcher.ExportDLL(baseDirectory, outputDllName);
        }
    }
}
