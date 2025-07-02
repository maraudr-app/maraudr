using Maraudr.MCP.Domain.Entities;
using Maraudr.MCP.Domain.Interfaces;
using MCP.Maraudr.Application.Dtos;
using MCP.Maraudr.Application.Services.Interfaces;

namespace MCP.Maraudr.Application.Services;

public class ChatService(
    IMCPRepository mcpRepository,
    IChatRepository chatRepository)
    : IChatService
{
    public async Task<ChatResponseDto> ProcessChatAsync(ChatRequestDto request)
        {
            try
            {
                var conversation = BuildConversation(request);
                var availableTools = await mcpRepository.GetAvailableToolsAsync();
                
                var response = await chatRepository.GetResponseAsync(
                    conversation.Messages, 
                    availableTools);

                conversation.AddMessage(new ChatMessage("assistant", response));

                return new ChatResponseDto(
                    response,
                    conversation.Messages.Select(m => new ChatMessageDto(m.Role, m.Content)).ToList()
                );
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to process chat request", ex);
            }
        }

        public async Task<IAsyncEnumerable<string>> ProcessStreamingChatAsync(ChatRequestDto request)
        {
            var conversation = BuildConversation(request);
            var availableTools = await mcpRepository.GetAvailableToolsAsync();
            
            return await chatRepository.GetStreamingResponseAsync(conversation.Messages, availableTools);
        }

        private Conversation BuildConversation(ChatRequestDto request)
        {
            var conversation = new Conversation();
            conversation.AddMessage(new ChatMessage("system", "Tu es un assistant spécialisé dans la gestion d'une association, des stocks d'une association, de la geolocalisation des utilisateurs et de signalements émis,. Réponds uniquement aux questions liées à ce domaine.Si la requête est hors contexte refuse poliment"));
            
            if (request.ConversationHistory != null)
            {
                foreach (var msg in request.ConversationHistory)
                {
                    conversation.AddMessage(new ChatMessage(msg.Role, msg.Content));
                }
            }
            
            conversation.AddMessage(new ChatMessage("user", request.Message));
            return conversation;
        }
    }