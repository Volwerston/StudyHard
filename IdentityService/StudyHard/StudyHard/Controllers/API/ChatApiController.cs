using IdentityService.Persistence.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StudyHard.Helpers;
using StudyHard.Service.Dtos;
using StudyHard.Service.Interfaces;

namespace StudyHard.Controllers.API
{
    [Route("api/chat")]
    public class ChatApiController : BaseController
    {
        private IChatService _chatService;

        public ChatApiController(
            IUserRepository userRepository, 
            IChatService chatService,
            IUserInfoProvider userInfoProvider) 
            : base(userRepository, userInfoProvider)
        {
            _chatService = chatService;
        }

        [HttpGet("with/{collocutorId}")]
        public IActionResult GetChatId(int collocutorId)
        {
            return new ObjectResult(new {ChatId = _chatService.InitiateChatWithUser(GetUserId(), collocutorId)});
        }
        
        [HttpGet("{chatId}/messages")]
        public IActionResult GetChatMessages(int chatId)
        {
            return new ObjectResult(_chatService.GetChatMessages(GetUserId(), chatId));
        }
        
        [HttpPost("{chatId}/messages")]
        public IActionResult SendMessage([FromBody] SendMessageRequest request)
        {
            return new ObjectResult(_chatService.SaveMessage(GetUserId(), request));
        }
        
        [HttpGet("chat-data")]
        public IActionResult GetChatData()
        {
            return new ObjectResult(_chatService.GetUserChats(GetUserId()));
        }
    }
}