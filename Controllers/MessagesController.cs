using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Luis;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Scorables;

namespace Bot_Application2
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        const string baseurl = "http://kkss.chinacloudsites.cn/";
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            await Conversation.SendAsync(activity, () => new MengmengDialog());
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
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
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

        [LuisModel("bbd87f18-ed4a-461d-809e-c93dc5d664ab", "7fddbeb7f5e14dc094699fef53434a72")]
        [Serializable]
        public class MengmengDialog : DispatchDialog<object>
        {
            [MethodBind]
            [ScorableGroup(0)]
            public async Task ConversationUpdateHandler(IDialogContext context, IConversationUpdateActivity update)
            {
                var result = update as LuisResult;
                var reply = context.MakeMessage();
                if (update.MembersAdded.Any())
                {
                    var newMembers = update.MembersAdded?.Where(t => t.Id != update.Recipient.Id);
                    foreach (var newMember in newMembers)
                    {
                        await context.PostAsync("萌萌为您服务");
                        //await context.PostAsync(Instruction(context, result));
                    }
                }
            }
            [LuisIntent("None")]
            [ScorableGroup(1)]
            public async Task None(IDialogContext context, LuisResult result)
            {
                string message = $"老娘听不懂，Can you speak Chinese?" ;
                await context.PostAsync(message);
                System.Threading.Thread.Sleep(2000);
                //发送一个Card
                await context.PostAsync(Instruction(context, result));                
            }
            [LuisIntent("时间")]
            [ScorableGroup(1)]
            public async Task Time(IDialogContext context, LuisResult result)
            {
                string time = DateTime.Now.AddHours(8).ToString();
                string message = $"现在是，北京时间：" + time ;
                await context.PostAsync(message);

            }
            [LuisIntent("讲笑话")]
            [ScorableGroup(1)]
            public async Task Joke(IDialogContext context, LuisResult result)
            {
                string message = $"从前有一个人...." ;
                await context.PostAsync(message);
                System.Threading.Thread.Sleep(2000);
                message = "他...";
                await context.PostAsync(message);
                System.Threading.Thread.Sleep(2000);
                message = "哈哈哈哈哈哈，笑死老娘了";
                await context.PostAsync(message);
            } 
            [LuisIntent("问候")]
            [ScorableGroup(1)]
            public async Task SayHello(IDialogContext context, LuisResult result)
            {
                
                string welmessage = $"你好，我是萌萌";
                await context.PostAsync(welmessage);                
            }
            [LuisIntent("爸爸名字")]
            [ScorableGroup(1)]
            public async Task DadName(IDialogContext context, LuisResult result)
            {

                string welmessage = $"当然是最帅的开开啦";
                await context.PostAsync(welmessage);
            }
            [LuisIntent("指示")]
            [ScorableGroup(1)]
            public async Task Instruct(IDialogContext context, LuisResult result)
            {
                await context.PostAsync(Instruction(context, result));

            }
            [LuisIntent("听歌")]
            [ScorableGroup(1)]
            public async Task ListenToMusic(IDialogContext context, LuisResult result)
            {
                var reply = context.MakeMessage();
                reply.Attachments = new List<Attachment>();

                string songurl = baseurl + "Music/jm.mp3";
                List<MediaUrl> media = new List<MediaUrl>();
                media.Add(new MediaUrl(url: songurl));
                AudioCard plCard = new AudioCard()
                {
                    Aspect = "25*25",
                    Title = $"猜猜这是什么歌呢?",
                    Media = media,
                    Autostart = true
                    
                };
                reply.Attachments.Add(plCard.ToAttachment());
                await context.PostAsync(reply);

                context.Call(new GuessDialog(), ResumeAfterGuessDialog);                
            }
            private async Task ResumeAfterGuessDialog(IDialogContext context, IAwaitable<IMessageActivity> result)
            {
                try
                {
                    var message = await result;
                    if (message.Text == "九妹")
                    {
                        await context.PostAsync($"小伙子有见识啊！");
                    }
                    else
                    {
                        await context.PostAsync($"这都不知道吗？？");
                    }
                }
                catch (Exception ex)
                {
                    await context.PostAsync($"Failed with message: {ex.Message}");
                }
                finally
                {
                }
            }
            public static IMessageActivity Instruction(IDialogContext context, LuisResult result)
            {
                var reply =  context.MakeMessage();
                reply.Attachments = new List<Attachment>();
                string[] meetingRoonImage = { "http://cdn.duitang.com/uploads/item/201610/01/20161001105121_nwvSs.jpeg" };
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: meetingRoonImage[0]));
                List<CardAction> cardButtons = new List<CardAction>();
                CardAction Button1 = new CardAction()
                {
                    Value = "你好萌萌~",
                    Type = "imBack",
                    Title = $"你好萌萌~"
                };
                cardButtons.Add(Button1);
                CardAction plButton = new CardAction()
                {
                    Value = "现在是几点钟？",
                    Type = "imBack",
                    Title = $"现在是几点钟啦~"
                };
                cardButtons.Add(plButton);
                CardAction Button2 = new CardAction()
                {
                    Value = "给我讲个笑话",
                    Type = "imBack",
                    Title = $"给我讲个笑话吧~"
                };
                cardButtons.Add(Button2);
                CardAction Button3 = new CardAction()
                {
                    Value = "你的爸爸是谁呀？",
                    Type = "imBack",
                    Title = $"你的爸爸是谁呀？"
                };
                cardButtons.Add(Button3);
                CardAction Button4 = new CardAction()
                {
                    Value = "听歌",
                    Type = "imBack",
                    Title = $"听歌"
                };
                cardButtons.Add(Button4);
                HeroCard plCard = new HeroCard()
                {
                    Title = $"其实你可以这样问我：",
                    Images = cardImages,
                    Buttons = cardButtons
                };
                reply.Attachments.Add(plCard.ToAttachment());
                return reply;
            }
            [Serializable]
            class GuessDialog : IDialog <IMessageActivity>
            {
                public Task StartAsync(IDialogContext context)
                {
                    context.Wait(this.None);
                    return Task.CompletedTask;
                }
                public async Task None(IDialogContext context, IAwaitable<IMessageActivity> result)
                {
                    var message = await result as Activity;
                    
                    context.Wait(None);
                    context.Done(message);
                }
            }
            private string urlconvertorlocal(string imagesurl1)
            {
                //获取程序根目录  
                string tmpRootDir = System.Web.HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());
                //转换成服务器绝对路径  
                string imagesurl2 = tmpRootDir + imagesurl1.Replace(@"/", @"/");
                return imagesurl2;
            }
        }
    }
}