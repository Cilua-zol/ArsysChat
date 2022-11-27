using Grpc.Core;
using Mapster;
using PostPigeon.DAL.DbRepositories.Interfaces;
using PostPigeon.Domain.Models;

namespace PostPigeon.Server.Services;

public class ChatroomService : Chatroom.ChatroomBase
{
    private readonly IUsersRepository _usersRepository;
    private readonly IMessagesRepository _messagesRepository;
    private readonly List<IServerStreamWriter<MessageReply>> _observers = new();

    public ChatroomService(IUsersRepository usersRepository, IMessagesRepository messagesRepository)
    {
        _usersRepository = usersRepository;
        _messagesRepository = messagesRepository;
    }
    
    public override async Task<JoinResponse> Join(UserRequest request, ServerCallContext context)
    {
        var user = await _usersRepository.GetAsync(request.Name);
        
        if (user != null) return new JoinResponse { Error = 1, Msg = "User already exist." };
        
        await _usersRepository.CreateAsync(new User{Name = request.Name});
        return new JoinResponse { Error = 0, Msg = "Success" };
    }

    public override async Task<None> SendMessage(MessageRequest request, ServerCallContext context)
    {
        var message = new Message()
        {
            Sender = request.Sender, 
            Text = request.Text, 
            Time = request.Time.ToDateTime()
        };
        await _messagesRepository.CreateAsync(message);

        return new None();
    }

    public override async Task ReceiveMessage(None request, IServerStreamWriter<MessageReply> responseStream, ServerCallContext context)
    {
        foreach (var message in await _messagesRepository.GetAllAsync())
            await responseStream.WriteAsync(message.Adapt<MessageReply>());
        
        _observers.Add(responseStream);
        while (!context.CancellationToken.IsCancellationRequested)
            await Task.Delay(100);
        _observers.Remove(responseStream);
    }

    public override async Task<UserList> GetAllUsers(None request, ServerCallContext context)
    {
        var users = await _usersRepository.GetAllAsync();
        var reply = new UserList();

        users.ForEach(user =>
            reply.Users.Add(user.Adapt<UserRequest>()));

        return reply;
    }
}