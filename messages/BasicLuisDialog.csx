using System;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

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
         await context.PostAsync($" You said: {result.Query}"); 
        
        var path=Utils.GetAppSetting("OffPath");
        
        await context.PostAsync($" calling {path}");
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var x = await response.Content;
                 await context.PostAsync($" result : {x}"); 
            }
        
        context.Wait(MessageReceived);
    }
    
     [LuisIntent("On")]
    public async Task On(IDialogContext context, LuisResult result)
    {
      
        await context.PostAsync($" You said: {result.Query}"); 
        
        var path=Utils.GetAppSetting("OnPath");
        
        await context.PostAsync($" calling {path}"); 
             
        
        context.Wait(MessageReceived);
    }
}