namespace KC.Shared.Models.Misc;

public class SignalRMethod<T>(string methodName)
{
    public string MethodName { get; } = methodName;
}