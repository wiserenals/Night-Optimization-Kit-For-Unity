using System;
using System.Reflection;

public class MethodObject
{
    public object objectOfMethod;
    public MethodInfo methodInfo;
    private Action action;
    public int customOrder;

    public MethodObject(object objectOfMethod, MethodInfo methodInfo, Action action, int order)
    {
        this.objectOfMethod = objectOfMethod;
        this.methodInfo = methodInfo;
        this.action = action;
        customOrder = order;
    }

    public void Call(params object[] parameters)
    {
        action();
    }
}