using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Hynamick.SearchAnswer;
using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;

namespace Hynamick.QaBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var query = activity.Text;

                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                var actions = userData.GetProperty<IDictionary<string, string>>("RelatedQuestions");

                if (actions != null && actions.ContainsKey(query))
                {
                    query = actions[query];
                }

                var searchResponse = await QaDialog.Handler.SearchAsync(query, QaDialog.DefaultAnswerCount);
                var responseText = QaDialog.BuildText(query, searchResponse, out actions);

                userData.SetProperty<IDictionary<string, string>>("RelatedQuestions", actions);
                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                // return our reply to the user
                Activity reply = activity.CreateReply(responseText, "zh-cn");

                await connector.Conversations.ReplyToActivityAsync(reply);
                ////await Conversation.SendAsync(activity, () => new QaDialog());

            }
            if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                IConversationUpdateActivity update = activity;
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                {
                    var client = scope.Resolve<IConnectorClient>();
                    if (update.MembersAdded.Any())
                    {
                        var reply = activity.CreateReply();
                        foreach (var newMember in update.MembersAdded)
                        {
                            if (newMember.Id != activity.Recipient.Id)
                            {
                                reply.Text = $"请问您有什么需要帮助的，{newMember.Name}？";
                            }
                            else
                            {
                                reply.Text = $"需要帮忙么，{activity.From.Name}？";
                            }
                            reply.Locale = "zh-cn";
                            await client.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}