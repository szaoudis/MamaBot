
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace MamaBot.Bots
{

        public class MamaBot<T> : ActivityHandler where T : Dialog
        {
            protected readonly Dialog Dialog;
            protected readonly BotState ConversationState;
            protected readonly BotState UserState;
            protected readonly ILogger Logger;

            public MamaBot(ConversationState conversationState, UserState userState, T dialog, ILogger<MamaBot<T>> logger)
            {
                ConversationState = conversationState;
                UserState = userState;
                Dialog = dialog;
                Logger = logger;
            }

            public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
            {
                await base.OnTurnAsync(turnContext, cancellationToken);

                // Save any state changes that might have occured during the turn.
                await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
                await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
             

        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome to MamaBot!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
            {
                Logger.LogInformation("Running dialog with Message Activity.");

                // Run the Dialog with the new message Activity.
                await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
            }
        }
    }
