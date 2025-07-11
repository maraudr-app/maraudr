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
    public async Task<ChatResponseDto> ProcessChatAsync(ChatRequestDto request,string jwt)
        {
            try
            {
                var conversation = BuildConversation(request,jwt);
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

        public async Task<IAsyncEnumerable<string>> ProcessStreamingChatAsync(ChatRequestDto request,string jwt)
        {
            var conversation = BuildConversation(request,jwt);
            var availableTools = await mcpRepository.GetAvailableToolsAsync();

            return await chatRepository.GetStreamingResponseAsync(conversation.Messages, availableTools);
        }

        private Conversation BuildConversation(ChatRequestDto request,string jwt)
        {
            var conversation = new Conversation();
            conversation.AddMessage(new ChatMessage("system", "Tu t'appelles Dog et tu es un assistant spécialisé dans la gestion d'une association, " +
                                                              "des stocks d'une association, des évennements et de la geolocalisation des utilisateurs et de signalements émis." +
                                                              "Réponds uniquement aux questions liées à ce domaine.Si la requête est hors contexte refuse poliment" +
                                                              "Ne cite pas les outils utilisés dans tes réponses." +
                                                              "Dès que tu veux renvoyer un saut de ligne tu renvoie slash n pour que le client comprenne"));
            conversation.AddMessage(new ChatMessage("system", $"L'utilisateur actuellement connecté possède le JWT suivant : {jwt} et tu dois le passer à tous les appels d'outils sans jamais le divulguer dans le chat" ));
            
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