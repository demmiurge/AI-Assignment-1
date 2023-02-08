using UnityEngine;
using UnityEditor;

public class ScriptFromTemplateCreator
{
    private const string pathToTemplate_BT = "Assets/Scripts/BTs/Editor/Templates/BT_Template.cs.txt";
    private const string pathToTemplate_Action = "Assets/Scripts/BTs/Editor/Templates/Action_Template.cs.txt";
    private const string pathToTemplate_Condition = "Assets/Scripts/BTs/Editor/Templates/Condition_Template.cs.txt";
    private const string pathToTemplate_FSM = "Assets/Scripts/BTs/Editor/Templates/FSM_Template.cs.txt";
    private const string pathToTemplate_LinearSteering = "Assets/Scripts/BTs/Editor/Templates/LinearSteering_Template.cs.txt";

    [MenuItem(itemName: "Assets/Create/C# BT Script", isValidateFunction: false, priority: 22)]
    public static void CreateScriptFromTemplate_00()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(pathToTemplate_BT, "new BT.cs");
    }

    [MenuItem(itemName: "Assets/Create/C# ACTION Script", isValidateFunction: false, priority: 22)]
    public static void CreateScriptFromTemplate_01()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(pathToTemplate_Action, "new Action.cs");
    }

    [MenuItem(itemName: "Assets/Create/C# CONDITION Script", isValidateFunction: false, priority: 22)]
    public static void CreateScriptFromTemplate_02()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(pathToTemplate_Condition, "new Condition.cs");
    }

    [MenuItem(itemName: "Assets/Create/C# FSM Script", isValidateFunction: false, priority: 22)]
    public static void CreateScriptFromTemplate_03()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(pathToTemplate_FSM, "new FSM.cs");
    }

    [MenuItem(itemName: "Assets/Create/C# Linear Steering Script", isValidateFunction: false, priority: 22)]
    public static void CreateScriptFromTemplate_04()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(pathToTemplate_LinearSteering, "new Steering.cs");
    }
}
