using System.Linq;
using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StudyHard.Models;
using StudyHard.Service.Interfaces;

namespace StudyHard.Controllers
{
    public class ChatController: BaseController
    {
        private readonly IChatService _chatService;
        
        public ChatController(IUserRepository userRepository, IChatService chatService): base(userRepository)
        {
            _chatService = chatService;
        }
        
        [HttpGet]
        public IActionResult ChatView()
        {
            long userId = GetUserId();
            var chatData = _chatService.GetUserChats(userId);
            return View(new ChatViewModel
            {
                UserId = userId,
                SelectedChatId = chatData.Any() ? chatData.First().ChatId : -1,
                UserChats = chatData
            });
        }
    }
}