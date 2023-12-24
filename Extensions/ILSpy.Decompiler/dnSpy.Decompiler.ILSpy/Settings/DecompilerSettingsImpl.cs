/*
    Copyright (C) 2014-2019 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using dnSpy.Contracts.Settings;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;

namespace dnSpy.Decompiler.ILSpy.Settings {
	[Export]
	sealed class DecompilerSettingsImpl : DecompilerSettings {
		static readonly Guid SETTINGS_GUID = new Guid("6745457F-254B-4B7B-90F1-F948F0721C3B");

		readonly ISettingsService settingsService;

		[ImportingConstructor]
		DecompilerSettingsImpl(ISettingsService settingsService) : base(LanguageVersion.CSharp11_0) {
			this.settingsService = settingsService;

			disableSave = true;
			var sect = settingsService.GetOrCreateSection(SETTINGS_GUID);
			// Only read those settings that can be changed in the dialog box
			DecompilationObject0 = sect.Attribute<DecompilationObject?>(nameof(DecompilationObject0)) ?? DecompilationObject0;
			DecompilationObject1 = sect.Attribute<DecompilationObject?>(nameof(DecompilationObject1)) ?? DecompilationObject1;
			DecompilationObject2 = sect.Attribute<DecompilationObject?>(nameof(DecompilationObject2)) ?? DecompilationObject2;
			DecompilationObject3 = sect.Attribute<DecompilationObject?>(nameof(DecompilationObject3)) ?? DecompilationObject3;
			DecompilationObject4 = sect.Attribute<DecompilationObject?>(nameof(DecompilationObject4)) ?? DecompilationObject4;

			NativeIntegers = sect.Attribute<bool?>(nameof(NativeIntegers)) ?? NativeIntegers;
			NumericIntPtr = sect.Attribute<bool?>(nameof(NumericIntPtr)) ?? NumericIntPtr;
			CovariantReturns = sect.Attribute<bool?>(nameof(CovariantReturns)) ?? CovariantReturns;
			InitAccessors = sect.Attribute<bool?>(nameof(InitAccessors)) ?? InitAccessors;
			RecordClasses = sect.Attribute<bool?>(nameof(RecordClasses)) ?? RecordClasses;
			RecordStructs = sect.Attribute<bool?>(nameof(RecordStructs)) ?? RecordStructs;
			WithExpressions = sect.Attribute<bool?>(nameof(WithExpressions)) ?? WithExpressions;
			UsePrimaryConstructorSyntax = sect.Attribute<bool?>(nameof(UsePrimaryConstructorSyntax)) ?? UsePrimaryConstructorSyntax;
			FunctionPointers = sect.Attribute<bool?>(nameof(FunctionPointers)) ?? FunctionPointers;
			ScopedRef = sect.Attribute<bool?>(nameof(ScopedRef)) ?? ScopedRef;
			RequiredMembers = sect.Attribute<bool?>(nameof(RequiredMembers)) ?? RequiredMembers;
			SwitchExpressions = sect.Attribute<bool?>(nameof(SwitchExpressions)) ?? SwitchExpressions;
			FileScopedNamespaces = sect.Attribute<bool?>(nameof(FileScopedNamespaces)) ?? FileScopedNamespaces;
			AnonymousMethods = sect.Attribute<bool?>(nameof(AnonymousMethods)) ?? AnonymousMethods;
			AnonymousTypes = sect.Attribute<bool?>(nameof(AnonymousTypes)) ?? AnonymousTypes;
			UseLambdaSyntax = sect.Attribute<bool?>(nameof(UseLambdaSyntax)) ?? UseLambdaSyntax;
			ExpressionTrees = sect.Attribute<bool?>(nameof(ExpressionTrees)) ?? ExpressionTrees;
			YieldReturn = sect.Attribute<bool?>(nameof(YieldReturn)) ?? YieldReturn;
			Dynamic = sect.Attribute<bool?>(nameof(Dynamic)) ?? Dynamic;
			AsyncAwait = sect.Attribute<bool?>(nameof(AsyncAwait)) ?? AsyncAwait;
			AwaitInCatchFinally = sect.Attribute<bool?>(nameof(AwaitInCatchFinally)) ?? AwaitInCatchFinally;
			AsyncEnumerator = sect.Attribute<bool?>(nameof(AsyncEnumerator)) ?? AsyncEnumerator;
			DecimalConstants = sect.Attribute<bool?>(nameof(DecimalConstants)) ?? DecimalConstants;
			FixedBuffers = sect.Attribute<bool?>(nameof(FixedBuffers)) ?? FixedBuffers;
			StringConcat = sect.Attribute<bool?>(nameof(StringConcat)) ?? StringConcat;
			LiftNullables = sect.Attribute<bool?>(nameof(LiftNullables)) ?? LiftNullables;
			NullPropagation = sect.Attribute<bool?>(nameof(NullPropagation)) ?? NullPropagation;
			AutomaticProperties = sect.Attribute<bool?>(nameof(AutomaticProperties)) ?? AutomaticProperties;
			GetterOnlyAutomaticProperties = sect.Attribute<bool?>(nameof(GetterOnlyAutomaticProperties)) ?? GetterOnlyAutomaticProperties;
			AutomaticEvents = sect.Attribute<bool?>(nameof(AutomaticEvents)) ?? AutomaticEvents;
			UsingStatement = sect.Attribute<bool?>(nameof(UsingStatement)) ?? UsingStatement;
			UseEnhancedUsing = sect.Attribute<bool?>(nameof(UseEnhancedUsing)) ?? UseEnhancedUsing;
			AlwaysUseBraces = sect.Attribute<bool?>(nameof(AlwaysUseBraces)) ?? AlwaysUseBraces;
			ForEachStatement = sect.Attribute<bool?>(nameof(ForEachStatement)) ?? ForEachStatement;
			ForEachWithGetEnumeratorExtension = sect.Attribute<bool?>(nameof(ForEachWithGetEnumeratorExtension)) ?? ForEachWithGetEnumeratorExtension;
			LockStatement = sect.Attribute<bool?>(nameof(LockStatement)) ?? LockStatement;
			SwitchStatementOnString = sect.Attribute<bool?>(nameof(SwitchStatementOnString)) ?? SwitchStatementOnString;
			SparseIntegerSwitch = sect.Attribute<bool?>(nameof(SparseIntegerSwitch)) ?? SparseIntegerSwitch;
			UsingDeclarations = sect.Attribute<bool?>(nameof(UsingDeclarations)) ?? UsingDeclarations;
			ExtensionMethods = sect.Attribute<bool?>(nameof(ExtensionMethods)) ?? ExtensionMethods;
			QueryExpressions = sect.Attribute<bool?>(nameof(QueryExpressions)) ?? QueryExpressions;
			UseImplicitMethodGroupConversion = sect.Attribute<bool?>(nameof(UseImplicitMethodGroupConversion)) ?? UseImplicitMethodGroupConversion;
			AlwaysCastTargetsOfExplicitInterfaceImplementationCalls = sect.Attribute<bool?>(nameof(AlwaysCastTargetsOfExplicitInterfaceImplementationCalls)) ?? AlwaysCastTargetsOfExplicitInterfaceImplementationCalls;
			AlwaysQualifyMemberReferences = sect.Attribute<bool?>(nameof(AlwaysQualifyMemberReferences)) ?? AlwaysQualifyMemberReferences;
			AlwaysShowEnumMemberValues = sect.Attribute<bool?>(nameof(AlwaysShowEnumMemberValues)) ?? AlwaysShowEnumMemberValues;
			UseDebugSymbols = sect.Attribute<bool?>(nameof(UseDebugSymbols)) ?? UseDebugSymbols;
			ArrayInitializers = sect.Attribute<bool?>(nameof(ArrayInitializers)) ?? ArrayInitializers;
			ObjectOrCollectionInitializers = sect.Attribute<bool?>(nameof(ObjectOrCollectionInitializers)) ?? ObjectOrCollectionInitializers;
			DictionaryInitializers = sect.Attribute<bool?>(nameof(DictionaryInitializers)) ?? DictionaryInitializers;
			ExtensionMethodsInCollectionInitializers = sect.Attribute<bool?>(nameof(ExtensionMethodsInCollectionInitializers)) ?? ExtensionMethodsInCollectionInitializers;
			UseRefLocalsForAccurateOrderOfEvaluation = sect.Attribute<bool?>(nameof(UseRefLocalsForAccurateOrderOfEvaluation)) ?? UseRefLocalsForAccurateOrderOfEvaluation;
			RefExtensionMethods = sect.Attribute<bool?>(nameof(RefExtensionMethods)) ?? RefExtensionMethods;
			StringInterpolation = sect.Attribute<bool?>(nameof(StringInterpolation)) ?? StringInterpolation;
			Utf8StringLiterals = sect.Attribute<bool?>(nameof(Utf8StringLiterals)) ?? Utf8StringLiterals;
			UnsignedRightShift = sect.Attribute<bool?>(nameof(UnsignedRightShift)) ?? UnsignedRightShift;
			CheckedOperators = sect.Attribute<bool?>(nameof(UnsignedRightShift)) ?? UnsignedRightShift;
			ShowXmlDocumentation = sect.Attribute<bool?>(nameof(ShowXmlDocumentation)) ?? ShowXmlDocumentation;
			DecompileMemberBodies = sect.Attribute<bool?>(nameof(DecompileMemberBodies)) ?? DecompileMemberBodies;
			UseExpressionBodyForCalculatedGetterOnlyProperties = sect.Attribute<bool?>(nameof(UseExpressionBodyForCalculatedGetterOnlyProperties)) ?? UseExpressionBodyForCalculatedGetterOnlyProperties;
			OutVariables = sect.Attribute<bool?>(nameof(OutVariables)) ?? OutVariables;
			Discards = sect.Attribute<bool?>(nameof(Discards)) ?? Discards;
			IntroduceRefModifiersOnStructs = sect.Attribute<bool?>(nameof(IntroduceRefModifiersOnStructs)) ?? IntroduceRefModifiersOnStructs;
			IntroduceReadonlyAndInModifiers = sect.Attribute<bool?>(nameof(IntroduceReadonlyAndInModifiers)) ?? IntroduceReadonlyAndInModifiers;
			ReadOnlyMethods = sect.Attribute<bool?>(nameof(ReadOnlyMethods)) ?? ReadOnlyMethods;
			AsyncUsingAndForEachStatement = sect.Attribute<bool?>(nameof(AsyncUsingAndForEachStatement)) ?? AsyncUsingAndForEachStatement;
			IntroduceUnmanagedConstraint = sect.Attribute<bool?>(nameof(IntroduceUnmanagedConstraint)) ?? IntroduceUnmanagedConstraint;
			StackAllocInitializers = sect.Attribute<bool?>(nameof(StackAllocInitializers)) ?? StackAllocInitializers;
			PatternBasedFixedStatement = sect.Attribute<bool?>(nameof(PatternBasedFixedStatement)) ?? PatternBasedFixedStatement;
			TupleTypes = sect.Attribute<bool?>(nameof(TupleTypes)) ?? TupleTypes;
			ThrowExpressions = sect.Attribute<bool?>(nameof(ThrowExpressions)) ?? ThrowExpressions;
			NamedArguments = sect.Attribute<bool?>(nameof(NamedArguments)) ?? NamedArguments;
			NonTrailingNamedArguments = sect.Attribute<bool?>(nameof(NonTrailingNamedArguments)) ?? NonTrailingNamedArguments;
			OptionalArguments = sect.Attribute<bool?>(nameof(OptionalArguments)) ?? OptionalArguments;
			LocalFunctions = sect.Attribute<bool?>(nameof(LocalFunctions)) ?? LocalFunctions;
			Deconstruction = sect.Attribute<bool?>(nameof(Deconstruction)) ?? Deconstruction;
			PatternMatching = sect.Attribute<bool?>(nameof(PatternMatching)) ?? PatternMatching;
			StaticLocalFunctions = sect.Attribute<bool?>(nameof(StaticLocalFunctions)) ?? StaticLocalFunctions;
			Ranges = sect.Attribute<bool?>(nameof(Ranges)) ?? Ranges;
			NullableReferenceTypes = sect.Attribute<bool?>(nameof(NullableReferenceTypes)) ?? NullableReferenceTypes;
			AssumeArrayLengthFitsIntoInt32 = sect.Attribute<bool?>(nameof(AssumeArrayLengthFitsIntoInt32)) ?? AssumeArrayLengthFitsIntoInt32;
			IntroduceIncrementAndDecrement = sect.Attribute<bool?>(nameof(IntroduceIncrementAndDecrement)) ?? IntroduceIncrementAndDecrement;
			MakeAssignmentExpressions = sect.Attribute<bool?>(nameof(MakeAssignmentExpressions)) ?? MakeAssignmentExpressions;
			RemoveDeadCode = sect.Attribute<bool?>(nameof(RemoveDeadCode)) ?? RemoveDeadCode;
			RemoveDeadStores = sect.Attribute<bool?>(nameof(RemoveDeadStores)) ?? RemoveDeadStores;
			ForStatement = sect.Attribute<bool?>(nameof(ForStatement)) ?? ForStatement;
			DoWhileStatement = sect.Attribute<bool?>(nameof(DoWhileStatement)) ?? DoWhileStatement;
			SeparateLocalVariableDeclarations = sect.Attribute<bool?>(nameof(SeparateLocalVariableDeclarations)) ?? SeparateLocalVariableDeclarations;
			AggressiveScalarReplacementOfAggregates = sect.Attribute<bool?>(nameof(AggressiveScalarReplacementOfAggregates)) ?? AggressiveScalarReplacementOfAggregates;
			AggressiveInlining = sect.Attribute<bool?>(nameof(AggressiveInlining)) ?? AggressiveInlining;
			AlwaysUseGlobal = sect.Attribute<bool?>(nameof(AlwaysUseGlobal)) ?? AlwaysUseGlobal;
			RemoveEmptyDefaultConstructors = sect.Attribute<bool?>(nameof(RemoveEmptyDefaultConstructors)) ?? RemoveEmptyDefaultConstructors;
			ShowTokenAndRvaComments = sect.Attribute<bool?>(nameof(ShowTokenAndRvaComments)) ?? ShowTokenAndRvaComments;
			SortMembers = sect.Attribute<bool?>(nameof(SortMembers)) ?? SortMembers;
			ForceShowAllMembers = sect.Attribute<bool?>(nameof(ForceShowAllMembers)) ?? ForceShowAllMembers;
			SortSystemUsingStatementsFirst = sect.Attribute<bool?>(nameof(SortSystemUsingStatementsFirst)) ?? SortSystemUsingStatementsFirst;
			MaxArrayElements = sect.Attribute<int?>(nameof(MaxArrayElements)) ?? MaxArrayElements;
			MaxStringLength = sect.Attribute<int?>(nameof(MaxStringLength)) ?? MaxStringLength;
			SortCustomAttributes = sect.Attribute<bool?>(nameof(SortCustomAttributes)) ?? SortCustomAttributes;
			UseSourceCodeOrder = sect.Attribute<bool?>(nameof(UseSourceCodeOrder)) ?? UseSourceCodeOrder;
			AllowFieldInitializers = sect.Attribute<bool?>(nameof(AllowFieldInitializers)) ?? AllowFieldInitializers;
			OneCustomAttributePerLine = sect.Attribute<bool?>(nameof(OneCustomAttributePerLine)) ?? OneCustomAttributePerLine;
			TypeAddInternalModifier = sect.Attribute<bool?>(nameof(TypeAddInternalModifier)) ?? TypeAddInternalModifier;
			MemberAddPrivateModifier = sect.Attribute<bool?>(nameof(MemberAddPrivateModifier)) ?? MemberAddPrivateModifier;
			HexadecimalNumbers = sect.Attribute<bool?>(nameof(HexadecimalNumbers)) ?? HexadecimalNumbers;
			SortSwitchCasesByILOffset = sect.Attribute<bool?>(nameof(SortSwitchCasesByILOffset)) ?? SortSwitchCasesByILOffset;
			InsertParenthesesForReadability = sect.Attribute<bool?>(nameof(InsertParenthesesForReadability)) ?? InsertParenthesesForReadability;
			FullyQualifyAmbiguousTypeNames = sect.Attribute<bool?>(nameof(FullyQualifyAmbiguousTypeNames)) ?? FullyQualifyAmbiguousTypeNames;
			FullyQualifyAllTypes = sect.Attribute<bool?>(nameof(FullyQualifyAllTypes)) ?? FullyQualifyAllTypes;
			AlwaysGenerateExceptionVariableForCatchBlocksUnlessTypeIsObject = sect.Attribute<bool?>(nameof(AlwaysGenerateExceptionVariableForCatchBlocksUnlessTypeIsObject)) ?? AlwaysGenerateExceptionVariableForCatchBlocksUnlessTypeIsObject;

			//TODO: CSharpFormattingOptions
			disableSave = false;
		}
		readonly bool disableSave;

		protected override void OnModified() {
			if (disableSave)
				return;

			var sect = settingsService.RecreateSection(SETTINGS_GUID);
			// Only save those settings that can be changed in the dialog box
			sect.Attribute(nameof(DecompilationObject0), DecompilationObject0);
			sect.Attribute(nameof(DecompilationObject1), DecompilationObject1);
			sect.Attribute(nameof(DecompilationObject2), DecompilationObject2);
			sect.Attribute(nameof(DecompilationObject3), DecompilationObject3);
			sect.Attribute(nameof(DecompilationObject4), DecompilationObject4);
			sect.Attribute(nameof(NativeIntegers), NativeIntegers);
			sect.Attribute(nameof(NumericIntPtr), NumericIntPtr);
			sect.Attribute(nameof(CovariantReturns), CovariantReturns);
			sect.Attribute(nameof(InitAccessors), InitAccessors);
			sect.Attribute(nameof(RecordClasses), RecordClasses);
			sect.Attribute(nameof(RecordStructs), RecordStructs);
			sect.Attribute(nameof(WithExpressions), WithExpressions);
			sect.Attribute(nameof(UsePrimaryConstructorSyntax), UsePrimaryConstructorSyntax);
			sect.Attribute(nameof(FunctionPointers), FunctionPointers);
			sect.Attribute(nameof(ScopedRef), ScopedRef);
			sect.Attribute(nameof(RequiredMembers), RequiredMembers);
			sect.Attribute(nameof(SwitchExpressions), SwitchExpressions);
			sect.Attribute(nameof(FileScopedNamespaces), FileScopedNamespaces);
			sect.Attribute(nameof(AnonymousMethods), AnonymousMethods);
			sect.Attribute(nameof(AnonymousTypes), AnonymousTypes);
			sect.Attribute(nameof(UseLambdaSyntax), UseLambdaSyntax);
			sect.Attribute(nameof(ExpressionTrees), ExpressionTrees);
			sect.Attribute(nameof(YieldReturn), YieldReturn);
			sect.Attribute(nameof(Dynamic), Dynamic);
			sect.Attribute(nameof(AsyncAwait), AsyncAwait);
			sect.Attribute(nameof(AwaitInCatchFinally), AwaitInCatchFinally);
			sect.Attribute(nameof(AsyncEnumerator), AsyncEnumerator);
			sect.Attribute(nameof(DecimalConstants), DecimalConstants);
			sect.Attribute(nameof(FixedBuffers), FixedBuffers);
			sect.Attribute(nameof(StringConcat), StringConcat);
			sect.Attribute(nameof(LiftNullables), LiftNullables);
			sect.Attribute(nameof(NullPropagation), NullPropagation);
			sect.Attribute(nameof(AutomaticProperties), AutomaticProperties);
			sect.Attribute(nameof(GetterOnlyAutomaticProperties), GetterOnlyAutomaticProperties);
			sect.Attribute(nameof(AutomaticEvents), AutomaticEvents);
			sect.Attribute(nameof(UsingStatement), UsingStatement);
			sect.Attribute(nameof(UseEnhancedUsing), UseEnhancedUsing);
			sect.Attribute(nameof(AlwaysUseBraces), AlwaysUseBraces);
			sect.Attribute(nameof(ForEachStatement), ForEachStatement);
			sect.Attribute(nameof(ForEachWithGetEnumeratorExtension), ForEachWithGetEnumeratorExtension);
			sect.Attribute(nameof(LockStatement), LockStatement);
			sect.Attribute(nameof(SwitchStatementOnString), SwitchStatementOnString);
			sect.Attribute(nameof(SparseIntegerSwitch), SparseIntegerSwitch);
			sect.Attribute(nameof(UsingDeclarations), UsingDeclarations);
			sect.Attribute(nameof(ExtensionMethods), ExtensionMethods);
			sect.Attribute(nameof(QueryExpressions), QueryExpressions);
			sect.Attribute(nameof(UseImplicitMethodGroupConversion), UseImplicitMethodGroupConversion);
			sect.Attribute(nameof(AlwaysCastTargetsOfExplicitInterfaceImplementationCalls), AlwaysCastTargetsOfExplicitInterfaceImplementationCalls);
			sect.Attribute(nameof(AlwaysQualifyMemberReferences), AlwaysQualifyMemberReferences);
			sect.Attribute(nameof(AlwaysShowEnumMemberValues), AlwaysShowEnumMemberValues);
			sect.Attribute(nameof(UseDebugSymbols), UseDebugSymbols);
			sect.Attribute(nameof(ArrayInitializers), ArrayInitializers);
			sect.Attribute(nameof(ObjectOrCollectionInitializers), ObjectOrCollectionInitializers);
			sect.Attribute(nameof(DictionaryInitializers), DictionaryInitializers);
			sect.Attribute(nameof(ExtensionMethodsInCollectionInitializers), ExtensionMethodsInCollectionInitializers);
			sect.Attribute(nameof(UseRefLocalsForAccurateOrderOfEvaluation), UseRefLocalsForAccurateOrderOfEvaluation);
			sect.Attribute(nameof(RefExtensionMethods), RefExtensionMethods);
			sect.Attribute(nameof(StringInterpolation), StringInterpolation);
			sect.Attribute(nameof(Utf8StringLiterals), Utf8StringLiterals);
			sect.Attribute(nameof(UnsignedRightShift), UnsignedRightShift);
			sect.Attribute(nameof(CheckedOperators), CheckedOperators);
			sect.Attribute(nameof(ShowXmlDocumentation), ShowXmlDocumentation);
			sect.Attribute(nameof(DecompileMemberBodies), DecompileMemberBodies);
			sect.Attribute(nameof(UseExpressionBodyForCalculatedGetterOnlyProperties), UseExpressionBodyForCalculatedGetterOnlyProperties);
			sect.Attribute(nameof(OutVariables), OutVariables);
			sect.Attribute(nameof(Discards), Discards);
			sect.Attribute(nameof(IntroduceRefModifiersOnStructs), IntroduceRefModifiersOnStructs);
			sect.Attribute(nameof(IntroduceReadonlyAndInModifiers), IntroduceReadonlyAndInModifiers);
			sect.Attribute(nameof(ReadOnlyMethods), ReadOnlyMethods);
			sect.Attribute(nameof(AsyncUsingAndForEachStatement), AsyncUsingAndForEachStatement);
			sect.Attribute(nameof(IntroduceUnmanagedConstraint), IntroduceUnmanagedConstraint);
			sect.Attribute(nameof(StackAllocInitializers), StackAllocInitializers);
			sect.Attribute(nameof(PatternBasedFixedStatement), PatternBasedFixedStatement);
			sect.Attribute(nameof(TupleTypes), TupleTypes);
			sect.Attribute(nameof(ThrowExpressions), ThrowExpressions);
			sect.Attribute(nameof(NamedArguments), NamedArguments);
			sect.Attribute(nameof(NonTrailingNamedArguments), NonTrailingNamedArguments);
			sect.Attribute(nameof(OptionalArguments), OptionalArguments);
			sect.Attribute(nameof(LocalFunctions), LocalFunctions);
			sect.Attribute(nameof(Deconstruction), Deconstruction);
			sect.Attribute(nameof(PatternMatching), PatternMatching);
			sect.Attribute(nameof(StaticLocalFunctions), StaticLocalFunctions);
			sect.Attribute(nameof(Ranges), Ranges);
			sect.Attribute(nameof(NullableReferenceTypes), NullableReferenceTypes);
			sect.Attribute(nameof(AssumeArrayLengthFitsIntoInt32), AssumeArrayLengthFitsIntoInt32);
			sect.Attribute(nameof(IntroduceIncrementAndDecrement), IntroduceIncrementAndDecrement);
			sect.Attribute(nameof(MakeAssignmentExpressions), MakeAssignmentExpressions);
			sect.Attribute(nameof(RemoveDeadCode), RemoveDeadCode);
			sect.Attribute(nameof(RemoveDeadStores), RemoveDeadStores);
			sect.Attribute(nameof(ForStatement), ForStatement);
			sect.Attribute(nameof(DoWhileStatement), DoWhileStatement);
			sect.Attribute(nameof(SeparateLocalVariableDeclarations), SeparateLocalVariableDeclarations);
			sect.Attribute(nameof(AggressiveScalarReplacementOfAggregates), AggressiveScalarReplacementOfAggregates);
			sect.Attribute(nameof(AggressiveInlining), AggressiveInlining);
			sect.Attribute(nameof(AlwaysUseGlobal), AlwaysUseGlobal);
			sect.Attribute(nameof(RemoveEmptyDefaultConstructors), RemoveEmptyDefaultConstructors);
			sect.Attribute(nameof(ShowTokenAndRvaComments), ShowTokenAndRvaComments);
			sect.Attribute(nameof(SortMembers), SortMembers);
			sect.Attribute(nameof(ForceShowAllMembers), ForceShowAllMembers);
			sect.Attribute(nameof(SortSystemUsingStatementsFirst), SortSystemUsingStatementsFirst);
			sect.Attribute(nameof(MaxArrayElements), MaxArrayElements);
			sect.Attribute(nameof(MaxStringLength), MaxStringLength);
			sect.Attribute(nameof(SortCustomAttributes), SortCustomAttributes);
			sect.Attribute(nameof(UseSourceCodeOrder), UseSourceCodeOrder);
			sect.Attribute(nameof(AllowFieldInitializers), AllowFieldInitializers);
			sect.Attribute(nameof(OneCustomAttributePerLine), OneCustomAttributePerLine);
			sect.Attribute(nameof(TypeAddInternalModifier), TypeAddInternalModifier);
			sect.Attribute(nameof(MemberAddPrivateModifier), MemberAddPrivateModifier);
			sect.Attribute(nameof(HexadecimalNumbers), HexadecimalNumbers);
			sect.Attribute(nameof(SortSwitchCasesByILOffset), SortSwitchCasesByILOffset);
			sect.Attribute(nameof(InsertParenthesesForReadability), InsertParenthesesForReadability);
			sect.Attribute(nameof(FullyQualifyAmbiguousTypeNames), FullyQualifyAmbiguousTypeNames);
			sect.Attribute(nameof(FullyQualifyAllTypes), FullyQualifyAllTypes);
			sect.Attribute(nameof(AlwaysGenerateExceptionVariableForCatchBlocksUnlessTypeIsObject), AlwaysGenerateExceptionVariableForCatchBlocksUnlessTypeIsObject);

			//sect.Attribute(nameof(FullyQualifyAllTypes), FullyQualifyAllTypes);
			//TODO: CSharpFormattingOptions
		}
	}
}
