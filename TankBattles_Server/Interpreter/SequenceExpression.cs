
public class SequenceExpression : IExpression
{
    private readonly List<IExpression> _expressions;

    public SequenceExpression(List<IExpression> expressions)
    {
        _expressions = expressions;
    }

    public void Interpret(InterpreterContext context)
    {
        foreach (IExpression expression in _expressions)
        {
            expression.Interpret(context);
        }
    }
}
