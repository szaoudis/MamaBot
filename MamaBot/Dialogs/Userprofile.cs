// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MamaBot;
using MamaBot.ApiCommunication;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace  MamaBot.Dialogs
{
    public class UserProfileDialog : ComponentDialog
    {
        
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;

        public UserProfileDialog(UserState userState, LuisDialog luisDialog)
            : base(nameof(UserProfileDialog))
        {
            _userProfileAccessor = userState.CreateProperty<UserProfile>("UserProfile");
          
            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {

                FirstNameAsync,
                LastNameAsync,
                AgeStepAsync,
                JobStepAsync,
                CountryStepAsync,
                TimeZoneStepAsync,
                ColorStepAsync,
                GenderStepAsync,
                SummaryStepAsync,

            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new TextPrompt("country", CountryPromptValidatorAsync));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(new NumberPrompt<int>("age", AgePromptValidatorAsync));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(luisDialog);

           


            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private static async Task<DialogTurnResult> FirstNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the user's response is received.

            return await stepContext.BeginDialogAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What is your name?") }, cancellationToken);
        }

        private static async Task<DialogTurnResult> LastNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["name"] = UppercaseFirst((string)stepContext.Result);



            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What is your last name?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> AgeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["lastname"] = UppercaseFirst((string)stepContext.Result);

            // We can send messages to the user at any point in the WaterfallStep.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks {stepContext.Values["name"]}."), cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter your age."),
                RetryPrompt = MessageFactory.Text("The value entered must be greater than 0 and less than 150."),
            };

            return await stepContext.PromptAsync("age", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> JobStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["age"] = (int)stepContext.Result;
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the user's response is received.
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What is your occupation?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Employee", "Student", "Unemployee" }),
                }, cancellationToken);

        }


        private async Task<DialogTurnResult> CountryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["occupation"] = ((FoundChoice)stepContext.Result).Value; ;
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Where are you from?"),
                RetryPrompt = MessageFactory.Text("This is not a valid country name...please type it again"),
            };

            return await stepContext.PromptAsync("country", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> TimeZoneStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["country"] = CountryInfo.CountryName;
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please select your timezone"),
                    Choices = ChoiceFactory.ToChoices(CountryInfo.Timezones),
                }, cancellationToken);


        }
        private async Task<DialogTurnResult> ColorStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["timezone"] = ((FoundChoice)stepContext.Result).Value;
            var usertimezone = stepContext.Values["timezone"].ToString();
            string userTime = usertimezone.Remove(0,3);
            string Timeoperator = userTime.Remove(1,5);
            string UtcTimeOfset = userTime.Remove(0,1);
            string UtcTime = DateTime.UtcNow.ToUniversalTime().TimeOfDay.ToString();
            int utctimelength = UtcTime.Length;
            string UtcTime1 = UtcTime.Substring(0, 8);
            DateTime T1 = DateTime.Parse(UtcTimeOfset);
            DateTime T2 = DateTime.Parse(UtcTime1);
            if(Timeoperator == "+")
            {
                DateTime d3 = T1.Add(T2.TimeOfDay);
                UserCurrentTime.UserTime = d3;

            }
            else
            {
                DateTime d3 = T2.Subtract(T1.TimeOfDay);
                UserCurrentTime.UserTime = d3;
            }

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What is your favorite colour?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Red", "Pink", "Purple", "Blue", "Tirquoise", "Green", "Yellow", "Orange", "Brown", "White", "Grey", "Black" }),
                }, cancellationToken);


        }
        private async Task<DialogTurnResult> GenderStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["color"] = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What is your gender?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Male", "Female" }),
                }, cancellationToken);



        }
        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["gender"] = ((FoundChoice)stepContext.Result).Value;
            switch (stepContext.Values["gender"])
            {
                case "Male":
                    await stepContext.Context.SendActivityAsync($"Well, my son, your name is {stepContext.Values["name"]}, your surname is {stepContext.Values["lastname"]}, you are from {stepContext.Values["country"]}, and your favorite color is {stepContext.Values["color"]}. Your current time is {UserCurrentTime.UserTime.TimeOfDay.ToString()}");
                    // return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                    break;
                default:
                    await stepContext.Context.SendActivityAsync($"Well, my girl, your name is {stepContext.Values["name"]}, your surname is {stepContext.Values["lastname"]}, you are from {stepContext.Values["country"]}, and your favorite color is {stepContext.Values["color"]}. Your current time is {UserCurrentTime.UserTime.TimeOfDay.ToString()}");
                    //return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                    break;
            }
            UserProfile.Name = stepContext.Values["name"].ToString();
            UserProfile.LastName = stepContext.Values["lastname"].ToString();
            UserProfile.Age = Int32.Parse(stepContext.Values["age"].ToString());
            UserProfile.Job = stepContext.Values["occupation"].ToString();
            UserProfile.Country = CountryInfo.CountryName;
            UserProfile.Color = stepContext.Values["color"].ToString();
            UserProfile.Gender = stepContext.Values["gender"].ToString();
            CounterCountry.counter = 0;


 
            return await stepContext.BeginDialogAsync(nameof(LuisDialog), null, cancellationToken);


        }

        private static Task<bool> CountryPromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {

                string countryname = promptContext.Recognized.Value;
                var ApiReply = ApiCommunication.ApiCommunication.GetCountryName(countryname);
                var flag = ApiReply.Result;
                
                // This condition is our validation rule. You can also change the value at this point.
                return Task.FromResult(flag == "1");
            
        }
        private static Task<bool> AgePromptValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            // This condition is our validation rule. You can also change the value at this point.
            return Task.FromResult(promptContext.Recognized.Succeeded && promptContext.Recognized.Value > 0 && promptContext.Recognized.Value < 150);
        }
        static class CounterCountry
        {
            public static int counter= 0;
        }
        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }

    }
   
}