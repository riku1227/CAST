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
         * 対象: EditPoseData.Load
         * PoseDataの読み込みが終わったあと MODManager.InitPoseData を呼び出す
         * 
         * EditPoseData.Load() {
         *     original code...
         *     CAST.MODManager.InitPoseData();
         * }
         */
        public void PatchEditPoseData_Load()
        {
            var EditPoseData = target.MainModule.GetType("SCENE_EDIT.EditPoseData");
            var EditPoseData_Load = EditPoseData.Methods.First(x => x.Name == "Load");

            var MODManager = inject.MainModule.GetType("CAST.MODManager");
            var MODManager_InitPoseData = MODManager.Methods.First(x => x.IsStatic && x.Name == "InitPoseData");
            var MODManager_InitPoseData_Ref = EditPoseData_Load.Module.ImportReference(MODManager_InitPoseData);

            var ilProcessor = EditPoseData_Load.Body.GetILProcessor();
            ilProcessor.Replace(ilProcessor.Body.Instructions.Last(), ilProcessor.Create(OpCodes.Call, MODManager_InitPoseData_Ref));
            ilProcessor.InsertAfter(ilProcessor.Body.Instructions.Last(), ilProcessor.Create(OpCodes.Ret));
        }

        public void PatchEditMenuItemData_Load()
        {
            var EditMenuItemData = target.MainModule.GetType("SCENE_EDIT.EditMenuItemData");
            var EditMenuItemData_Load = EditMenuItemData.Methods.First(x => x.Name == "Load");

            var MODManager = inject.MainModule.GetType("CAST.MODManager");
            var MODManager_InitPartsData = MODManager.Methods.First(x => x.IsStatic && x.Name == "InitPartsData");
            var MODManager_InitPartsData_Ref = EditMenuItemData_Load.Module.ImportReference(MODManager_InitPartsData);

            var ilProcessor = EditMenuItemData_Load.Body.GetILProcessor();
            ilProcessor.Replace(ilProcessor.Body.Instructions.Last(), ilProcessor.Create(OpCodes.Call, MODManager_InitPartsData_Ref));
            ilProcessor.InsertAfter(ilProcessor.Body.Instructions.Last(), ilProcessor.Create(OpCodes.Ret));
        }

        public void PatchEditCategoryPartsData_Load()
        {
            var EditCategoryPartsData = target.MainModule.GetType("SCENE_EDIT.EditCategoryPartsData");
            var EditCategoryPartsData_Load = EditCategoryPartsData.Methods.First(x => x.Name == "Load");

            var MODManager = inject.MainModule.GetType("CAST.MODManager");
            var MODManager_InitCategoryParts = MODManager.Methods.First(x => x.IsStatic && x.Name == "InitCategoryParts");
            var MODManager_InitCategoryParts_Ref = EditCategoryPartsData_Load.Module.ImportReference(MODManager_InitCategoryParts);

            var ilProcessor = EditCategoryPartsData_Load.Body.GetILProcessor();
            ilProcessor.Replace(ilProcessor.Body.Instructions.Last(), ilProcessor.Create(OpCodes.Call, MODManager_InitCategoryParts_Ref));
            ilProcessor.InsertAfter(ilProcessor.Body.Instructions.Last(), ilProcessor.Create(OpCodes.Ret));
        }

        public void PatchSceneEditBodyFigureWindow_Open()
        {
            var SceneEdit = target.MainModule.GetType("SCENE_EDIT.SceneEditBodyFigureWindow");
            var SceneEdit_SetUp = SceneEdit.Methods.First(x => x.Name == "Open");

            var ilProcessor = SceneEdit_SetUp.Body.GetILProcessor();

            ilProcessor.Replace(ilProcessor.Body.Instructions[158], ilProcessor.Create(OpCodes.Ldc_I4_0));
            ilProcessor.Remove(ilProcessor.Body.Instructions[159]);

            ilProcessor.Replace(ilProcessor.Body.Instructions[163], ilProcessor.Create(OpCodes.Ldc_I4_S, (sbyte)100));
            ilProcessor.Remove(ilProcessor.Body.Instructions[164]);
        }

        public void PatchMaid_SetProp()
        {
            var Maid = target.MainModule.GetType("Maid");
            var Maid_SetProp = Maid.Methods.First(x => {
                if(x.FullName == "System.Void Maid::SetProp(MPN,System.Int32,System.Boolean)")
                {
                    return true;
                }
                return false;
            });

            var ilProcessor = Maid_SetProp.Body.GetILProcessor();

            ilProcessor.Remove(ilProcessor.Body.Instructions[19]);
            ilProcessor.Remove(ilProcessor.Body.Instructions[19]);
            ilProcessor.Remove(ilProcessor.Body.Instructions[19]);

            ilProcessor.Remove(ilProcessor.Body.Instructions[23]);
            ilProcessor.Remove(ilProcessor.Body.Instructions[23]);
            ilProcessor.Remove(ilProcessor.Body.Instructions[23]);
        }

        public void ExportDLL(String baseDirectory, String outputDllName)
        {
            target.Write(Path.Combine(baseDirectory, outputDllName));
        }
    }
}
