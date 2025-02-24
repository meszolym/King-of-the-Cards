namespace KC.App.Backend.Models.Interfaces
{
    public interface IIdentityBearer<T>
    {
        T Id { get; }
    }
}
