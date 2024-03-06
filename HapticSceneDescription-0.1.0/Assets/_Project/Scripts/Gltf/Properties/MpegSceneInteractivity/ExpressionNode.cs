using System;
using System.Collections.Generic;

namespace HapticSceneDescription.Gltf.Properties.MpegSceneInteractivity
{
    public interface IExpressionNode
    {
        bool Evaluate(Dictionary<int, Func<bool>> methods);

        public static class Factory
        {
            public static IExpressionNode Parse(string expression)
            {
                expression = expression.Replace(" ", "");
                Stack<IExpressionNode> nodeStack = new Stack<IExpressionNode>();
                Stack<char> operatorStack = new Stack<char>();
                int index = 0;
                while (index < expression.Length)
                {
                    char c = expression[index];
                    if (c == '#')
                    {
                        int conditionId = 0;
                        index++;
                        while (index < expression.Length && char.IsDigit(expression[index]))
                        {
                            conditionId = conditionId * 10 + (expression[index] - '0');
                            index++;
                        }
                        nodeStack.Push(new AtomicConditionNode(conditionId));
                    }
                    else if (c == '(')
                    {
                        operatorStack.Push(c);
                        index++;
                    }
                    else if (c == ')')
                    {
                        while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                        {
                            char op = operatorStack.Pop();
                            if (op == '~')
                            {
                                IExpressionNode right = nodeStack.Pop();
                                nodeStack.Push(new LogicalOperatorNode(right, null, op));
                            }
                            else
                            {
                                IExpressionNode right = nodeStack.Pop();
                                IExpressionNode left = nodeStack.Pop();
                                nodeStack.Push(new LogicalOperatorNode(left, right, op));
                            }
                        }
                        operatorStack.Pop();
                        index++;
                    }
                    else if (c == '~' || c == '&' || c == '|')
                    {
                        while (operatorStack.Count > 0 && GetPriority(operatorStack.Peek()) >= GetPriority(c))
                        {
                            char op = operatorStack.Pop();
                            if (op == '~')
                            {
                                IExpressionNode right = nodeStack.Pop();
                                nodeStack.Push(new LogicalOperatorNode(right, null, op));
                            }
                            else
                            {
                                IExpressionNode right = nodeStack.Pop();
                                IExpressionNode left = nodeStack.Pop();
                                nodeStack.Push(new LogicalOperatorNode(left, right, op));
                            }
                        }
                        operatorStack.Push(c);
                        index++;
                    }

                }

                while (operatorStack.Count > 0)
                {
                    char op = operatorStack.Pop();
                    if (op == '~')
                    {
                        IExpressionNode right = nodeStack.Pop();
                        nodeStack.Push(new LogicalOperatorNode(right, null, op));
                    }
                    else
                    {
                        IExpressionNode right = nodeStack.Pop();
                        IExpressionNode left = nodeStack.Pop();
                        nodeStack.Push(new LogicalOperatorNode(left, right, op));
                    }
                }

                return nodeStack.Pop();
            }

            private static int GetPriority(char v)
            {
                return v switch
                {
                    '~' => 3,
                    '&' => 2,
                    '|' => 1,
                    _ => 0,
                };
            }
        }

    }

    public class AtomicConditionNode : IExpressionNode
    {
        public int ConditionId { get; }

        public AtomicConditionNode(int conditionId)
        {
            ConditionId = conditionId;
        }

        public bool Evaluate(Dictionary<int, Func<bool>> methods)
        {
            if (methods.TryGetValue(ConditionId, out Func<bool> method) && method != null)
            {
                return method.Invoke();
            }
            throw new ArgumentException($"Method not found for condition: {ConditionId}");
        }
    }

    public class LogicalOperatorNode : IExpressionNode
    {
        public IExpressionNode Left { get; }
        public IExpressionNode Right { get; }
        public char Operator { get; }

        public LogicalOperatorNode(IExpressionNode left, IExpressionNode right, char @operator)
        {
            Left = left;
            Right = right;
            Operator = @operator;
        }

        public bool Evaluate(Dictionary<int, Func<bool>> methods)
        {
            if (Operator == '&')
            {
                return Left.Evaluate(methods) && Right.Evaluate(methods);
            }
            else if (Operator == '|')
            {
                return Left.Evaluate(methods) || Right.Evaluate(methods);
            }
            else if (Operator == '~')
            {
                return !Left.Evaluate(methods);
            }
            else
            {
                throw new ArgumentException($"Invalid operator: {Operator}");
            }
        }
    }

}