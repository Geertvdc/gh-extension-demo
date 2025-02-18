using Microsoft.Extensions.AI;

namespace PizzaAgent;

public class ChatRequest
{
    public bool Stream { get; set; }
    public List<ChatMessage> Messages { get; set; } = [];
}