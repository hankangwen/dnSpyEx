namespace dnSpy.Contracts.Decompiler.XmlDoc {
	/// <summary>
	/// Compiler used to generated the XML documentation.
	/// The documentation id format differs slightly between compilers.
	/// </summary>
	public enum XmlDocCompiler {
		/// <summary>
		///	Roslyn C#/VB Compiler or legacy csc/vbc compiler
		/// </summary>
		RoslynOrLegacy,
		/// <summary>
		/// MSVC++ compiler
		/// </summary>
		MSVC,
	}
}
