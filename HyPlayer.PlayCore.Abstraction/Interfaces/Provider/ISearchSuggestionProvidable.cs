using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides autocomplete and suggestion results for provider searches.
/// </summary>
public interface ISearchSuggestionProvidable : IProvider
{
    /// <summary>
    /// Gets provider search suggestions for a keyword.
    /// </summary>
    /// <param name="keyword">The partial search keyword supplied by the user.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A container grouping provider-neutral suggestion items.</returns>
    public Task<ContainerBase> GetSearchSuggestionsAsync(string keyword, CancellationToken ctk = new());
}
