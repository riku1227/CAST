using System;
using System.IO;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CAST.Patcher
{
    class Patcher
    {
        private AssemblyDefinition target;
        private AssemblyDefinition inject;
        public Patcher(AssemblyDefinition targetAssembly, AssemblyDefinition injectAssembly)
        {
            this.target = targetAssembly;
            this.inject = injectAssembly;
        }

        /*
         * 対象: SceneFirstDownload#Start
         * 最初にMODManager.InitMODManager()を呼びStartGame()を呼び出す
         * StartGame()を呼ぶことでオフラインでも起動できるようになる
         * 
         * SceneFirstDownload#Start() {
         *     CAST.MODManager.InitMODManager();
         *     this.StartGame();
         * }
         */
        public void PatchSceneFirstDownload_Start()
        {
            var SceneFirstDownload = target.MainModule.GetType("SceneFirstDownload");
            var SceneFirstDownload_Start = SceneFirstDownload.Methods.First(x => x.Name == "Start");
            var SceneFirstDownload_StartGame = SceneFirstDownload.Methods.First(x => x.Name == "StartGame");

            var MODManager = inject.MainModule.GetType("CAST.MODManager");
            var MODManager_InitMODManager = MODManager.Methods.First(x => x.IsStatic && x.Name == "InitMODManager");
            var MODManager_InitMODManager_Ref = SceneFirstDownload_Start.Module.ImportReference(MODManager_InitMODManager);

            var ilProcessor = SceneFirstDownload_Start.Body.GetILProcessor();
            ilProcessor.Body.Instructions.Clear();
            ilProcessor.Body.Variables.Clear();
            ilProcessor.Emit(OpCodes.Call, MODManager_InitMODManager_Ref);
            ilProcessor.Emit(OpCodes.Ldarg_0);
            ilProcessor.Emit(OpCodes.Call, SceneFirstDownload_StartGame);
            ilProcessor.Emit(OpCodes.Ret);
        }

        /*
         * 対象: GameUty.Init
         * Assets読み込み処理をCAST側で行うようにする
         * 
         * GameUty.Init() {
         *     CAST.MODManager.InitGameUty();
         * }
         */
        public void PatchGameUty_Init()
        {
            var GameUty = target.MainModule.GetType("GameUty");
            var GameUty_Init = GameUty.Methods.First(x => x.Name == "Init");

            var MODManager = inject.MainModule.GetType("CAST.MODManager");
            var MODManager_InitGameUty = MODManager.Methods.First(x => x.IsStatic && x.Name == "InitGameUty");
            var MODManager_InitGameUty_Ref = GameUty_Init.Module.ImportReference(MODManager_InitGameUty);

            var ilProcessor = GameUty_Init.Body.GetILProcessor();
            ilProcessor.Body.Instructions.Clear();
            ilProcessor.Body.Variables.Clear();
            ilProcessor.Emit(OpCodes.Call, MODManager_InitGameUty_Ref);
            ilProcessor.Emit(OpCodes.Ret);
        }

        /*
         * 対象: EditPoseData.EditPoseData
         * PoseDataの読み込みが終わったあと MODManager.InitPoseData を呼び出す
         * 
         * EditPoseData.EditPoseData() {
         *     EditPoseData.Load();
         *     CAST.MODManager.InitPoseData();
         * }
         */
        public void PatchEditPoseData_EditPoseData()
        {
            var EditPoseData = target.MainModule.GetType("SCENE_EDIT.EditPoseData");
            var EditPoseData_EditPoseData = EditPoseData.Methods.First(x => x.Name == ".cctor");

            var MODManager = inject.MainModule.GetType("CAST.MODManager");
            var MODManager_InitPoseData = MODManager.Methods.First(x => x.IsStatic && x.Name == "InitPoseData");
            var MODManager_InitPoseData_Ref = EditPoseData_EditPoseData.Module.ImportReference(MODManager_InitPoseData);

            var ilProcessor = EditPoseData_EditPoseData.Body.GetILProcessor();
            ilProcessor.InsertBefore(ilProcessor.Body.Instructions[ilProcessor.Body.Instructions.Count - 1], ilProcessor.Create(OpCodes.Call, MODManager_InitPoseData_Ref));
        }

        public void ExportDLL(String baseDirectory, String outputDllName)
        {
            target.Write(Path.Combine(baseDirectory, outputDllName));
        }
    }
}
