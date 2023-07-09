using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SwansonParser2023.Models;
using SwansonParser2023.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bots.Http;

namespace SwansonBot.Models;


public class BotCore
{
    private ProductsContext context;
    private IProductWriter productWriter;

    public BotCore(ProductsContext productsContext, IProductWriter writer)
    {
        context = productsContext;
        productWriter = writer;
    }
    private async Task SendByEmailAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
{
    var file = Guid.NewGuid().ToString("D") + ".xlsx";
    productWriter.SaveAs(file, await context.Products.ToListAsync());
    
    string smtpServer = "smtp.gmail.com";
    int smtpPort = 587;
    string senderEmail = "Rosenberg145@gmail.com";
    string senderPassword = "Steinberg328";
    string recipientEmail = "petyaaaaaaaaaaaaa@gmail.com";
    string emailSubject = "Products List";
    string emailBody = "Please find attached the list of products.";

    using (MailMessage mailMessage = new MailMessage())
    {
        mailMessage.From = new MailAddress(senderEmail);
        mailMessage.To.Add(recipientEmail);
        mailMessage.Subject = emailSubject;
        mailMessage.Body = emailBody;

        Attachment attachment = new Attachment(file);
        mailMessage.Attachments.Add(attachment);

        using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
        {
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }

    await bot.SendTextMessageAsync(
        chatId: update.Message.Chat.Id,
        text: "File sent by email successfully!"
    );

    await Task.Run(() => System.IO.File.Delete(file), cancellationToken);
}
    private async Task SendProductsFileAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        var file = Guid.NewGuid().ToString("D") + ".xlsx";
        productWriter.SaveAs(file, await context.Products.ToListAsync());
        // send file
        await using (Stream stream = System.IO.File.OpenRead(file))
        {
            await bot.SendDocumentAsync(
            chatId: update.Message.Chat.Id,
            document: InputFile.FromStream(stream: stream, fileName: "products.xlsx"),
            caption: "List of products");
        }

        await Task.Run(() => System.IO.File.Delete(file), cancellationToken);

    }

    private async Task FindProductAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        var code = update.Message.Text.Trim();
        var product = context.Products.FirstOrDefault(x => x.Code == code);
        if (product != null)
        {
            await bot.SendPhotoAsync(
                    chatId: update.Message.Chat.Id,
                    photo: InputFile.FromUri(product.ImageUrl),
                    caption:
                    $"{(product.Available ? "🟢" : "🔴")} <b>{product.Title}</b>\n" +
                    $"{product.Description}\n" +
                    $"${product.Price.ToString("F")}\n",
                    parseMode: ParseMode.Html,
                    replyMarkup: new InlineKeyboardMarkup(
                            InlineKeyboardButton.WithUrl(
                                text: "Open on site",
                                url: product.FullUrl))
                    );

        }
        else
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[] { new KeyboardButton[] { "Download file" }, })
            {
                ResizeKeyboard = true
            };

            await bot.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "Not found",
                replyMarkup: replyKeyboardMarkup
                );
        }
    }
    private async Task StartCommand(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
{
    ReplyKeyboardMarkup replyKeyboardMarkup = new(
        new[] { new KeyboardButton[] { "Download file" }, new KeyboardButton[] { "Receive by mail" } })
    {
        ResizeKeyboard = true
    };

    await bot.SendTextMessageAsync(
        chatId: update.Message.Chat.Id,
        text: "Hello! This is Swanson parser bot.",
        replyMarkup: replyKeyboardMarkup
    );
}
    public async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
{
    await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(update));

    if (update.Message != null)
    {
        await bot.SendChatActionAsync(
                   chatId: update.Message.Chat.Id,
                   chatAction: ChatAction.Typing
                   );

        var text = update.Message.Text;

        if (text == "Download file")
        {
            await SendProductsFileAsync(bot, update, cancellationToken);
        }
        else if (text == "/start")
        {
            await StartCommand(bot, update, cancellationToken);
        }
        else if (text == "Receive by mail") 
        {
            await SendByEmailAsync(bot, update, cancellationToken);
        }
        else
        {
            await FindProductAsync(bot, update, cancellationToken);
        }
        return;
    }
}
}
