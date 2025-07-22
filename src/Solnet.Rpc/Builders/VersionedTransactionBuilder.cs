using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Solnet.Rpc.Builders
{
    /// <summary>
    /// Mirrors the legacy TransactionBuilder but builds versioned (v0) transactions.
    /// </summary>
    public class VersionedTransactionBuilder
    {
        public const int SignatureLength = 64;

        private readonly VersionedMessageBuilder _messageBuilder;
        private readonly List<string> _signatures;
        private byte[] _serializedMessage;

        public VersionedTransactionBuilder()
        {
            _messageBuilder = new VersionedMessageBuilder();
            _signatures = new List<string>();
        }

        public VersionedTransactionBuilder SetRecentBlockHash(string recentBlockHash)
        {
            _messageBuilder.RecentBlockHash = recentBlockHash;
            return this;
        }

        public VersionedTransactionBuilder SetNonceInformation(NonceInformation nonceInfo)
        {
            _messageBuilder.NonceInformation = nonceInfo;
            return this;
        }

        public VersionedTransactionBuilder SetFeePayer(PublicKey feePayer)
        {
            _messageBuilder.FeePayer = feePayer;
            return this;
        }

        public VersionedTransactionBuilder AddInstruction(TransactionInstruction instruction)
        {
            _messageBuilder.AddInstruction(instruction);
            return this;
        }

        public byte[] CompileMessage()
        {
            return _messageBuilder.Build();
        }

        public VersionedTransactionBuilder AddSignature(byte[] signature)
        {
            _signatures.Add(Encoders.Base58.EncodeData(signature));
            return this;
        }

        public VersionedTransactionBuilder AddAddressTableLookup(Message.VersionedMessage.MessageAddressTableLookup lookup)
        {
            _messageBuilder.AddressTableLookups.Add(lookup);
            return this;
        }

        public VersionedTransactionBuilder AddAddressTableLookups(IEnumerable<Message.VersionedMessage.MessageAddressTableLookup> lookups)
        {
            _messageBuilder.AddressTableLookups.AddRange(lookups);
            return this;
        }

        public VersionedTransactionBuilder AddSignature(string base58Signature)
        {
            _signatures.Add(base58Signature);
            return this;
        }

        public byte[] Serialize()
        {
            byte[] signaturesLength = ShortVectorEncoding.EncodeLength(_signatures.Count);
            _serializedMessage ??= _messageBuilder.Build();

            using var buffer = new MemoryStream(signaturesLength.Length + _signatures.Count * SignatureLength + _serializedMessage.Length);
            buffer.Write(signaturesLength);

            foreach (string signature in _signatures)
                buffer.Write(Encoders.Base58.DecodeData(signature));

            buffer.Write(_serializedMessage);

            return buffer.ToArray();
        }

        private void Sign(IList<Account> signers)
        {
            if (signers == null || signers.Count == 0)
                throw new Exception("no signers for the transaction");

            if (_messageBuilder.FeePayer == null)
                throw new Exception("fee payer is required");

            _serializedMessage = _messageBuilder.Build();

            foreach (var signer in signers)
            {
                byte[] signature = signer.Sign(_serializedMessage);
                _signatures.Add(Encoders.Base58.EncodeData(signature));
            }
        }

        public byte[] Build(Account signer)
        {
            return Build(new List<Account> { signer });
        }

        public byte[] Build(IList<Account> signers)
        {
            Sign(signers);
            return Serialize();
        }
    }
}