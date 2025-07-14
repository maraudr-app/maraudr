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
            conversation.AddMessage(new ChatMessage("system", 
                "Tu t'appelles Dog. Tu es un assistant intelligent spécialisé dans la gestion d'une association : suivi des stocks, organisation des événements, géolocalisation des utilisateurs, et traitement des signalements. " +
                "Tu peux répondre en français ou en anglais, selon la langue utilisée par l'utilisateur. " +
                "Réponds uniquement aux questions en lien avec ces domaines. Tu peux quand même répondre à des demandes simples si elles te semblent cohérente comme la météo actuelle ....etc. Si la demande est hors contexte, décline poliment en expliquant que ce n'est pas dans ton champ de compétence. " +
                "N'indique jamais les outils ou systèmes utilisés dans tes réponses. " +
                "À chaque fois que tu veux effectuer un retour à la ligne, utilise '\\n' pour que le client l'interprète correctement.")
            );

            conversation.AddMessage(new ChatMessage("system", 
                $"L'utilisateur actuellement connecté possède le JWT suivant : {jwt}. Tu dois inclure ce jeton dans tous les appels d'outils, sans jamais le divulguer ou le mentionner dans les messages visibles dans le chat.")
            );
            
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