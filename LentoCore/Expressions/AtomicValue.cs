﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LentoCore.Atoms;
using LentoCore.Evaluator;
using LentoCore.Exception;
using LentoCore.Util;

namespace LentoCore.Expressions
{
    class AtomicValue<TAtom> : Expression where TAtom : Atomic
    {
        private readonly TAtom _value;

        public AtomicValue(TAtom value, LineColumnSpan span) : base(span)
        {
            _value = value;
        }

        public override Atomic Evaluate(Scope scope)
        {
            if (_value is Identifier identifier)
            {
                if (!scope.Contains(identifier.Name)) throw new RuntimeErrorException(ErrorHandler.EvaluateError(Span.Start, $"Undefined variable '{identifier.Name}'"));
                return scope.Get(identifier.Name);
            }

            if (_value is IdentifierDotList identifierDotList)
            {
                throw new NotImplementedException($"Error at line {Span.Start.Line} column {Span.Start.Column}: Language feature not supported! Usage of identifier lists are not yet implemented!");
            }
            return _value;
        }

        public override string ToString(string indent) => $"Atomic {typeof(TAtom).Name}: {_value.ToString()}";
    }
    /*
    class AtomicValue : AtomicValue<Atom>
    {
        public AtomicValue(Atom value, LineColumnSpan span) : base(value, span) { }
    }
    */
}
