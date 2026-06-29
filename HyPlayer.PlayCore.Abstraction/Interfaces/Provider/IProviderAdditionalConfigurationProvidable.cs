namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IProviderAdditionalConfigurationProvidable : IProvider
{
    bool HasAdditionalConfiguration { get; }

    string ExportAdditionalConfiguration();

    void ImportAdditionalConfiguration(string configurationJson);
}
