using System.Threading;
using System.Threading.Tasks;
using Trakx.Common.Models;

namespace Trakx.IndiceManager.Client
{
    public interface IApiClient
    {
        string BaseUrl { get; set; }
        bool ReadResponseAsString { get; set; }

        /// <summary>Tries to retrieve a component and its details using its address on chain.</summary>
        /// <param name="address">The ethereum address of the component.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ComponentDetailModel> GetComponentByAddressAsync(string address);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Tries to retrieve a component and its details using its address on chain.</summary>
        /// <param name="address">The ethereum address of the component.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ComponentDetailModel> GetComponentByAddressAsync(string address, System.Threading.CancellationToken cancellationToken);

        /// <summary>Tries to retrieve all of the components that are currently in database.</summary>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.List<ComponentDetailModel>> GetAllComponentsAsync();

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Tries to retrieve all of the components that are currently in database.</summary>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.List<ComponentDetailModel>> GetAllComponentsAsync(System.Threading.CancellationToken cancellationToken);

        /// <summary>Tries to put a new component in the database to use it later in a new indice.</summary>
        /// <param name="body">The component that we want to save.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ComponentDetailModel> SaveComponentDefinitionAsync(ComponentDetailModel body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Tries to put a new component in the database to use it later in a new indice.</summary>
        /// <param name="body">The component that we want to save.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ComponentDetailModel> SaveComponentDefinitionAsync(ComponentDetailModel body, System.Threading.CancellationToken cancellationToken);

        /// <summary>Tries to retrieve all of the indices in our database.</summary>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.List<IndiceDetailModel>> GetAllIndicesAsync();

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Tries to retrieve all of the indices in our database.</summary>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.List<IndiceDetailModel>> GetAllIndicesAsync(System.Threading.CancellationToken cancellationToken);

        /// <summary>Tries to get all of the compositions for an indice.</summary>
        /// <param name="body">The symbol of the indice that we want the compositions. Not to be confused with the composition symbol.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.List<IndiceCompositionModel>> GetCompositionsBySymbolAsync(string body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Tries to get all of the compositions for an indice.</summary>
        /// <param name="body">The symbol of the indice that we want the compositions. Not to be confused with the composition symbol.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.List<IndiceCompositionModel>> GetCompositionsBySymbolAsync(string body, System.Threading.CancellationToken cancellationToken);

        /// <summary>Tries to save a new indice in our database.</summary>
        /// <param name="body">The indice that we want to save.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<IndiceDetailModel> SaveIndiceDefinitionAsync(IndiceDetailModel body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Tries to save a new indice in our database.</summary>
        /// <param name="body">The indice that we want to save.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<IndiceDetailModel> SaveIndiceDefinitionAsync(IndiceDetailModel body, System.Threading.CancellationToken cancellationToken);

        /// <summary>Tries to save a new composition for an indice in our database.</summary>
        /// <param name="body">The composition that the user want to save.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<IndiceCompositionModel> SaveIndiceCompositionAsync(IndiceCompositionModel body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Tries to save a new composition for an indice in our database.</summary>
        /// <param name="body">The composition that the user want to save.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<IndiceCompositionModel> SaveIndiceCompositionAsync(IndiceCompositionModel body, System.Threading.CancellationToken cancellationToken);

        /// <summary>Tries to save a transaction of issuing or redeeming of indices in the database.</summary>
        /// <param name="body">The transaction that we want to save.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<bool> SaveTransactionAsync(IndiceSupplyTransactionModel body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Tries to save a transaction of issuing or redeeming of indices in the database.</summary>
        /// <param name="body">The transaction that we want to save.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<bool> SaveTransactionAsync(IndiceSupplyTransactionModel body, System.Threading.CancellationToken cancellationToken);

        /// <summary>Tries to retrieve all of the issuing and redeeming of indices made by a specific user.</summary>
        /// <param name="body">The user that made the transactions.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.List<IndiceSupplyTransactionModel>> RetrieveTransactionsAsync(string body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Tries to retrieve all of the issuing and redeeming of indices made by a specific user.</summary>
        /// <param name="body">The user that made the transactions.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.List<IndiceSupplyTransactionModel>> RetrieveTransactionsAsync(string body, System.Threading.CancellationToken cancellationToken);

        /// <summary>Allows to return a corresponding address to a token in order for the user to make the transfer to a specific address.</summary>
        /// <param name="body">The symbol of the token for which we want the Trakx's address.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<string> GetTrakxAddressFromSymbolAsync(string body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Allows to return a corresponding address to a token in order for the user to make the transfer to a specific address.</summary>
        /// <param name="body">The symbol of the token for which we want the Trakx's address.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<string> GetTrakxAddressFromSymbolAsync(string body, System.Threading.CancellationToken cancellationToken);

        /// <summary>This route allow to wrap and unwrap tokens. It will register the transaction in the database and send the correct
        /// amount of tokens/wrapped tokens to the user.</summary>
        /// <param name="body">The Trakx.Common.Models.WrappingTransactionModel which that has all the information needed to
        ///             complete the transaction.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<string> WrapTokensAsync(WrappingTransactionModel body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>This route allow to wrap and unwrap tokens. It will register the transaction in the database and send the correct
        /// amount of tokens/wrapped tokens to the user.</summary>
        /// <param name="body">The Trakx.Common.Models.WrappingTransactionModel which that has all the information needed to
        ///             complete the transaction.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<string> WrapTokensAsync(WrappingTransactionModel body, System.Threading.CancellationToken cancellationToken);

        /// <summary>This route allow to retrieve all the transaction associated to an user.</summary>
        /// <param name="body">Here is the name of the user that is register in all the transactions.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.List<WrappingTransactionModel>> GetAllTransactionByUserAsync(string body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>This route allow to retrieve all the transaction associated to an user.</summary>
        /// <param name="body">Here is the name of the user that is register in all the transactions.</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.List<WrappingTransactionModel>> GetAllTransactionByUserAsync(string body, System.Threading.CancellationToken cancellationToken);

        /// <summary>This route allows to retrieve the Trakx's balance, either with native or wrapped tokens.</summary>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.List<AccountBalanceModel>> GetTrakxBalanceAsync();

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>This route allows to retrieve the Trakx's balance, either with native or wrapped tokens.</summary>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.List<AccountBalanceModel>> GetTrakxBalanceAsync(System.Threading.CancellationToken cancellationToken);
    }
}