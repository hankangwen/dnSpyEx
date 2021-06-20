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
using System.ComponentModel.Composition;
using System.Diagnostics;
using dnSpy.Contracts.Settings;
using ICSharpCode.Decompiler;
using ICSharpCode.NRefactory.CSharp;

namespace dnSpy.Decompiler.ILSpy.Settings {
	[Export]
	sealed class DecompilerSettingsImpl : DecompilerSettings {
		static readonly Guid SETTINGS_GUID = new Guid("6745457F-254B-4B7B-90F1-F948F0721C3B");

		readonly ISettingsService settingsService;

		[ImportingConstructor]
		DecompilerSettingsImpl(ISettingsService settingsService) {
			this.settingsService = settingsService;

			disableSave = true;
			var sect = settingsService.GetOrCreateSection(SETTINGS_GUID);
			// Only read those settings that can be changed in the dialog box
			DecompilationObject0 = sect.Attribute<DecompilationObject?>(nameof(DecompilationObject0)) ?? DecompilationObject0;
			DecompilationObject1 = sect.Attribute<DecompilationObject?>(nameof(DecompilationObject1)) ?? DecompilationObject1;
			DecompilationObject2 = sect.Attribute<DecompilationObject?>(nameof(DecompilationObject2)) ?? DecompilationObject2;
			DecompilationObject3 = sect.Attribute<DecompilationObject?>(nameof(DecompilationObject3)) ?? DecompilationObject3;
			DecompilationObject4 = sect.Attribute<DecompilationObject?>(nameof(DecompilationObject4)) ?? DecompilationObject4;
			AnonymousMethods = sect.Attribute<bool?>(nameof(AnonymousMethods)) ?? AnonymousMethods;
			ExpressionTrees = sect.Attribute<bool?>(nameof(ExpressionTrees)) ?? ExpressionTrees;
			YieldReturn = sect.Attribute<bool?>(nameof(YieldReturn)) ?? YieldReturn;
			AsyncAwait = sect.Attribute<bool?>(nameof(AsyncAwait)) ?? AsyncAwait;
			//AutomaticProperties = sect.Attribute<bool?>(nameof(AutomaticProperties)) ?? AutomaticProperties;
			//AutomaticEvents = sect.Attribute<bool?>(nameof(AutomaticEvents)) ?? AutomaticEvents;
			//UsingStatement = sect.Attribute<bool?>(nameof(UsingStatement)) ?? UsingStatement;
			//ForEachStatement = sect.Attribute<bool?>(nameof(ForEachStatement)) ?? ForEachStatement;
			//LockStatement = sect.Attribute<bool?>(nameof(LockStatement)) ?? LockStatement;
			//SwitchStatementOnString = sect.Attribute<bool?>(nameof(SwitchStatementOnString)) ?? SwitchStatementOnString;
			//UsingDeclarations = sect.Attribute<bool?>(nameof(UsingDeclarations)) ?? UsingDeclarations;
			QueryExpressions = sect.Attribute<bool?>(nameof(QueryExpressions)) ?? QueryExpressions;
			FullyQualifyAmbiguousTypeNames = sect.Attribute<bool?>(nameof(FullyQualifyAmbiguousTypeNames)) ?? FullyQualifyAmbiguousTypeNames;
			FullyQualifyAllTypes = sect.Attribute<bool?>(nameof(FullyQualifyAllTypes)) ?? FullyQualifyAllTypes;
			UseDebugSymbols = sect.Attribute<bool?>(nameof(UseDebugSymbols)) ?? UseDebugSymbols;
			//ObjectOrCollectionInitializers = sect.Attribute<bool?>(nameof(ObjectOrCollectionInitializers)) ?? ObjectOrCollectionInitializers;
			ShowXmlDocumentation = sect.Attribute<bool?>(nameof(ShowXmlDocumentation)) ?? ShowXmlDocumentation;
			RemoveEmptyDefaultConstructors = sect.Attribute<bool?>(nameof(RemoveEmptyDefaultConstructors)) ?? RemoveEmptyDefaultConstructors;
			//IntroduceIncrementAndDecrement = sect.Attribute<bool?>(nameof(IntroduceIncrementAndDecrement)) ?? IntroduceIncrementAndDecrement;
			//MakeAssignmentExpressions = sect.Attribute<bool?>(nameof(MakeAssignmentExpressions)) ?? MakeAssignmentExpressions;
			//AlwaysGenerateExceptionVariableForCatchBlocksUnlessTypeIsObject = sect.Attribute<bool?>(nameof(AlwaysGenerateExceptionVariableForCatchBlocksUnlessTypeIsObject)) ?? AlwaysGenerateExceptionVariableForCatchBlocksUnlessTypeIsObject;
			ShowTokenAndRvaComments = sect.Attribute<bool?>(nameof(ShowTokenAndRvaComments)) ?? ShowTokenAndRvaComments;
			SortMembers = sect.Attribute<bool?>(nameof(SortMembers)) ?? SortMembers;
			ForceShowAllMembers = sect.Attribute<bool?>(nameof(ForceShowAllMembers)) ?? ForceShowAllMembers;
			SortSystemUsingStatementsFirst = sect.Attribute<bool?>(nameof(SortSystemUsingStatementsFirst)) ?? SortSystemUsingStatementsFirst;
			//MaxArrayElements = sect.Attribute<int?>(nameof(MaxArrayElements)) ?? MaxArrayElements;
			SortCustomAttributes = sect.Attribute<bool?>(nameof(SortCustomAttributes)) ?? SortCustomAttributes;
			UseSourceCodeOrder = sect.Attribute<bool?>(nameof(UseSourceCodeOrder)) ?? UseSourceCodeOrder;
			AllowFieldInitializers = sect.Attribute<bool?>(nameof(AllowFieldInitializers)) ?? AllowFieldInitializers;
			OneCustomAttributePerLine = sect.Attribute<bool?>(nameof(OneCustomAttributePerLine)) ?? OneCustomAttributePerLine;
			TypeAddInternalModifier = sect.Attribute<bool?>(nameof(TypeAddInternalModifier)) ?? TypeAddInternalModifier;
			MemberAddPrivateModifier = sect.Attribute<bool?>(nameof(MemberAddPrivateModifier)) ?? MemberAddPrivateModifier;
			//RemoveNewDelegateClass = sect.Attribute<bool?>(nameof(RemoveNewDelegateClass)) ?? RemoveNewDelegateClass;
			HexadecimalNumbers = sect.Attribute<bool?>(nameof(HexadecimalNumbers)) ?? HexadecimalNumbers;

			CSharpFormattingOptions.IndentSwitchBody = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.IndentSwitchBody)) ?? CSharpFormattingOptions.IndentSwitchBody;
			CSharpFormattingOptions.IndentCaseBody = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.IndentCaseBody)) ?? CSharpFormattingOptions.IndentCaseBody;
			CSharpFormattingOptions.AutoPropertyFormatting = sect.Attribute<PropertyFormatting?>(nameof(CSharpFormattingOptions.AutoPropertyFormatting)) ?? CSharpFormattingOptions.AutoPropertyFormatting;

			CSharpFormattingOptions.NamespaceBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.NamespaceBraceStyle)) ?? CSharpFormattingOptions.NamespaceBraceStyle;
			CSharpFormattingOptions.ClassBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.ClassBraceStyle)) ?? CSharpFormattingOptions.ClassBraceStyle;
			CSharpFormattingOptions.InterfaceBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.InterfaceBraceStyle)) ?? CSharpFormattingOptions.InterfaceBraceStyle;
			CSharpFormattingOptions.StructBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.StructBraceStyle)) ?? CSharpFormattingOptions.StructBraceStyle;
			CSharpFormattingOptions.EnumBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.EnumBraceStyle)) ?? CSharpFormattingOptions.EnumBraceStyle;
			CSharpFormattingOptions.MethodBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.MethodBraceStyle)) ?? CSharpFormattingOptions.MethodBraceStyle;
			CSharpFormattingOptions.AnonymousMethodBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.AnonymousMethodBraceStyle)) ?? CSharpFormattingOptions.AnonymousMethodBraceStyle;
			CSharpFormattingOptions.ConstructorBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.ConstructorBraceStyle)) ?? CSharpFormattingOptions.ConstructorBraceStyle;
			CSharpFormattingOptions.DestructorBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.DestructorBraceStyle)) ?? CSharpFormattingOptions.DestructorBraceStyle;
			CSharpFormattingOptions.PropertyBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.PropertyBraceStyle)) ?? CSharpFormattingOptions.PropertyBraceStyle;
			CSharpFormattingOptions.PropertyGetBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.PropertyGetBraceStyle)) ?? CSharpFormattingOptions.PropertyGetBraceStyle;
			CSharpFormattingOptions.PropertySetBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.PropertySetBraceStyle)) ?? CSharpFormattingOptions.PropertySetBraceStyle;
			CSharpFormattingOptions.EventBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.EventBraceStyle)) ?? CSharpFormattingOptions.EventBraceStyle;
			CSharpFormattingOptions.EventAddBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.EventAddBraceStyle)) ?? CSharpFormattingOptions.EventAddBraceStyle;
			CSharpFormattingOptions.EventRemoveBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.EventRemoveBraceStyle)) ?? CSharpFormattingOptions.EventRemoveBraceStyle;
			CSharpFormattingOptions.StatementBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.StatementBraceStyle)) ?? CSharpFormattingOptions.StatementBraceStyle;

			CSharpFormattingOptions.ElseNewLinePlacement = sect.Attribute<NewLinePlacement?>(nameof(CSharpFormattingOptions.ElseNewLinePlacement)) ?? CSharpFormattingOptions.ElseNewLinePlacement;
			CSharpFormattingOptions.ElseIfNewLinePlacement = sect.Attribute<NewLinePlacement?>(nameof(CSharpFormattingOptions.ElseIfNewLinePlacement)) ?? CSharpFormattingOptions.ElseIfNewLinePlacement;
			CSharpFormattingOptions.CatchNewLinePlacement = sect.Attribute<NewLinePlacement?>(nameof(CSharpFormattingOptions.CatchNewLinePlacement)) ?? CSharpFormattingOptions.CatchNewLinePlacement;
			CSharpFormattingOptions.FinallyNewLinePlacement = sect.Attribute<NewLinePlacement?>(nameof(CSharpFormattingOptions.FinallyNewLinePlacement)) ?? CSharpFormattingOptions.FinallyNewLinePlacement;
			CSharpFormattingOptions.WhileNewLinePlacement = sect.Attribute<NewLinePlacement?>(nameof(CSharpFormattingOptions.WhileNewLinePlacement)) ?? CSharpFormattingOptions.WhileNewLinePlacement;

			CSharpFormattingOptions.SpaceBeforeMethodDeclarationParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeMethodDeclarationParentheses)) ?? CSharpFormattingOptions.SpaceBeforeMethodDeclarationParentheses;
			CSharpFormattingOptions.SpaceWithinMethodDeclarationParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceWithinMethodDeclarationParentheses)) ?? CSharpFormattingOptions.SpaceWithinMethodDeclarationParentheses;
			CSharpFormattingOptions.SpaceBeforeMethodCallParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeMethodCallParentheses)) ?? CSharpFormattingOptions.SpaceBeforeMethodCallParentheses;
			CSharpFormattingOptions.SpaceWithinMethodCallParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceWithinMethodCallParentheses)) ?? CSharpFormattingOptions.SpaceWithinMethodCallParentheses;
			CSharpFormattingOptions.SpaceBeforeConstructorDeclarationParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeConstructorDeclarationParentheses)) ?? CSharpFormattingOptions.SpaceBeforeConstructorDeclarationParentheses;
			CSharpFormattingOptions.SpaceBeforeDelegateDeclarationParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeDelegateDeclarationParentheses)) ?? CSharpFormattingOptions.SpaceBeforeDelegateDeclarationParentheses;
			CSharpFormattingOptions.SpaceBeforeIfParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeIfParentheses)) ?? CSharpFormattingOptions.SpaceBeforeIfParentheses;
			CSharpFormattingOptions.SpaceBeforeWhileParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeWhileParentheses)) ?? CSharpFormattingOptions.SpaceBeforeWhileParentheses;
			CSharpFormattingOptions.SpaceBeforeForParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeForParentheses)) ?? CSharpFormattingOptions.SpaceBeforeForParentheses;
			CSharpFormattingOptions.SpaceBeforeForeachParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeForeachParentheses)) ?? CSharpFormattingOptions.SpaceBeforeForeachParentheses;
			CSharpFormattingOptions.SpaceBeforeCatchParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeCatchParentheses)) ?? CSharpFormattingOptions.SpaceBeforeCatchParentheses;
			CSharpFormattingOptions.SpaceBeforeSwitchParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeSwitchParentheses)) ?? CSharpFormattingOptions.SpaceBeforeSwitchParentheses;
			CSharpFormattingOptions.SpaceBeforeLockParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeLockParentheses)) ?? CSharpFormattingOptions.SpaceBeforeLockParentheses;
			CSharpFormattingOptions.SpaceBeforeUsingParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeUsingParentheses)) ?? CSharpFormattingOptions.SpaceBeforeUsingParentheses;
			CSharpFormattingOptions.SpaceAroundAssignment = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceAroundAssignment)) ?? CSharpFormattingOptions.SpaceAroundAssignment;
			CSharpFormattingOptions.SpaceAroundLogicalOperator = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceAroundLogicalOperator)) ?? CSharpFormattingOptions.SpaceAroundLogicalOperator;
			CSharpFormattingOptions.SpaceAroundEqualityOperator = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceAroundEqualityOperator)) ?? CSharpFormattingOptions.SpaceAroundEqualityOperator;
			CSharpFormattingOptions.SpaceAroundRelationalOperator = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceAroundRelationalOperator)) ?? CSharpFormattingOptions.SpaceAroundRelationalOperator;
			CSharpFormattingOptions.SpaceAroundBitwiseOperator = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceAroundBitwiseOperator)) ?? CSharpFormattingOptions.SpaceAroundBitwiseOperator;
			CSharpFormattingOptions.SpaceAroundAdditiveOperator = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceAroundAdditiveOperator)) ?? CSharpFormattingOptions.SpaceAroundAdditiveOperator;
			CSharpFormattingOptions.SpaceAroundMultiplicativeOperator = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceAroundMultiplicativeOperator)) ?? CSharpFormattingOptions.SpaceAroundMultiplicativeOperator;
			CSharpFormattingOptions.SpaceAroundShiftOperator = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceAroundShiftOperator)) ?? CSharpFormattingOptions.SpaceAroundShiftOperator;
			CSharpFormattingOptions.SpacesWithinParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinParentheses)) ?? CSharpFormattingOptions.SpacesWithinParentheses;
			CSharpFormattingOptions.SpacesWithinIfParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinIfParentheses)) ?? CSharpFormattingOptions.SpacesWithinIfParentheses;
			CSharpFormattingOptions.SpacesWithinWhileParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinWhileParentheses)) ?? CSharpFormattingOptions.SpacesWithinWhileParentheses;
			CSharpFormattingOptions.SpacesWithinForParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinForParentheses)) ?? CSharpFormattingOptions.SpacesWithinForParentheses;
			CSharpFormattingOptions.SpacesWithinForeachParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinForeachParentheses)) ?? CSharpFormattingOptions.SpacesWithinForeachParentheses;
			CSharpFormattingOptions.SpacesWithinCatchParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinCatchParentheses)) ?? CSharpFormattingOptions.SpacesWithinCatchParentheses;
			CSharpFormattingOptions.SpacesWithinSwitchParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinSwitchParentheses)) ?? CSharpFormattingOptions.SpacesWithinSwitchParentheses;
			CSharpFormattingOptions.SpacesWithinLockParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinLockParentheses)) ?? CSharpFormattingOptions.SpacesWithinLockParentheses;
			CSharpFormattingOptions.SpacesWithinUsingParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinUsingParentheses)) ?? CSharpFormattingOptions.SpacesWithinUsingParentheses;
			CSharpFormattingOptions.SpacesWithinCastParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinCastParentheses)) ?? CSharpFormattingOptions.SpacesWithinCastParentheses;
			CSharpFormattingOptions.SpacesWithinSizeOfParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinSizeOfParentheses)) ?? CSharpFormattingOptions.SpacesWithinSizeOfParentheses;
			CSharpFormattingOptions.SpacesWithinTypeOfParentheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinTypeOfParentheses)) ?? CSharpFormattingOptions.SpacesWithinTypeOfParentheses;
			CSharpFormattingOptions.SpacesWithinCheckedExpressionParantheses = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinCheckedExpressionParantheses)) ?? CSharpFormattingOptions.SpacesWithinCheckedExpressionParantheses;
			CSharpFormattingOptions.SpaceBeforeConditionalOperatorCondition = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeConditionalOperatorCondition)) ?? CSharpFormattingOptions.SpaceBeforeConditionalOperatorCondition;
			CSharpFormattingOptions.SpaceAfterConditionalOperatorCondition = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceAfterConditionalOperatorCondition)) ?? CSharpFormattingOptions.SpaceAfterConditionalOperatorCondition;
			CSharpFormattingOptions.SpaceBeforeConditionalOperatorSeparator = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeConditionalOperatorSeparator)) ?? CSharpFormattingOptions.SpaceBeforeConditionalOperatorSeparator;
			CSharpFormattingOptions.SpaceAfterConditionalOperatorSeparator = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceAfterConditionalOperatorSeparator)) ?? CSharpFormattingOptions.SpaceAfterConditionalOperatorSeparator;
			CSharpFormattingOptions.SpacesWithinBrackets = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpacesWithinBrackets)) ?? CSharpFormattingOptions.SpacesWithinBrackets;
			CSharpFormattingOptions.SpaceBeforeBracketComma = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeBracketComma)) ?? CSharpFormattingOptions.SpaceBeforeBracketComma;
			CSharpFormattingOptions.SpaceAfterBracketComma = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceAfterBracketComma)) ?? CSharpFormattingOptions.SpaceAfterBracketComma;
			CSharpFormattingOptions.SpaceBeforeForSemicolon = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceBeforeForSemicolon)) ?? CSharpFormattingOptions.SpaceBeforeForSemicolon;
			CSharpFormattingOptions.SpaceAfterForSemicolon = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceAfterForSemicolon)) ?? CSharpFormattingOptions.SpaceAfterForSemicolon;
			CSharpFormattingOptions.SpaceAfterTypecast = sect.Attribute<bool?>(nameof(CSharpFormattingOptions.SpaceAfterTypecast)) ?? CSharpFormattingOptions.SpaceAfterTypecast;

			CSharpFormattingOptions.MinimumBlankLinesAfterUsings = sect.Attribute<int?>(nameof(CSharpFormattingOptions.MinimumBlankLinesAfterUsings)) ?? CSharpFormattingOptions.MinimumBlankLinesAfterUsings;
			CSharpFormattingOptions.MinimumBlankLinesBetweenMembers = sect.Attribute<int?>(nameof(CSharpFormattingOptions.MinimumBlankLinesBetweenMembers)) ?? CSharpFormattingOptions.MinimumBlankLinesBetweenMembers;

			CSharpFormattingOptions.ArrayInitializerWrapping = sect.Attribute<Wrapping?>(nameof(CSharpFormattingOptions.ArrayInitializerWrapping)) ?? CSharpFormattingOptions.ArrayInitializerWrapping;
			CSharpFormattingOptions.ArrayInitializerBraceStyle = sect.Attribute<BraceStyle?>(nameof(CSharpFormattingOptions.ArrayInitializerBraceStyle)) ?? CSharpFormattingOptions.ArrayInitializerBraceStyle;

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
			sect.Attribute(nameof(AnonymousMethods), AnonymousMethods);
			sect.Attribute(nameof(ExpressionTrees), ExpressionTrees);
			sect.Attribute(nameof(YieldReturn), YieldReturn);
			sect.Attribute(nameof(AsyncAwait), AsyncAwait);
			//sect.Attribute(nameof(AutomaticProperties), AutomaticProperties);
			//sect.Attribute(nameof(AutomaticEvents), AutomaticEvents);
			//sect.Attribute(nameof(UsingStatement), UsingStatement);
			//sect.Attribute(nameof(ForEachStatement), ForEachStatement);
			//sect.Attribute(nameof(LockStatement), LockStatement);
			//sect.Attribute(nameof(SwitchStatementOnString), SwitchStatementOnString);
			//sect.Attribute(nameof(UsingDeclarations), UsingDeclarations);
			sect.Attribute(nameof(QueryExpressions), QueryExpressions);
			sect.Attribute(nameof(FullyQualifyAmbiguousTypeNames), FullyQualifyAmbiguousTypeNames);
			sect.Attribute(nameof(FullyQualifyAllTypes), FullyQualifyAllTypes);
			sect.Attribute(nameof(UseDebugSymbols), UseDebugSymbols);
			//sect.Attribute(nameof(ObjectOrCollectionInitializers), ObjectOrCollectionInitializers);
			sect.Attribute(nameof(ShowXmlDocumentation), ShowXmlDocumentation);
			sect.Attribute(nameof(RemoveEmptyDefaultConstructors), RemoveEmptyDefaultConstructors);
			//sect.Attribute(nameof(IntroduceIncrementAndDecrement), IntroduceIncrementAndDecrement);
			//sect.Attribute(nameof(MakeAssignmentExpressions), MakeAssignmentExpressions);
			//sect.Attribute(nameof(AlwaysGenerateExceptionVariableForCatchBlocksUnlessTypeIsObject), AlwaysGenerateExceptionVariableForCatchBlocksUnlessTypeIsObject);
			sect.Attribute(nameof(ShowTokenAndRvaComments), ShowTokenAndRvaComments);
			sect.Attribute(nameof(SortMembers), SortMembers);
			sect.Attribute(nameof(ForceShowAllMembers), ForceShowAllMembers);
			sect.Attribute(nameof(SortSystemUsingStatementsFirst), SortSystemUsingStatementsFirst);
			//sect.Attribute(nameof(MaxArrayElements), MaxArrayElements);
			sect.Attribute(nameof(SortCustomAttributes), SortCustomAttributes);
			sect.Attribute(nameof(UseSourceCodeOrder), UseSourceCodeOrder);
			sect.Attribute(nameof(AllowFieldInitializers), AllowFieldInitializers);
			sect.Attribute(nameof(OneCustomAttributePerLine), OneCustomAttributePerLine);
			sect.Attribute(nameof(TypeAddInternalModifier), TypeAddInternalModifier);
			sect.Attribute(nameof(MemberAddPrivateModifier), MemberAddPrivateModifier);
			//sect.Attribute(nameof(RemoveNewDelegateClass), RemoveNewDelegateClass);
			sect.Attribute(nameof(HexadecimalNumbers), HexadecimalNumbers);

			sect.Attribute(nameof(CSharpFormattingOptions.IndentSwitchBody), CSharpFormattingOptions.IndentSwitchBody);
            sect.Attribute(nameof(CSharpFormattingOptions.IndentCaseBody), CSharpFormattingOptions.IndentCaseBody);
            sect.Attribute(nameof(CSharpFormattingOptions.AutoPropertyFormatting), CSharpFormattingOptions.AutoPropertyFormatting);

            sect.Attribute(nameof(CSharpFormattingOptions.NamespaceBraceStyle), CSharpFormattingOptions.NamespaceBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.ClassBraceStyle), CSharpFormattingOptions.ClassBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.InterfaceBraceStyle), CSharpFormattingOptions.InterfaceBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.StructBraceStyle), CSharpFormattingOptions.StructBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.EnumBraceStyle), CSharpFormattingOptions.EnumBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.MethodBraceStyle), CSharpFormattingOptions.MethodBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.AnonymousMethodBraceStyle), CSharpFormattingOptions.AnonymousMethodBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.ConstructorBraceStyle), CSharpFormattingOptions.ConstructorBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.DestructorBraceStyle), CSharpFormattingOptions.DestructorBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.PropertyBraceStyle), CSharpFormattingOptions.PropertyBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.PropertyGetBraceStyle), CSharpFormattingOptions.PropertyGetBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.PropertySetBraceStyle), CSharpFormattingOptions.PropertySetBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.EventBraceStyle), CSharpFormattingOptions.EventBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.EventAddBraceStyle), CSharpFormattingOptions.EventAddBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.EventRemoveBraceStyle), CSharpFormattingOptions.EventRemoveBraceStyle);
            sect.Attribute(nameof(CSharpFormattingOptions.StatementBraceStyle), CSharpFormattingOptions.StatementBraceStyle);

            sect.Attribute(nameof(CSharpFormattingOptions.ElseNewLinePlacement), CSharpFormattingOptions.ElseNewLinePlacement);
            sect.Attribute(nameof(CSharpFormattingOptions.ElseIfNewLinePlacement), CSharpFormattingOptions.ElseIfNewLinePlacement);
            sect.Attribute(nameof(CSharpFormattingOptions.CatchNewLinePlacement), CSharpFormattingOptions.CatchNewLinePlacement);
            sect.Attribute(nameof(CSharpFormattingOptions.FinallyNewLinePlacement), CSharpFormattingOptions.FinallyNewLinePlacement);
            sect.Attribute(nameof(CSharpFormattingOptions.WhileNewLinePlacement), CSharpFormattingOptions.WhileNewLinePlacement);

            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeMethodDeclarationParentheses), CSharpFormattingOptions.SpaceBeforeMethodDeclarationParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceWithinMethodDeclarationParentheses), CSharpFormattingOptions.SpaceWithinMethodDeclarationParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeMethodCallParentheses), CSharpFormattingOptions.SpaceBeforeMethodCallParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceWithinMethodCallParentheses), CSharpFormattingOptions.SpaceWithinMethodCallParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeConstructorDeclarationParentheses), CSharpFormattingOptions.SpaceBeforeConstructorDeclarationParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeDelegateDeclarationParentheses), CSharpFormattingOptions.SpaceBeforeDelegateDeclarationParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeIfParentheses), CSharpFormattingOptions.SpaceBeforeIfParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeWhileParentheses), CSharpFormattingOptions.SpaceBeforeWhileParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeForParentheses), CSharpFormattingOptions.SpaceBeforeForParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeForeachParentheses), CSharpFormattingOptions.SpaceBeforeForeachParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeCatchParentheses), CSharpFormattingOptions.SpaceBeforeCatchParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeSwitchParentheses), CSharpFormattingOptions.SpaceBeforeSwitchParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeLockParentheses), CSharpFormattingOptions.SpaceBeforeLockParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeUsingParentheses), CSharpFormattingOptions.SpaceBeforeUsingParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceAroundAssignment), CSharpFormattingOptions.SpaceAroundAssignment);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceAroundLogicalOperator), CSharpFormattingOptions.SpaceAroundLogicalOperator);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceAroundEqualityOperator), CSharpFormattingOptions.SpaceAroundEqualityOperator);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceAroundRelationalOperator), CSharpFormattingOptions.SpaceAroundRelationalOperator);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceAroundBitwiseOperator), CSharpFormattingOptions.SpaceAroundBitwiseOperator);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceAroundAdditiveOperator), CSharpFormattingOptions.SpaceAroundAdditiveOperator);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceAroundMultiplicativeOperator), CSharpFormattingOptions.SpaceAroundMultiplicativeOperator);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceAroundShiftOperator), CSharpFormattingOptions.SpaceAroundShiftOperator);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinParentheses), CSharpFormattingOptions.SpacesWithinParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinIfParentheses), CSharpFormattingOptions.SpacesWithinIfParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinWhileParentheses), CSharpFormattingOptions.SpacesWithinWhileParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinForParentheses), CSharpFormattingOptions.SpacesWithinForParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinForeachParentheses), CSharpFormattingOptions.SpacesWithinForeachParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinCatchParentheses), CSharpFormattingOptions.SpacesWithinCatchParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinSwitchParentheses), CSharpFormattingOptions.SpacesWithinSwitchParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinLockParentheses), CSharpFormattingOptions.SpacesWithinLockParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinUsingParentheses), CSharpFormattingOptions.SpacesWithinUsingParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinCastParentheses), CSharpFormattingOptions.SpacesWithinCastParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinSizeOfParentheses), CSharpFormattingOptions.SpacesWithinSizeOfParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinTypeOfParentheses), CSharpFormattingOptions.SpacesWithinTypeOfParentheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinCheckedExpressionParantheses), CSharpFormattingOptions.SpacesWithinCheckedExpressionParantheses);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeConditionalOperatorCondition), CSharpFormattingOptions.SpaceBeforeConditionalOperatorCondition);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceAfterConditionalOperatorCondition), CSharpFormattingOptions.SpaceAfterConditionalOperatorCondition);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeConditionalOperatorSeparator), CSharpFormattingOptions.SpaceBeforeConditionalOperatorSeparator);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceAfterConditionalOperatorSeparator), CSharpFormattingOptions.SpaceAfterConditionalOperatorSeparator);
            sect.Attribute(nameof(CSharpFormattingOptions.SpacesWithinBrackets), CSharpFormattingOptions.SpacesWithinBrackets);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeBracketComma), CSharpFormattingOptions.SpaceBeforeBracketComma);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceAfterBracketComma), CSharpFormattingOptions.SpaceAfterBracketComma);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceBeforeForSemicolon), CSharpFormattingOptions.SpaceBeforeForSemicolon);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceAfterForSemicolon), CSharpFormattingOptions.SpaceAfterForSemicolon);
            sect.Attribute(nameof(CSharpFormattingOptions.SpaceAfterTypecast), CSharpFormattingOptions.SpaceAfterTypecast);

            sect.Attribute(nameof(CSharpFormattingOptions.MinimumBlankLinesAfterUsings), CSharpFormattingOptions.MinimumBlankLinesAfterUsings);
            sect.Attribute(nameof(CSharpFormattingOptions.MinimumBlankLinesBetweenMembers), CSharpFormattingOptions.MinimumBlankLinesBetweenMembers);

            sect.Attribute(nameof(CSharpFormattingOptions.ArrayInitializerWrapping), CSharpFormattingOptions.ArrayInitializerWrapping);
            sect.Attribute(nameof(CSharpFormattingOptions.ArrayInitializerBraceStyle), CSharpFormattingOptions.ArrayInitializerBraceStyle);
		}
	}
}
