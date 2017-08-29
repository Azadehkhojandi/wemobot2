using System;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;


using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

[Serializable]
public class ContextConstants
{
    public const string OffPath = "OffPath";

    public const string OnPath = "OnPath";
}

// For more information about this template visit http://aka.ms/azurebots-csharp-luis
[Serializable]
public class BasicLuisDialog : LuisDialog<object>
{

    public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(Utils.GetAppSetting("LuisAppId"), Utils.GetAppSetting("LuisAPIKey"))))
   
    {
    }

    [LuisIntent("None")]
    public async Task NoneIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"You have reached the none intent. You said: {result.Query}"); //
        context.Wait(MessageReceived);
    }

    // Go to https://luis.ai and create a new intent, then train/publish your luis app.
    // Finally replace "MyIntent" with the name of your newly created intent in the following handler
    [LuisIntent("Off")]
    public async Task Off(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"You said: {result.Query}");

        string path = "";
        if (!context.UserData.TryGetValue(ContextConstants.OffPath, out path))
        {
            PromptDialog.Text(context, this.ResumeAfterPromptOffPath, "What\'s the off Url?");
            return;

        }
        else
        {
            await RunCommand(context, path);

        }



        context.Wait(MessageReceived);
    }

    [LuisIntent("On")]
    public async Task On(IDialogContext context, LuisResult result)
    {

        await context.PostAsync($"You said: {result.Query}");

        string path = "";
        if (!context.UserData.TryGetValue(ContextConstants.OnPath, out path))
        {
        
            PromptDialog.Text(context, this.ResumeAfterPromptOnPath, "What\'s the on Url?");
            return;

        }
        else
        {
            await context.PostAsync($"run command {path}");
            await RunCommand(context, path);
        }


        context.Wait(MessageReceived);
    }

    private async Task ResumeAfterPromptOffPath(IDialogContext context, IAwaitable<string> result)
    {

        var url = await result;


        context.UserData.SetValue(ContextConstants.OffPath, url);
        await context.PostAsync($"run command {url}");
        await RunCommand(context, url);

        context.Wait(MessageReceived);
    }

    private async Task ResumeAfterPromptOnPath(IDialogContext context, IAwaitable<string> result)
    {

        var url = await result;


        context.UserData.SetValue(ContextConstants.OnPath, url);
        await context.PostAsync($"run command {url}");
        await RunCommand(context, url);

        context.Wait(MessageReceived);
    }
    private async Task RunCommand(IDialogContext context, string path)
    {
        try
        {
             await context.PostAsync($"calling {path}");
        var client = new HttpClient();
        var response = await client.GetAsync(path);
        if (response.IsSuccessStatusCode)
        {
            var x = await response.Content.ReadAsStringAsync();
            await context.PostAsync($"{x}");
        }
        else
        {
            await context.PostAsync($"something went wrong please try again");
        }
        }
        catch (System.Exception ex)
        {
            
             await context.PostAsync($"{ex.Message}");
        }
       

    }
}