using Mapster;
using PostPigeon.Domain.Models;

namespace PostPigeon.Server.Mappings;

public class RegisterMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserRequest>()
            .RequireDestinationMemberSource(true);

        config.NewConfig<Message, MessageReply>()
            .RequireDestinationMemberSource(true);
    }
}