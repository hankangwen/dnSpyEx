using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.LanguageServices;

namespace dnSpy.Roslyn.Internal.QuickInfo {
	sealed class QuickInfoContext {
		/// <summary>
		/// The document that quick info was requested within.
		/// </summary>
		public Document Document { get; }

		/// <summary>
		/// The caret position where quick info was requested from.
		/// </summary>
		public int Position { get; }

		public SymbolDescriptionOptions Options { get; }

		/// <summary>
		/// The cancellation token to use for this operation.
		/// </summary>
		public CancellationToken CancellationToken { get; }

		/// <summary>
		/// Creates a <see cref="QuickInfoContext"/> instance.
		/// </summary>
		public QuickInfoContext(Document document,
			int position,
			SymbolDescriptionOptions options,
			CancellationToken cancellationToken) {
			Document = document ?? throw new ArgumentNullException(nameof(document));
			Position = position;
			Options = options;
			CancellationToken = cancellationToken;
		}
	}
}
