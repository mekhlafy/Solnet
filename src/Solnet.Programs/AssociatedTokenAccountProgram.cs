using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the Associated Token Account Program methods.
    /// <remarks>
    /// For more information see: https://spl.solana.com/associated-token-account
    /// This refactor adds:
    ///  - Token-2022 support (by parameterizing the token program id in derivation + creation)
    ///  - Idempotent create (safe to include even if ATA already exists)
    ///  - Backward-compatible legacy overloads
    /// </remarks>
    /// </summary>
    public static class AssociatedTokenAccountProgram
    {
        /// <summary>
        /// The address of the Associated Token Account (ATA) Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new("ATokenGPvbdGVxr1b2hvZbsiqW5xWH25efTNsLJA8knL");

        /// <summary>
        /// Legacy SPL Token Program id (Tokenkeg...).
        /// </summary>
        public static readonly PublicKey TokenProgramId = TokenProgram.ProgramIdKey;


        /// <summary>
        /// Token-2022 Program id (TokenzQd...).
        /// </summary>
        public static readonly PublicKey Token2022ProgramId = new("TokenzQdBNbLqP5VEhdkAS6EPFLC1PHnBqCXEpPxuEb");

        private const string ProgramName = "Associated Token Account Program";

        private const string InstructionNameCreate = "Create Associated Token Account";
        private const string InstructionNameCreateIdempotent = "Create Associated Token Account (Idempotent)";

        /// <summary>
        /// Initialize a new transaction which interacts with the ATA Program to create
        /// a new associated token account (LEGACY behavior).
        /// </summary>
        /// <remarks>
        /// This is kept for backward compatibility. For new code, prefer
        /// <see cref="CreateAssociatedTokenAccountIdempotent(PublicKey, PublicKey, PublicKey, PublicKey)"/>.
        /// This overload assumes the legacy SPL Token program (Tokenkeg...).
        /// </remarks>
        public static TransactionInstruction CreateAssociatedTokenAccount(PublicKey payer, PublicKey owner, PublicKey mint)
        {
            var associatedTokenAddress = DeriveAssociatedTokenAccount(owner, mint, TokenProgramId);

            var keys = new List<AccountMeta>(7)
        {
            AccountMeta.Writable(payer, true),
            AccountMeta.Writable(associatedTokenAddress, false),
            AccountMeta.ReadOnly(owner, false),
            AccountMeta.ReadOnly(mint, false),
            AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
            AccountMeta.ReadOnly(TokenProgramId, false),
            AccountMeta.ReadOnly(SysVars.RentKey, false)
        };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = Array.Empty<byte>() // legacy create (non-idempotent)
            };
        }

        /// <summary>
        /// Initialize a new transaction which interacts with the ATA Program to create
        /// a new associated token account using the IDEMPOTENT variant.
        /// </summary>
        /// <param name="payer">Account used to fund the associated token account (signer).</param>
        /// <param name="owner">The owner of the associated token account.</param>
        /// <param name="mint">The mint for the associated token account.</param>
        /// <param name="tokenProgramId">
        /// The token program that owns <paramref name="mint"/> (Tokenkeg... for legacy SPL, TokenzQd... for Token-2022).
        /// </param>
        /// <returns>The transaction instruction. Safe to include even if the ATA already exists.</returns>
        public static TransactionInstruction CreateAssociatedTokenAccountIdempotent(
            PublicKey payer,
            PublicKey owner,
            PublicKey mint,
            PublicKey tokenProgramId)
        {
            var associatedTokenAddress = DeriveAssociatedTokenAccount(owner, mint, tokenProgramId);

            // AToken "create idempotent" discriminator = 1 (single byte payload)
            var data = new byte[] { 1 };

            var keys = new List<AccountMeta>(7)
            {
                // Keep account order compatible with many clients and your previous fork
                AccountMeta.Writable(payer, true),
                AccountMeta.Writable(associatedTokenAddress, false),
                AccountMeta.ReadOnly(owner, false),
                AccountMeta.ReadOnly(mint, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(tokenProgramId, false), // MUST match mint.owner (legacy or 2022)
                AccountMeta.ReadOnly(SysVars.RentKey, false)
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = data
            };
        }

        /// <summary>
        /// Convenience overload for legacy SPL Token (Tokenkeg...).
        /// </summary>
        public static TransactionInstruction CreateAssociatedTokenAccountIdempotent(
            PublicKey payer,
            PublicKey owner,
            PublicKey mint) => CreateAssociatedTokenAccountIdempotent(payer, owner, mint, TokenProgramId);

        /// <summary>
        /// Derive the associated token account PDA for either legacy SPL Token or Token-2022.
        /// </summary>
        /// <param name="owner">Owner of the associated token account.</param>
        /// <param name="mint">Mint of the associated token account.</param>
        /// <param name="tokenProgramId">Token program that owns the mint (legacy or 2022).</param>
        public static PublicKey DeriveAssociatedTokenAccount(PublicKey owner, PublicKey mint, PublicKey tokenProgramId)
        {
            PublicKey.TryFindProgramAddress(
                new[] { owner.KeyBytes, tokenProgramId.KeyBytes, mint.KeyBytes },
                ProgramIdKey,
                out var ata,
                out _
            );
            return ata!;
        }

        /// <summary>
        /// Legacy convenience overload (assumes legacy SPL token program).
        /// </summary>
        public static PublicKey DeriveAssociatedTokenAccount(PublicKey owner, PublicKey mint) =>
            DeriveAssociatedTokenAccount(owner, mint, TokenProgramId);

        /// <summary>
        /// Decodes an instruction created by the ATA Program (supports both legacy create and idempotent).
        /// </summary>
        public static DecodedInstruction Decode(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            // Note: different clients order accounts differently. This decoder assumes the order used above:
            // [payer, associated, owner, mint, system_program, token_program, rent]
            string name = data.Length == 1 && data[0] == 1 ? InstructionNameCreateIdempotent : InstructionNameCreate;

            var decodedInstruction = new DecodedInstruction
            {
                PublicKey = ProgramIdKey,
                InstructionName = name,
                ProgramName = ProgramName,
                Values = new Dictionary<string, object>
            {
                {"Payer", keys[keyIndices[0]]},
                {"Associated Token Account Address", keys[keyIndices[1]]},
                {"Owner", keys[keyIndices[2]]},
                {"Mint", keys[keyIndices[3]]},
                {"System Program", keys[keyIndices[4]]},
                {"Token Program", keys[keyIndices[5]]},
                {"Rent Sysvar", keys[keyIndices[6]]},
            },
                InnerInstructions = new List<DecodedInstruction>()
            };

            return decodedInstruction;
        }
    }
}