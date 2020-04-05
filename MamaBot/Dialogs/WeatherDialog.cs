using MamaBot.ApiCommunication;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MamaBot.Dialogs
{
    public class WeatherDialog : ComponentDialog
    {
        public WeatherDialog()
            : base(nameof(WeatherDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                WeatherQuestionStepAsync,
                WeatherResultStepAsync,

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> WeatherQuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var weatherDetails = stepContext.Options;



            return await stepContext.BeginDialogAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("So you came to the weather section! I am a weather guru. In which city do you want to make my predicts?") }, cancellationToken);

        }

        private async Task<DialogTurnResult> WeatherResultStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var Intent = stepContext.Result.ToString();
            var weather = await WeatherApi.GetWeather(Intent);
            var weatherOk = weather.ToString();
            await stepContext.Context.SendActivityAsync(weatherOk);


            return await stepContext.EndDialogAsync(null, cancellationToken);

        }
    }
}
