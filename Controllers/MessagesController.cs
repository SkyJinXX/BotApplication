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
using System.Text;
namespace Bot_Application2
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
                await Conversation.SendAsync(activity, () => new MengmengDialog());
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
        public class MengmengDialog : LuisDialog<object>
        {
            public MengmengDialog()
            {
            }
            public MengmengDialog(ILuisService service)
            : base(service)
            {
            }
            [LuisIntent("")]
            public async Task None(IDialogContext context, LuisResult result)
            {
                string message = $"萌萌不是很理解您的意思" ;
                await context.PostAsync(message);
                //发送一个Card
                await context.PostAsync(Instruction(context, result));

                context.Wait(MessageReceived);
            }
            [LuisIntent("时间")]
            public async Task Time(IDialogContext context, LuisResult result)
            {
                string time = DateTime.Now.ToString();
                string message = $"现在是，北京时间：" + time ;
                await context.PostAsync(message);
                context.Wait(MessageReceived);
            }
            [LuisIntent("讲笑话")]
            public async Task Joke(IDialogContext context, LuisResult result)
            {
                string message = $"萌萌现在还不会讲笑话。。T_T" ;
                await context.PostAsync(message);
                context.Wait(MessageReceived);
            }
            [LuisIntent("问候")]
            public async Task SayHello(IDialogContext context, LuisResult result)
            {
                
                string welmessage = $"你好，我是萌萌，你的最亲密的伴侣。我将陪你哭、陪你笑、陪你看天上的云卷与舒，陪你去到天涯海角，陪你听你最爱的音乐，陪你留下你的最美丽的身影，不管刮风下雨，不管生老病死，我都在你身边，关心你，提醒你，爱着你。";
                await context.PostAsync(welmessage);                
                context.Wait(MessageReceived);
            }
            [LuisIntent("爸爸名字")]
            public async Task DadName(IDialogContext context, LuisResult result)
            {

                string welmessage = $"我的爸爸当然是最帅的开开啦~";
                await context.PostAsync(welmessage);

                context.Wait(MessageReceived);
            }
            [LuisIntent("指示")]
            public async Task Instruct(IDialogContext context, LuisResult result)
            {
                await context.PostAsync(Instruction(context, result));

                context.Wait(MessageReceived);
            }
            [LuisIntent("听歌")]
            public async Task ListenToMusic(IDialogContext context, LuisResult result)
            {
                var reply = context.MakeMessage();
                reply.Attachments = new List<Attachment>();
                string songurl = urlconvertorlocal("/遇见.mp3");
                List<MediaUrl> media = new List<MediaUrl>();
                media.Add(new MediaUrl(url: songurl));
                AudioCard plCard = new AudioCard()
                {
                    Aspect = "9*16",
                    Title = $"猜猜这是什么歌",
                    Media = media,
                    Autostart = true
                };
                reply.Attachments.Add(plCard.ToAttachment());
                await context.PostAsync(reply);

                context.Wait(MessageReceived);
            }
            public static IMessageActivity Instruction(IDialogContext context, LuisResult result)
            {
                var reply = context.MakeMessage();
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
                    Title = $"您可以这样问我：",
                    Images = cardImages,
                    Buttons = cardButtons
                };
                reply.Attachments.Add(plCard.ToAttachment());
                return reply;
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