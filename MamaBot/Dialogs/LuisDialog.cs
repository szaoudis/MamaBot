using MamaBot.ApiCommunication;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MamaBot.Dialogs
{
    public class LuisDialog : ComponentDialog
    {
        private readonly MamaLuisRecognizer _luisRecognizer;
        protected readonly ILogger Logger;

        // Dependency injection uses this constructor to instantiate MainDialog
        public LuisDialog(MamaLuisRecognizer luisRecognizer, ILogger<LuisDialog> logger, RecipeDialog recipeDialog, WeatherDialog weatherDialog)
            : base(nameof(LuisDialog))
        {
            _luisRecognizer = luisRecognizer;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(weatherDialog);
            AddDialog(recipeDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var messageText = stepContext.Options?.ToString() ?? $"What can I do for you {UserProfile.Name} ?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                // LUIS is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Luis not Configured") }, cancellationToken);
            }

         
            var Intent = await LuisApi.SendUserInputToLuis(stepContext.Result.ToString());
            switch (Intent)
            {
                case "Hello":


                    var HelloMessageText = "Hello there";
                    var getHelloMessage = MessageFactory.Text(HelloMessageText, HelloMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(getHelloMessage, cancellationToken);
                    break;


                case "Goodbye":
                    // We haven't implemented the GetWeatherDialog so we just display a TODO message.
                    var ByeMessageText = "Goodbye there";
                    var getByeMessage = MessageFactory.Text(ByeMessageText, ByeMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(getByeMessage, cancellationToken);
                    break;

                case "Jokes":
                 
                    var joke = await JokesApi.GetJoke();
                    var JokeMessageText = "Mmmm well..."+System.Environment.NewLine+joke ;
                    var getJokeMessage = MessageFactory.Text(JokeMessageText, JokeMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(getJokeMessage, cancellationToken);
                    break;

                case "UserLaughs":
                    Random rnd = new Random();
                    int rndv = rnd.Next(1, 3);
                    if (rndv == 1)
                    {
                        var userLaughText = "Yes, this was funny, I liked it too... 🤣🤣🤣🤣";
                        var getuserLaughMessage = MessageFactory.Text(userLaughText, userLaughText, InputHints.IgnoringInput);
                        await stepContext.Context.SendActivityAsync(getuserLaughMessage, cancellationToken);
                        break;
                    }
                    else
                    {
                        var userLaughText = "🤣🤣🤣🤣";
                        var getuserLaughMessage = MessageFactory.Text(userLaughText, userLaughText, InputHints.IgnoringInput);
                        await stepContext.Context.SendActivityAsync(getuserLaughMessage, cancellationToken);
                        break;
                    }

                case "Recipes":
                    return await stepContext.BeginDialogAsync(nameof(RecipeDialog), null, cancellationToken);

                case "weather":
                    return await stepContext.BeginDialogAsync(nameof(WeatherDialog), null, cancellationToken);

                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText = $"Sorry, I didn't get that. Please try asking in a different way";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            // Restart the main dialog with a different message the second time around
            var promptMessage = "";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
