namespace KC.App.Models.Interfaces
{
    public interface IIdentityBearer<T>
    {
        T Id { get; }
    }
}
