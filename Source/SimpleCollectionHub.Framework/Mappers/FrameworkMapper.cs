using Mapster;

namespace SimpleCollectionHub.Framework.Mappers;

internal class FrameworkMapper : IRegister
{
	void IRegister.Register(TypeAdapterConfig config)
	{
		//config.NewConfig<UserSequenceItem, UserSequence>().RequireDestinationMemberSource(true).TwoWays();

	}

}
