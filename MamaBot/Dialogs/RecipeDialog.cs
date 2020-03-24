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
    public class RecipeDialog  : ComponentDialog
    {
        

        public RecipeDialog()
            : base(nameof(RecipeDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
      
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ItemOfRecipeStepAsync,
                RecipesResultStepAsync,
              
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ItemOfRecipeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = stepContext.Options;

         

           return await stepContext.BeginDialogAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Tell me what kind of food do you want to cook...)") }, cancellationToken);
           
        }

        private async Task<DialogTurnResult> RecipesResultStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var cookingDetails = stepContext.Result.ToString();
            var LuisRecipeIntent = await LuisApi.SendUserInputToLuis(cookingDetails);
            
            var Recipes = await RecipeApi.GetRecipe(cookingDetails);
            return await stepContext.EndDialogAsync(null, cancellationToken);
            
        }

        
    }
}

