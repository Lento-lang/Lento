﻿using LentoCore.Atoms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LentoCore.Exception;
using LentoCore.Parser;
using LentoCore.Util;

namespace LentoCore.Expressions
{
    class Prefix : Expression
    {
        private readonly PrefixOperator _operator;
        private readonly Expression _rhs;

        public Prefix(PrefixOperator @operator, Expression rhs, LineColumnSpan span) : base(span)
        {
            _operator = @operator;
            _rhs = rhs;
        }

        private Atoms.Tuple EvaluateTupleElements(Atoms.Tuple tuple, PrefixOperator op)
        {
            Prefix[] negativeElementsExpressions = tuple.BaseExpression.Elements.Select(e => new Prefix(op, e, e.Span)).ToArray();
            Atomic[] negativeAtoms = negativeElementsExpressions.Select(n => n.Evaluate()).ToArray();
            tuple.Elements = negativeAtoms;
            return tuple;
        }

        public override Atomic Evaluate()
        {
            Atomic value = _rhs.Evaluate();
            switch (_operator)
            {
                case PrefixOperator.Negative:
                {
                    if (value is Integer @integer) return new Atoms.Integer(@integer.Value * -1);
                    if (value is Float @float) return new Atoms.Float(@float.Value * -1);
                    if (value is Atoms.Tuple @tuple) return EvaluateTupleElements(@tuple, _operator);
                    throw new EvaluateErrorException(ErrorHandler.EvaluateErrorTypeMismatch(_rhs.Span.Start, value, typeof(Integer), typeof(Float), typeof(Atoms.Tuple)));
                }
                case PrefixOperator.Not:
                {
                    if (value is Atoms.Boolean @bool) return new Atoms.Boolean(!@bool.Value);
                    if (value is Atoms.Tuple @tuple) return EvaluateTupleElements(@tuple, _operator);
                    throw new EvaluateErrorException(ErrorHandler.EvaluateErrorTypeMismatch(_rhs.Span.Start, value, typeof(Atoms.Boolean), typeof(Atoms.Tuple)));
                }
                case PrefixOperator.Referenced:
                {
                    // Find reference in scope
                    if (value is Atoms.Identifier @ident) return new Reference(@ident);
                    if (value is Atoms.IdentifierDotList @identDotList) return new Reference(@identDotList);
                    throw new EvaluateErrorException(ErrorHandler.EvaluateErrorTypeMismatch(_rhs.Span.Start, value, typeof(Atoms.Identifier)));
                }
                default: throw new EvaluateErrorException(ErrorHandler.EvaluateError(Span.Start, $"Could not evaluate {_operator}. Invalid prefix operator!"));
            }
        }

        public override string ToString() => $"{_operator}({_rhs.ToString()})";
    }
}
