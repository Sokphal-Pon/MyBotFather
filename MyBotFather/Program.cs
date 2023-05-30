using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


var botClient = new TelegramBotClient("6145398330:AAHutVRme6eJz98tug46BCwBhZdOLr33-aw");


//Time
int year;
int month;
int day;
int hour;
int minute;
int second;

//Messages and user info
long chatId = 0;
string messageText;
int messageId;
string firstName;
string lastName;
long id;
Message sentMessage;

//poll info
int pollId = 0;

//Read time and save variables
year = int.Parse(DateTime.UtcNow.Year.ToString());
month = int.Parse(DateTime.UtcNow.Month.ToString());
day = int.Parse(DateTime.UtcNow.Day.ToString());
hour = int.Parse(DateTime.UtcNow.Hour.ToString());
minute = int.Parse(DateTime.UtcNow.Minute.ToString());
second = int.Parse(DateTime.UtcNow.Second.ToString());
Console.WriteLine("Data: " + year + "/" + month + "/" + day);
Console.WriteLine("Time: " + hour + ":" + minute + ":" + second);

//cts token
using var cts = new CancellationTokenSource();

// Bot StartReceiving, does not block the caller thread. Receiving is done on the ThreadPool.
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { } // receive all update types
};
botClient.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token);

var me = await botClient.GetMeAsync();

//write on console a hello message by bot 
Console.WriteLine($"\nHello! I'm {me.Username} and i'm your Bot!");

// Send cancellation request to stop bot and close console
Console.ReadKey();
cts.Cancel();

//Answer of the bot to the input.
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type != UpdateType.Message)
        return;
    // Only process text messages
    if (update.Message!.Type != MessageType.Text)
        return;

    //set variables
    chatId = update.Message.Chat.Id;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    messageText = update.Message.Text;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
    messageId = update.Message.MessageId;
    firstName = update.Message.From.FirstName;
    lastName = update.Message.From.LastName;
    id = update.Message.From.Id;
    year = update.Message.Date.Year;
    month = update.Message.Date.Month;
    day = update.Message.Date.Day;
    hour = update.Message.Date.Hour;
    minute = update.Message.Date.Minute;
    second = update.Message.Date.Second;

    //when receive a message show data and time on console.
    Console.WriteLine("\nData message --> " + year + "/" + month + "/" + day + " - " + hour + ":" + minute + ":" + second);
    //show the message, the chat id and the user info on console.
    Console.WriteLine($"Received a '{messageText}' message in chat {chatId} from user:\n" + firstName + " - " + lastName + " - " + " 5873853");

    //set text all lowercase
    messageText = messageText.ToLower();

    // I insert this if to solve a bug, if you haven't problems you can removed it.
    if (messageText != null && int.Parse(day.ToString()) >= day && int.Parse(hour.ToString()) >= hour && int.Parse(minute.ToString()) >= minute && int.Parse(second.ToString()) >= second - 10)
    {
        //For every message, check if the user is a member of the group/channel.
        // IF yes..skip the verification
        // IF no ..before to continue, ask to user to join the chat you want.
        var getchatmember = await botClient.GetChatMemberAsync(/*ID or NAME of the chat*/"@zetalvx",/*user id*/ id);
        var getchatmember2 = await botClient.GetChatMemberAsync(/*ID or NAME of the chat*/"@tutorialbotprogramming",/*user id*/ id);

        //Using the string of the "Status" command, check if is member 
        if (getchatmember.Status.ToString() == "Left" || getchatmember.Status.ToString() == null || getchatmember.Status.ToString() == "null" || getchatmember.Status.ToString() == "" || getchatmember2.Status.ToString() == "Left" || getchatmember2.Status.ToString() == null || getchatmember2.Status.ToString() == "null" || getchatmember2.Status.ToString() == "")
        {
            // create the "buttons" with the URL of the channel to join.
            InlineKeyboardMarkup inlineKeyboard = new(new[]
                  {
                    //First row. You can also add multiple rows.
                    new []
                    {
                        InlineKeyboardButton.WithUrl(text: "Canale 1", url: "https://t.me/@UreMybot"),
                    },
                });

            Message sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Before use the bot you must follow this channels.\nWhen you are ready, click -> /home <- to continue", //The message to display
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
        }
        else
        {

            //Block vulgarity - it is a numeric variable that corresponds to the 3 block levels, hard, medium and no blocks. You can set the level of the block either by modifying it in the code and restarting the bot, or directly in ghe group chat via the command /vulgarity.

            //code
            //if I write “/vulgarity”, the bot changes the state of the block.
            if (messageText == "/@UreMybot")
            {
                int blockLevel = 0;
                switch (blockLevel)
                {
                    case 0:
                        blockLevel = 1;
                        await botClient.SendTextMessageAsync
                        (
                        chatId: chatId,
                        text: "Vulgarity: \"Medium block\".",
                         cancellationToken: cancellationToken
                        );
                        return;

                    case 1:
                        blockLevel = 2;
                        await botClient.SendTextMessageAsync
                        (
                        chatId: chatId,
                        text: "Vulgarity: \"Hard block\".",
                         cancellationToken: cancellationToken
                        );
                        return;
                    case 2:
                        blockLevel = 0;
                        await botClient.SendTextMessageAsync
                        (
                        chatId: chatId,
                        text: "Vulgarity: \"Block disabled\".",
                         cancellationToken: cancellationToken
                        );
                        return;
                }
            }


            //if message is Hello .. bot answer Hello + name of user.
            if (messageText == "hello")
            {
                // Echo received message text
                sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Hello " + firstName + " " + lastName + "",
                cancellationToken: cancellationToken);
            }
            
            
            //if message is "countdown" .. bot answer with a countdown video.
            if (messageText == "countdown")
            {
                await NewMethod(botClient, chatId, cancellationToken);
            }

            //if message is "album" .. bot answer with multiple images.
            if (messageText == "album")
            {
                Message[] messages = await botClient.SendMediaGroupAsync(
                chatId: chatId,
                media: new IAlbumInputMedia[]
                {
       //         new InputMediaPhoto("C:\Users\Un Name\\Pictures\Camera Roll/Angkor wat.jpg"),
                },
                cancellationToken: cancellationToken);
            }

          

            //if message is "poll" .. create a poll.
            if (messageText == "poll")
            {
                //save the poll id message
                pollId = messageId + 1;

                Console.WriteLine($"\nPoll number: {pollId}!");
                Message pollMessage = await botClient.SendPollAsync(
                chatId: chatId,
                question: "How are you?",
                options: new[]
                {
                "Good!",
                "I could be better.."
                },
                cancellationToken: cancellationToken);
            }
            //if message is "close poll" .. close the pool.
            if (messageText == "close poll")
            {
                Console.WriteLine($"\nPoll number {pollId} is close!");
                Poll poll = await botClient.StopPollAsync(
                chatId: chatId,
                messageId: pollId,
                cancellationToken: cancellationToken);
            }


            /*This is the code to send a contact. Mandatory are the parameters chatId, phoneNumber and firstName.*/
            if (messageText == "send me the phone number of anna")
            {
                Message message = await botClient.SendContactAsync(
                chatId: chatId,
                phoneNumber: "+1234567890",
                firstName: "Anna",
                lastName: "Rossi",
                cancellationToken: cancellationToken);
            }

            //The code snippet below sends a venue with a title and a address as given parameters:
            if (messageText == "roma location")
            {
                Message message = await botClient.SendVenueAsync(
                    chatId: chatId,
                    latitude: 41.9027835f,
                    longitude: 12.4963655,
                    title: "Rome",
                    address: "Rome, via Daqua 8, 08089",
                    cancellationToken: cancellationToken);
            }

            //The code snippet below sends a location:
            if (messageText == "send me a location")
            {
                //The difference between sending a location and a venue is, that the venue requires a title and address. A location can be any given point as latitude and longitude.The following snippet shows how to send a location with the mandatory parameters:

                Message message = await botClient.SendLocationAsync(
                    chatId: chatId,
                    latitude: 41.9027835f,
                    longitude: 12.4963655,
                    cancellationToken: cancellationToken);
            }
        }
    }
}

Task NewMethod(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
{
    throw new NotImplementedException();
}

Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}/*

static async Task NewMethod(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
{
    await NewMethod1(botClient, chatId, cancellationToken);

    static async Task NewMethod1(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
       Message message = await botClient.SendVideoAsync(
        chatId: chatId,
        video: "C:\Users\Un Name\Videos\Captures/remember when.mp4",
        supportsStreaming: true,
        cancellationToken: cancellationToken);
    
} }*/