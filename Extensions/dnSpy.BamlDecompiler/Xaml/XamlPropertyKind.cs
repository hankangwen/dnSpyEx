namespace dnSpy.BamlDecompiler.Xaml {
	enum XamlPropertyKind : byte {
		// Note: The order of these should remain consistent with BamlAttributeUsage!
		Default,
		XmlLang,
		XmlSpace,
		RuntimeName,
	}
}
