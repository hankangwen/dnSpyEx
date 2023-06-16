using System.Xml.Linq;
using dnSpy.BamlDecompiler.Xaml;

namespace dnSpy.BamlDecompiler {
	sealed class BamlConnectionId {
		public uint Id { get; }

		public BamlConnectionId(uint id) => Id = id;
	}

	sealed class TargetTypeAnnotation {
		public XamlType Type { get; }

		public TargetTypeAnnotation(XamlType type) => Type = type;
	}

	sealed class IsMemberNameAnnotation {
		public static readonly IsMemberNameAnnotation Instance = new IsMemberNameAnnotation();
	}

	static class XNodeAnnotationExtensions {
		public static T WithAnnotation<T>(this T node, object annotation) where T : XNode {
			node.AddAnnotation(annotation);
			return node;
		}
	}
}
