﻿using System.Linq;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StudyHard.Helpers;
using StudyHard.Models;
using StudyHard.Service.Interfaces;

namespace StudyHard.Controllers
{
    public class ChatController: BaseController
    {
        private readonly IChatService _chatService;

        public ChatController(
            IUserRepository userRepository, 
            IChatService chatService,
            IUserInfoProvider userInfoProvider) 
            : base(userRepository, userInfoProvider)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public IActionResult ChatView([FromQuery(Name = "chatId")] long? selectedChat)
        {
            long userId = GetUserId();
            var chatData = _chatService.GetUserChats(userId);
            return View(new ChatViewModel
            {
                UserId = userId,
                SelectedChatId = selectedChat ?? (chatData.Any() ? chatData.First().ChatId : -1),
                UserChats = chatData
            });
        }
    }
}