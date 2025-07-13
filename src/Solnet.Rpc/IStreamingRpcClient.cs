using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Solnet.Rpc
{
    /// <summary>
    /// Represents the streaming RPC client for the solana API.
    /// </summary>
    public interface IStreamingRpcClient : IDisposable
    {
        /// <summary>
        /// Current connection state.
        /// </summary>
        WebSocketState State { get; }

        /// <summary>
        /// Event triggered when the connection status changes between connected and disconnected.
        /// </summary>
        event EventHandler<WebSocketState> ConnectionStateChangedEvent;

        /// <summary>
        /// The address this client connects to.
        /// </summary>
        Uri NodeAddress { get; }

        /// <summary>
        /// Statistics of the current connection.
        /// </summary>
        IConnectionStatistics Statistics { get; }

        /// <summary>
        /// Subscribes asynchronously to AccountInfo notifications.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="pubkey">The public key of the account.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeAccountInfoAsync(string pubkey, Action<SubscriptionState, 
            ResponseValue<AccountInfo>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes to the AccountInfo. This is a synchronous and blocking function.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="pubkey">The public key of the account.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeAccountInfo(string pubkey, Action<SubscriptionState, ResponseValue<AccountInfo>> callback, 
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes asynchronously to Token Account notifications. Note: Only works if the account is a Token Account.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="pubkey">The public key of the account.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeTokenAccountAsync(string pubkey, Action<SubscriptionState, 
            ResponseValue<TokenAccountInfo>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes  to Token Account notifications. Note: Only works if the account is a Token Account.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="pubkey">The public key of the account.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeTokenAccount(string pubkey, Action<SubscriptionState, ResponseValue<TokenAccountInfo>> callback, 
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes asynchronously to the logs notifications that mention a given public key.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="pubkey">The public key to filter by mention.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeLogInfoAsync(string pubkey, Action<SubscriptionState, ResponseValue<LogInfo>> callback, 
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes asynchronously to the logs notifications.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="subscriptionType">The filter mechanism.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeLogInfoAsync(LogsSubscriptionType subscriptionType, Action<SubscriptionState, 
            ResponseValue<LogInfo>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes to the logs notifications that mention a given public key. This is a synchronous and blocking function.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="pubkey">The public key to filter by mention.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeLogInfo(string pubkey, Action<SubscriptionState, ResponseValue<LogInfo>> callback, 
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes to the logs notifications. This is a synchronous and blocking function.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="subscriptionType">The filter mechanism.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeLogInfo(LogsSubscriptionType subscriptionType, Action<SubscriptionState, 
            ResponseValue<LogInfo>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes asynchronously to a transaction signature to receive notification when the transaction is confirmed.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="transactionSignature">The transaction signature.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeSignatureAsync(string transactionSignature, Action<SubscriptionState, 
            ResponseValue<ErrorResult>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes to a transaction signature to receive notification when the transaction is confirmed. This is a synchronous and blocking function.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="transactionSignature">The transaction signature.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeSignature(string transactionSignature, Action<SubscriptionState, ResponseValue<ErrorResult>> callback, 
            Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes asynchronously to changes to a given program account data.
        /// </summary>
        /// <param name="programPubkey"></param>
        /// <param name="callback"></param>
        /// <param name="commitment"></param>
        /// <param name="dataSize"></param>
        /// <param name="memCmpList"></param>
        /// <returns></returns>
        Task<SubscriptionState> SubscribeProgramAsync(string programPubkey, Action<SubscriptionState, ResponseValue<AccountKeyPair>> callback, 
            Commitment commitment = Commitment.Finalized, int? dataSize = null, IList<MemCmp> memCmpList = null);

        /// <summary>
        /// Subscribes to changes to a given program account data. This is a synchronous and blocking function.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="programPubkey">The program pubkey.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <param name="dataSize"></param>
        /// <param name="memCmpList"></param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeProgram(string programPubkey, Action<SubscriptionState, ResponseValue<AccountKeyPair>> callback, 
            Commitment commitment = Commitment.Finalized, int? dataSize = null, IList<MemCmp> memCmpList = null);

        /// <summary>
        /// Subscribes asynchronously to receive notifications anytime a slot is processed by the validator.
        /// </summary>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeSlotInfoAsync(Action<SubscriptionState, SlotInfo> callback);

        /// <summary>
        /// Subscribes to receive notifications anytime a slot is processed by the validator. This is a synchronous and blocking function.
        /// </summary>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeSlotInfo(Action<SubscriptionState, SlotInfo> callback);


        /// <summary>
        /// Subscribes asynchronously to block notifications, with optional filtering and configuration.
        /// Note: Unstable method (This subscription is considered unstable and is only available if the validator was started with the --rpc-pubsub-enable-block-subscription flag. The format of this subscription may change in the future)
        /// </summary>
        /// <param name="mentionsAccountOrProgram">
        /// Filter criteria for the logs to receive results by account type. Supported values:
        /// <list type="bullet">
        /// <item><description><c>all</c> - include all transactions in block</description></item>
        /// <item><description>A base-58 encoded public key string - return only transactions that mention the provided public key. If no mentions in a given block, then no notification will be sent.</description></item>
        /// </list>
        /// </param>
        /// <param name="callback">The callback to handle block data notifications.</param>
        /// <param name="commitment">
        /// Optional. The state commitment to consider when querying the ledger state. Supported values:
        /// <list type="bullet">
        /// <item><description><c>confirmed</c></description></item>
        /// <item><description><c>finalized</c> (default)</description></item>
        /// </list>
        /// <para>Describes how finalized a block is at that point in time. See Configuring State Commitment.</para>
        /// <para><c>processed</c> is not supported.</para>
        /// </param>
        /// <param name="encoding">
        /// Optional. Encoding format for each returned transaction. Supported values:
        /// <list type="bullet">
        /// <item><description><c>json</c> (default)</description></item>
        /// <item><description><c>jsonParsed</c></description></item>
        /// <item><description><c>base58</c></description></item>
        /// <item><description><c>base64</c></description></item>
        /// </list>
        /// <para><c>jsonParsed</c> attempts to use program-specific instruction parsers for more human-readable data. If unavailable, falls back to regular JSON encoding.</para>
        /// </param>
        /// <param name="transactionDetails">
        /// Optional. Level of transaction detail to return. Supported values:
        /// <list type="bullet">
        /// <item><description><c>full</c> (default)</description></item>
        /// <item><description><c>accounts</c></description></item>
        /// <item><description><c>signatures</c></description></item>
        /// <item><description><c>none</c></description></item>
        /// </list>
        /// <para>If <c>accounts</c> is requested, transaction details only include signatures and an annotated list of accounts. Transaction metadata is limited to: fee, err, pre_balances, post_balances, pre_token_balances, and post_token_balances.</para>
        /// </param>
        /// <param name="maxSupportedTransactionVersion">
        /// Optional. The maximum transaction version to return. Default is <c>0</c>.
        /// <para>Currently, only <c>0</c> is valid. Setting to <c>0</c> allows fetching all transactions, including both Versioned and legacy transactions.</para>
        /// <para>If omitted, only legacy transactions are returned; any versioned transaction will result in an error.</para>
        /// </param>
        /// <param name="rewards">
        /// Optional. Whether to populate the rewards array. If not provided, rewards are included by default.
        /// </param>
        /// <returns>The task object representing the asynchronous operation, with a subscription state containing the subscription id (needed to unsubscribe).</returns>
        Task<SubscriptionState> SubscribeBlockInfoAsync(
            string mentionsAccountOrProgram,
            Action<SubscriptionState, BlockInfo> callback,
            Commitment commitment = Commitment.Finalized,
            string encoding = "base64",
            string transactionDetails = "full",
            int maxSupportedTransactionVersion = 0,
            bool rewards = false);

        /// <summary>
        /// Subscribes to block notifications, with optional configuration. This is a synchronous and blocking function.
        /// </summary>
        /// <param name="mentionsAccountOrProgram">
        /// Filter criteria for the logs to receive results by account type. Supported values:
        /// <list type="bullet">
        /// <item><description><c>all</c> - include all transactions in block</description></item>
        /// <item><description>A base-58 encoded public key string - return only transactions that mention the provided public key. If no mentions in a given block, then no notification will be sent.</description></item>
        /// </list>
        /// </param>
        /// <param name="callback">The callback to handle block data notifications.</param>
        /// <param name="commitment">
        /// Optional. The state commitment to consider when querying the ledger state. Supported values:
        /// <list type="bullet">
        /// <item><description><c>confirmed</c></description></item>
        /// <item><description><c>finalized</c> (default)</description></item>
        /// </list>
        /// <para>Describes how finalized a block is at that point in time. See Configuring State Commitment.</para>
        /// <para><c>processed</c> is not supported.</para>
        /// </param>
        /// <param name="encoding">
        /// Optional. Encoding format for each returned transaction. Supported values:
        /// <list type="bullet">
        /// <item><description><c>json</c> (default)</description></item>
        /// <item><description><c>jsonParsed</c></description></item>
        /// <item><description><c>base58</c></description></item>
        /// <item><description><c>base64</c></description></item>
        /// </list>
        /// <para><c>jsonParsed</c> attempts to use program-specific instruction parsers for more human-readable data. If unavailable, falls back to regular JSON encoding.</para>
        /// </param>
        /// <param name="transactionDetails">
        /// Optional. Level of transaction detail to return. Supported values:
        /// <list type="bullet">
        /// <item><description><c>full</c> (default)</description></item>
        /// <item><description><c>accounts</c></description></item>
        /// <item><description><c>signatures</c></description></item>
        /// <item><description><c>none</c></description></item>
        /// </list>
        /// <para>If <c>accounts</c> is requested, transaction details only include signatures and an annotated list of accounts. Transaction metadata is limited to: fee, err, pre_balances, post_balances, pre_token_balances, and post_token_balances.</para>
        /// </param>
        /// <param name="maxSupportedTransactionVersion">
        /// Optional. The maximum transaction version to return. Default is <c>0</c>.
        /// <para>Currently, only <c>0</c> is valid. Setting to <c>0</c> allows fetching all transactions, including both Versioned and legacy transactions.</para>
        /// <para>If omitted, only legacy transactions are returned; any versioned transaction will result in an error.</para>
        /// </param>
        /// <param name="rewards">
        /// Optional. Whether to populate the rewards array. If not provided, rewards are included by default.
        /// </param>
        /// <returns>Returns a subscription state containing the subscription id (needed to unsubscribe).</returns>
        SubscriptionState SubscribeBlockInfo(
            string mentionsAccountOrProgram,
            Action<SubscriptionState, BlockInfo> callback,
            Commitment commitment = Commitment.Finalized,
            string encoding = "base64",
            string transactionDetails = "full",
            int maxSupportedTransactionVersion = 0,
            bool rewards = false);

        /// <summary>
        /// Subscribes asynchronously to receive notifications anytime a new root is set by the validator.
        /// </summary>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeRootAsync(Action<SubscriptionState, int> callback);

        /// <summary>
        /// Subscribes to receive notifications anytime a new root is set by the validator. This is a synchronous and blocking function.
        /// </summary>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeRoot(Action<SubscriptionState, int> callback);

        /// <summary>
        /// Asynchronously unsubscribes from a given subscription using the state object.
        /// </summary>
        /// <param name="subscription">The subscription state object.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task UnsubscribeAsync(SubscriptionState subscription);

        /// <summary>
        /// Unsubscribes from a given subscription using the state object. This is a synchronous and blocking function.
        /// </summary>
        /// <param name="subscription">The subscription state object.</param>
        void Unsubscribe(SubscriptionState subscription);

        /// <summary>
        /// Asynchronously initializes the client connection and starts listening for socket messages.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task ConnectAsync();
        /// <summary>
        /// Asynchronously disconnects and removes all running subscriptions.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task DisconnectAsync();
    }
}