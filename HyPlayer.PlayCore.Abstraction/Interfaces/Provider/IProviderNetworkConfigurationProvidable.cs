namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IProviderNetworkConfigurationProvidable : IProvider
{
    void ConfigureClientNetwork(string? clientIp, bool useInsecureHttp);
}
