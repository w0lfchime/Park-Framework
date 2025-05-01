using System;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{

    void Awake()
    {
        RegisterServices();
        RegisterCommands();

        //lets add a player for early testing 


    }

    void RegisterServices()
    {
        
        //audio
        //camera
        //input
        //scene
        //time
        //ui

        ServiceLocator.RegisterService<AudioManager>(GetComponent<AudioManager>());
        ServiceLocator.RegisterService<CameraManager>(GetComponent<CameraManager>());
        //ServiceLocator.RegisterService<InputManager>(GetComponent<InputManager>());

        ServiceLocator.RegisterService<TimeManager>(GetComponent<TimeManager>());
        ServiceLocator.RegisterService<UIManager>(GetComponent<UIManager>());
        ServiceLocator.RegisterService<VectorRenderManager>(GetComponent<VectorRenderManager>());
    }

    void RegisterCommands()
    {
        CommandHandler.RegisterCommand("echo", args =>
        {
            if (args.Length == 0)
            {
                LogCore.Log("CommandMeta", "Usage: echo [message]");
            }
            else
            {
                // Join all arguments to recreate the user's input message
                string message = string.Join(" ", args);
                LogCore.Log("CommandMeta", message);
            }
        });

        //CommandHandler.RegisterCommand("log", args =>
        //{
        //    if (args.Length < 2)
        //    {
        //        LogCore.Log("Usage: /log [enable|disable] [category]");
        //        return;
        //    }

        //    string action = args[0].ToLower();
        //    string categoryName = args[1];
        //    if (!Enum.TryParse(typeof(LogCategory), categoryName, true, out var categoryEnum))
        //    {
        //        LogCore.LogError($"Invalid log category: {categoryName}");
        //        return;
        //    }

        //    var category = (LogCategory)categoryEnum;

        //    if (action == "enable")
        //    {
        //        LogCore.EnableCategory(category);
        //        LogCore.Log($"{category} logging enabled.", LogCategory.Response);
        //    }
        //    else if (action == "disable")
        //    {
        //        LogCore.DisableCategory(category);
        //        LogCore.Log($"{category} logging disabled.", LogCategory.Response);
        //    }
        //    else
        //    {
        //        LogCore.LogError("Unknown action. Use 'enable' or 'disable'.");
        //    }
        //});
    }
}
