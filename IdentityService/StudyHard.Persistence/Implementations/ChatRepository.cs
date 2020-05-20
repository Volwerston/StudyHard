using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using StudyHard.Persistence.Entities;
using StudyHard.Persistence.Interfaces;

namespace StudyHard.Persistence.Implementations
{
    public class ChatRepository : IChatRepository
    {
        private readonly string _connectionString;

        public ChatRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ChatEntity GetChatById(long id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var chats = db.Query<ChatEntity>(
                    "SELECT * FROM Chat WHERE Id = @Id", new {Id = id}).ToList();

                return chats.Any() ? chats.First() : null;
            }
        }

        public ChatEntity GetChatByUsers(long userId1, long userId2)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var chats = db.Query<ChatEntity>(
                    "SELECT * FROM Chat WHERE (UserId1 = @UserId1 AND UserId2 = @UserId2) OR (UserId2 = @UserId1 AND UserId1 = @UserId2)",
                    new {UserId1 = userId1, UserId2 = userId2}).ToList();

                return chats.Any() ? chats.First() : null;
            }
        }

        public List<ChatEntity> GetChatsByUser(long userId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return db.Query<ChatEntity>(
                    "SELECT * FROM Chat WHERE UserId1 = @Id OR UserId2 = @Id", new {Id = userId}).ToList();
            }
        }

        public ChatEntity CreateChat(long userId1, long userId2)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                long createdId = db.Query<long>(
                    "INSERT INTO Chat (UserId1, UserId2, Created) VALUES (@UserId1, @UserId2, @Created); SELECT ident_current('[dbo].[Chat]')",
                    new {UserId1 = userId1, UserId2 = userId2, Created = DateTime.Now}).First();

                return GetChatById(createdId);
            }
        }

        public List<MessageEntity> GetMessages(long chatId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return db.Query<MessageEntity>(
                    "SELECT * FROM Message WHERE ChatId = @ChatId", new {ChatId = chatId}).ToList();
            }
        }

        public List<MessageEntity> GetLatestMessages(List<long> chatIds)
        {
            if (!chatIds.Any())
            {
                return new List<MessageEntity>();
            }

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return db.Query<MessageEntity>(
                    "SELECT * FROM Message m WHERE m.ChatId IN @ChatIds AND m.Id = (SELECT MAX(mm.Id) FROM Message mm WHERE mm.ChatId = m.ChatId)",
                    new {ChatIds = chatIds}).ToList();
            }
        }

        public MessageEntity GetMessageById(long id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var messages = db.Query<MessageEntity>(
                    "SELECT * FROM Message WHERE Id = @Id", new {Id = id}).ToList();

                return messages.Any() ? messages.First() : null;
            }
        }

        public MessageEntity SaveMessage(long chatId, long sentById, string content)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                long createdId = db.Query<long>(
                    "INSERT INTO Message (Content, SentDateTime, ChatId, SentBy) VALUES (@Content, @SentDateTime, @ChatId, @SentById); SELECT ident_current('Message')",
                    new {Content = content, SentDateTime = DateTime.Now, ChatId = chatId, SentById = sentById}).First();

                return GetMessageById(createdId);
            }
        }
    }
}